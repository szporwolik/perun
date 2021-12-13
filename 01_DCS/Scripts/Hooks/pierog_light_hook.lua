-- Pierog for DCS World, inspired by
-- https://github.com/szporwolik/perun -> DCS Hook component
net.log("[Pierog] Loading Pierog")

-- Initial init
local Pierog = {}

-- Load Luas
package.path  = package.path..";"..lfs.currentdir().."/LuaSocket/?.lua"..";"..lfs.writedir() .. "/Mods/services/Pierog/lua/?.lua"
package.cpath = package.cpath..";"..lfs.currentdir().."/LuaSocket/?.dll"

-- Load Dlls
package.cpath = package.cpath..';'.. lfs.writedir()..'/Mods/services/Pierog/bin/' ..'?.dll;'
net.log("[Pierog]", "Loading DLL")
Pierog.DLL = require('pierog')

-- Load config file
net.log("[Pierog]", "Loading config")
local config = require "pierog_config"

Pierog.host = config.host;
Pierog.port = config.port;
Pierog.delimiters = config.delimiters;
Pierog.Instance = config.Instance;
Pierog.DebugMode = config.DebugMode;
Pierog.logPath = config.logPath;

Pierog.MOTD_line1 = config.MOTD_line1
Pierog.MOTD_line2 = config.MOTD_line2
Pierog.ConnectionError = config.ConnectionError
Pierog.BroadcastPierogErrors = config.BroadcastPierogErrors
Pierog.BroadcastErrorEachSeconds = config.BroadcastErrorEachSeconds or 600

Pierog.keepAliveSeconds              = config.keepAliveSeconds
Pierog.bigUpdatesSeconds             = config.bigUpdatesSeconds
Pierog.refreshSlotsSeconds           = config.refreshSlotsSeconds
Pierog.refreshMissionSeconds         = config.refreshMissionSeconds

net.log("[Pierog]", "Config loaded")

-- to avoid declaring every time a function is run
Pierog.Globals = {}
Pierog.Globals.version = "v0.12.1"
Pierog.Globals.Sides =  {
    [0] = 'SPECTATOR',
    [1] = 'RED',
    [2] = 'BLUE',
    [3] = 'NEUTRAL',	-- TBD check once this is released in DCS
}

net.log("[Pierog]", "Global variables initialised")

--net.log("[Pierog]", "Dupa")

Pierog.lastSentStatus = 0
Pierog.lastSentMission = 0
Pierog.lastSentKeepAlive = 0
Pierog.lastConnectionError = 0
Pierog.lastFrameStart = 0;
Pierog.lastTimer = 0
Pierog.lastFrameTime = 0;

Pierog.ReconnectTimeout = 30;
Pierog.RefreshKeepAlive = 5

--net.log("[Pierog]", "Dupa2")

Pierog.State = {}
Pierog.State.connected = false;
Pierog.State.slotsInitialised = false;

--net.log("[Pierog]", "Dupa3")

Pierog.Mission = {}
Pierog.Mission.Hash = {}
Pierog.Mission.Slots = {}
Pierog.Mission.Players = {}
Pierog.Mission.Units = {}

net.log("[Pierog]", "Initialising handlers")

Pierog.HandlerArgCounts = {}

Pierog.HandlerArgCounts["self_kill"] = 1
Pierog.HandlerArgCounts["crash"] = 2
Pierog.HandlerArgCounts["eject"] = 2
Pierog.HandlerArgCounts["connect"] = 2
Pierog.HandlerArgCounts["mission_end"] = 2
Pierog.HandlerArgCounts["pilot_death"] = 2
Pierog.HandlerArgCounts["takeoff"] = 3
Pierog.HandlerArgCounts["landing"] = 3
Pierog.HandlerArgCounts["change_slot"] = 3
Pierog.HandlerArgCounts["friendly_fire"] = 3
Pierog.HandlerArgCounts["disconnect"] = 4
Pierog.HandlerArgCounts["kill"] = 7

Pierog.Stats = {}
Pierog.Stats.LastSentTCP = 0
Pierog.Stats.LastSentSlots = 0
Pierog.Stats.LastBigUpdate = 0
Pierog.Stats.LastSentMission = 0
Pierog.Stats.LastConnectionError = 0

Pierog.func = {};
Pierog.func.ensureSlots = function()
    if(not Pierog.State.slotsInitialised) then

        local count = 0;
        local coalitions = DCS.getAvailableCoalitions();
        for name, _ in pairs(coalitions) do
            local slots = DCS.getAvailableSlots(name);
            Pierog.Mission.Slots[name] = {}
            for k, unit in pairs(slots) do
                Pierog.Mission.Slots[name][unit["unitId"]] = unit
                count = count + 1;
            end
        end

        if(count > 0) then
            net.log("[Pierog]", "initialised ".. count .. " slots")
            Pierog.State.slotsInitialised = true;
        end
    end
end

Pierog.func.ensureUnitId = function(unitId)
    if(not Pierog.Mission.Units[unitId]) then
        local unitType = DCS.getUnitType(unitId);
        Pierog.Mission.Units[unitId] = unitType;

        net.log("[Pierog]", unitId, "category:", DCS.getUnitTypeAttribute(unitType, "category"));
        net.log("[Pierog]", unitId, "category via type -> Prop:", DCS.getUnitProperty(unitType, DCS.UNIT_CATEGORY));
        net.log("[Pierog]", unitId, "category via id -> Prop:", DCS.getUnitProperty(unitId, DCS.UNIT_CATEGORY));
        net.log("[Pierog]", unitId, "wing span:", DCS.getUnitTypeAttribute(unitType, "WingSpan"));
        net.log("[Pierog]", unitId, "deck level", DCS.getUnitTypeAttribute(unitType, "DeckLevel"));
    --
        local payload = Pierog.Handlers["event"]("unit_definition");
        payload["unitId"] = unitId;
        payload["unitType"] = unitType;
        payload["unitCategory"] = DCS.getUnitTypeAttribute(unitId, "category");
        payload["wingSpan"] = DCS.getUnitTypeAttribute(unitType, "WingSpan");
        payload["deckLevel"] = DCS.getUnitTypeAttribute(unitType, "DeckLevel");
    --
        Pierog.func.sendData(net.lua2json(payload))
    end
end

Pierog.func.sendData = function(payload)

    local now = DCS.getRealTime()

    local connected, reconnected = Pierog.DLL.tcpSend(payload)
    --net.log("[Pierog]", connected, reconnected)
    if(connected < 1) and (now > Pierog.Stats.LastConnectionError + Pierog.ReconnectTimeout) then
        Pierog.AddLog("ERROR - TCP connection not available",0)

        if Pierog.BroadcastPierogErrors > 0 then
            local _all_players = net.get_player_list()
            for i, playerId in ipairs(_all_players) do
                net.send_chat_to(Pierog.ConnectionError , playerId)
            end
        end

        Pierog.Stats.LastConnectionError = now
    else
        Pierog.Stats.LastSentTCP = now

        if(reconnected > 0) then
            net.log("[Pierog]", "TCP reconnected")
            Pierog.Stats.LastBigUpdate = 0
            Pierog.Stats.LastSentSlots = 0
            Pierog.Stats.LastSentMission = 0
        end
    end
end

Pierog.func.sendMissionUpdate = function()
    local payload = Pierog.Handlers["event"]("mission")
    payload["mission"] = DCS.getCurrentMission()["mission"]
    payload["missionName"] = DCS.getCurrentMission()

    local json = net.lua2json(payload);
    Pierog.func.sendData(json)
end

Pierog.func.sendSlotUpdate = function()
    local payload = Pierog.Handlers["event"]("slots")
    payload["slots"] = Pierog.Mission.Slots;

    local json = net.lua2json(payload)
    Pierog.func.sendData(json)
end

Pierog.Handlers = {}
Pierog.Handlers["event"] = function(eventName)
    local data = {};
    data["event"] = eventName;
    data["missionHash"] = Pierog.Mission.Hash
    data['datetime']=os.date('%Y-%m-%d %H:%M:%S')

    return data;
end

Pierog.Handlers["friendly_fire"] = function(playerID, weaponName, victimPlayerID)
    local data = Pierog.Handlers["event"]("friendly_fire")

    data["killer"] = playerID;
    data["victim"] = victimPlayerID;
    data["weapon"] = weaponName;

    return data;
end

Pierog.Handlers["mission_end"] = function(winner, msg)
    local data = Pierog.Handlers["event"]("mission_end")

    data["winner"] = winner;
    data["message"] = msg;

    return data;
end

Pierog.Handlers["kill"] = function(killerPlayerID, killerUnitType, killerSide, victimPlayerID, victimUnitType, victimSide, weaponName)
    local data = Pierog.Handlers["event"]("kill")

    data["killer"] = {id = killerPlayerID, side = killerSide, type = killerUnitType, ucid = net.get_player_info(killerPlayerID)["ucid"]}
    data["victim"] = {id = victimPlayerID, side = victimSide, type = victimUnitType, ucid = net.get_player_info(victimPlayerID)["ucid"]}
    data["weapon"] = weaponName;

    return data;
end

Pierog.Handlers["self_kill"] = function(playerID)
    local data = Pierog.Handlers["event"]("self_kill")
    data["who"] = playerID;

    return data;
end

Pierog.Handlers["change_slot"] = function(playerID, slotID, prevSide)
    local newDetails = net.get_player_info(playerID);
    local oldDetails = Pierog.Mission.Players[playerID];

    Pierog.Mission.Players[playerID] = newDetails;

    local data = Pierog.Handlers["event"]("change_slot")
    data["previousSlot"] = oldDetails["slot"]
    data["previousSide"] = oldDetails["side"]
    data["side"] = newDetails["side"]
    data["slot"] = newDetails["slot"]
    data["ucid"] = newDetails["ucid"]

    return data;
end

Pierog.Handlers["connect"] = function(playerID, name)
    net.log("[Pierog]", "Getting player details")
    local details = net.get_player_info(playerID)
    Pierog.Mission.Players[playerID] = details

    local data = Pierog.Handlers["event"]("connect")
    data["who"] = playerID
    data["details"] = details;

    return data;
end

Pierog.Handlers["disconnect"] = function(playerID, name, playerSide, reason_code)
    local data = Pierog.Handlers["event"]("disconnect")
    data["who"] = Pierog.Mission.Players[playerID]
    data["reasonCode"] = reason_code;

    Pierog.Mission.Players[playerID] = nil

    return data;
end
--"crash", playerID, unit_missionID
Pierog.Handlers["crash"] = function(playerID, unit_missionID)
    Pierog.func.ensureUnitId(unit_missionID)

    local data = Pierog.Handlers["event"]("crash")
    data["who"] = playerID;
    data["unitId"] = unit_missionID;

    return data;
end
--"eject", playerID, unit_missionID
Pierog.Handlers["eject"] = function(playerID, unit_missionID)
    Pierog.func.ensureUnitId(unit_missionID)

    local data = Pierog.Handlers["event"]("eject")
    data["who"] = playerID;
    data["unitId"] = unit_missionID;

    return data;
end
--"takeoff", playerID, unit_missionID, airdromeName
Pierog.Handlers["takeoff"] = function(playerID, unit_missionID, airdromeName)
    Pierog.func.ensureUnitId(unit_missionID)

    local data = Pierog.Handlers["event"]("takeoff")
    data["who"] = playerID;
    data["unitId"] = unit_missionID;
    data["from"] = airdromeName;

    return data;
end
--"landing", playerID, unit_missionID, airdromeName
Pierog.Handlers["landing"] = function(playerID, unit_missionID, airdromeName)
    Pierog.func.ensureUnitId(unit_missionID)

    local data = Pierog.Handlers["event"]("landing")
    data["who"] = playerID;
    data["unitId"] = unit_missionID;
    data["at"] = airdromeName;

    return data;
end
--"pilot_death", playerID, unit_missionID
Pierog.Handlers["pilot_death"] = function(playerID, unit_missionID)
    Pierog.func.ensureUnitId(unit_missionID)

    local data = Pierog.Handlers["event"]("pilot_death")
    data["who"] = playerID;
    data["unitId"] = unit_missionID;

    return data;
end

net.log("[Pierog] Handlers defined")

--- ################################ Helper function definitions ################################

Pierog.AddLog = function(text, logLevel)
    -- Adds logs to DCS.log file
    if(logLevel == nil) then
        logLevel = -1
    end

    if Pierog.DebugMode >= logLevel then
        net.log("[Pierog]", text)
    end
end

Pierog.generate_mission_hash = function()
    -- Generates unique simulation mission hash
    return DCS.getMissionName( ).."@".. Pierog.Instance .. "@" .. Pierog.Globals.version .. "@".. os.date('%Y%m%d_%H%M%S');
end

--- ################################ Log functions ################################

Pierog.LogChat = function(playerID, msg, all)

    local data = Pierog.Handlers["event"]("chat");

    data['who']= Pierog.Mission.Players[playerID]["ucid"]
    data['msg']=msg
    data['messageToAll']=all
    data['missionhash']=Pierog.Mission.Hash

    Pierog.func.sendData(net.lua2json(data));
end

Pierog.LogEvent = function(log_type, log_content, log_arg_1, log_arg_2)
    -- Logs events messages
    local data = Pierog.Handlers["event"]("customLog");
    data['log_type']= log_type
    data['log_arg_1']= log_arg_1
    data['log_arg_2']= log_arg_2
    data['log_content']=log_content
    data['log_missionhash']=Pierog.Mission.Hash

    Pierog.AddLog("Sending event data, event: " .. log_type .. ", arg1:" .. log_arg_1 .. ", arg2:" .. log_arg_2 .. ", content: " .. log_content,1)

    Pierog.func.sendData(net.lua2json(data));
end

--- ################################ Event callbacks ################################

Pierog.onSimulationStart = function()
    Pierog.Mission.Hash=Pierog.generate_mission_hash()
    Pierog.LogEvent("SimStart","Mission " .. Pierog.Mission.Hash .. " started",nil,nil);
    Pierog.lastSentMission = 0 -- reset so mission information will be sent
    Pierog.State.slotsInitialised = false;

    Pierog.DLL.markMissionStart(Pierog.Mission.Hash);
end

Pierog.onSimulationStop = function()
    Pierog.LogEvent("SimStop","Mission " .. Pierog.Mission.Hash .. " finished",nil,nil);

    Pierog.Mission = {}
    Pierog.Mission.Hash=Pierog.generate_mission_hash();
    Pierog.Mission.Slots = {}
    Pierog.Mission.Players = {}
    Pierog.Mission.Units = {}

    Pierog.Stats.LastSentSlots = 0
    Pierog.Stats.LastBigUpdate = 0
    Pierog.Stats.LastSentMission = 0
    Pierog.DLL.markMissionStart(Pierog.Mission.Hash);
end

Pierog.onPlayerStop = function (id)
    -- Player left the simulation (happens right before a disconnect, if player exited by desire)

    Pierog.LogEvent("quit", "Player " .. id .. " quit the server.",nil,nil);
    return
end

Pierog.onSimulationFrame = function()

    if(DCS.getMissionName() == "") then
        -- no mission loaded
        return;
    end

    local now = DCS.getRealTime()

    if(Pierog.Stats.LastBigUpdate < 1 or now > Pierog.Stats.LastBigUpdate + Pierog.bigUpdatesSeconds) then
        --net.log("[Pierog]", "Big update evaluation at", now, ", Pierog.Stats.LastSentSlots", Pierog.Stats.LastSentSlots, ", Pierog.Stats.LastBigUpdate", Pierog.Stats.LastBigUpdate)
        --net.log("[Pierog]", "Pierog.refreshSlotsSeconds: ".. Pierog.refreshSlotsSeconds)
        --net.log("[Pierog]", "name: >"..DCS.getMissionName().."<");
        --net.log("[Pierog]", "mission"..net.lua2json(DCS.getCurrentMission()))

        if(not Pierog.State.slotsInitialised) then
            local foo = Pierog.func.ensureSlots;

            local status, error = pcall(foo);
            if(error) then
                Pierog.AddLog( "error getting slot information" .. error);
            else
                Pierog.func.sendSlotUpdate();
                Pierog.Stats.LastSentSlots = now;
                Pierog.Stats.LastBigUpdate = now;
            end
        elseif(now > Pierog.Stats.LastSentSlots + Pierog.refreshSlotsSeconds) then
            --net.log("[Pierog]", "Slots")

            Pierog.func.sendSlotUpdate();
            Pierog.Stats.LastSentSlots = now;
            Pierog.Stats.LastBigUpdate = now;
        end

        if (now > Pierog.Stats.LastSentMission) then
            --net.log("[Pierog]", "Mission")

            Pierog.func.sendMissionUpdate()
            Pierog.Stats.LastSentMission = now
            Pierog.Stats.LastBigUpdate = now
        end
    end

    if(now > Pierog.LastSentTCP + Pierog.RefreshKeepAlive) then
        Pierog.func.sendData(" ")
    end
end

Pierog.onPlayerStart = function (playerId)
    -- Player entered cocpit
    net.send_chat_to(Pierog.MOTD_line1, playerId);
    net.send_chat_to(Pierog.MOTD_line2, playerId);
end

Pierog.onPlayerTrySendChat = function (playerID, msg, all)
    -- Somebody tries to send chat message
    if (msg == Pierog.ConnectionError or msg == Pierog.MOTD_line1 or msg == Pierog.MOTD_line2) then
        return msg;
    end

    Pierog.LogChat(playerID, msg, all)
    return msg;
end

Pierog.onGameEvent = function(eventName, arg1, arg2, arg3, arg4, arg5, arg6, arg7)
    local argCount = Pierog.HandlerArgCounts[eventName]
    local handler = Pierog.Handlers[eventName]

    if(Pierog.Handlers[eventName] == nil) then
        --    no handler found
        net.log("[Pierog]", 'Event type not found'..eventName)
        return;
    end

    local payload = {}
    if(argCount == 1) then
        payload = handler(arg1)
    elseif argCount == 2 then
        payload = handler(arg1, arg2)
    elseif argCount == 3 then
        payload = handler(arg1, arg2, arg3)
    elseif argCount == 4 then
        payload = handler(arg1, arg2, arg3, arg4)
    elseif argCount == 7 then
        payload = handler(arg1, arg2, arg3, arg4, arg5, arg6, arg7)
    end

    local json = net.lua2json(payload)
    Pierog.func.sendData(json)

    if(eventName == "mission_end") then
        Pierog.DLL.markMissionStart(Pierog.Mission.Hash);
    end

    --net.log("[Pierog] json", json)

    --net.log("[Pierog]", "get_player_list", net.lua2json(net.get_player_list()))
    --net.log("[Pierog]", "mission_data", net.lua2json(DCS.getCurrentMission()))

    --net.log("[Pierog]", "DCS.getAvailableCoalitions()", net.lua2json(DCS.getAvailableCoalitions()))
    --net.log("[Pierog]", "DCS.getAvailableSlots(0)", DCS.getAvailableSlots(0))
    --net.log("[Pierog]", "DCS.getAvailableSlots(3)", DCS.getAvailableSlots(3))
    --net.log("[Pierog]", "DCS.getMissionName()", DCS.getMissionName())
    --net.log("[Pierog]", "DCS.getModelTime()", DCS.getModelTime())
    --net.log("[Pierog]", "DCS.getRealTime()", DCS.getRealTime())
end

-- ########### Finalize and set callbacks ###########
if DCS.isServer() then
    -- If this game instance is hosting multiplayer game, start Pierog
    net.log("[Pierog]", "In a server, setting DLL", Pierog.logPath, Pierog.host, Pierog.port)
    Pierog.DLL.delimiters(Pierog.delimiters[1], Pierog.delimiters[2])
    Pierog.DLL.StartOfApp(Pierog.logPath, Pierog.host, Pierog.port);
    net.log("[Pierog]", "DLL started")
    Pierog.Mission.Hash=Pierog.generate_mission_hash();													-- Generate initial missionhash
    DCS.setUserCallbacks(Pierog);																		-- Set user callbacs,  map DCS event handlers with functions defined above
    Pierog.AddLog("Loaded - Pierog for DCS World - version: " .. Pierog.Globals.version,0)		    -- Display Pierog information in log
end


