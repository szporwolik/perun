-- Perun for DCS World https://github.com/szporwolik/perun -> DCS Hook component
net.log("[Perun] Loading Perun")

-- Initial init
local Perun = {}

-- Load Luas
package.path  = package.path..";"..lfs.currentdir().."/LuaSocket/?.lua"..";"..lfs.writedir() .. "/Mods/services/Perun/lua/?.lua"
package.cpath = package.cpath..";"..lfs.currentdir().."/LuaSocket/?.dll"

-- Load Dlls
package.cpath = package.cpath..';'.. lfs.writedir()..'/Mods/services/Perun/bin/' ..'?.dll;'
net.log("[Perun]", "Loading DLL")
Perun.DLL = require('perun')
net.log("[Perun]", "Loaded DLL, starting")
Perun.DLL.StartOfApp();
net.log("[Perun]", "DLL started")

-- Load config file
net.log("[Perun]", "Loading config")
local config = require "perun_config"
net.log("[Perun]", "Config loaded")

Perun.host = config.host
Perun.port = config.port
Perun.Instance = config.Instance
Perun.DebugMode = config.DebugMode

Perun.MOTD_line1 = config.MOTD_line1
Perun.MOTD_line2 = config.MOTD_line2
Perun.ConnectionError = config.ConnectionError
Perun.BroadcastPerunErrors = config.BroadcastPerunErrors

Perun.keepAliveSeconds              = config.keepAliveSeconds
Perun.bigUpdatesSeconds             = config.bigUpdatesSeconds
Perun.refreshSlotsSeconds           = config.refreshSlotsSeconds
Perun.refreshMissionSeconds         = config.refreshMissionSeconds

Perun.Version = "v0.12.1"

Perun.MissionHash=""

Perun.lastSentStatus = 0
Perun.lastSentMission = 0
Perun.lastSentKeepAlive = 0
Perun.lastConnectionError = 0
Perun.lastFrameStart = 0;
Perun.lastTimer = 0
Perun.lastFrameTime = 0;

Perun.ReconnectTimeout = 30;
Perun.RefreshKeepAlive = 5

Perun.State = {}
Perun.State.connected = false;
Perun.State.slotsInitialised = false;

-- to avoid declaring every time a function is run
Perun.Globals = {}
Perun.Globals.Sides =  {
    [0] = 'SPECTATOR',
    [1] = 'RED',
    [2] = 'BLUE',
    [3] = 'NEUTRAL',	-- TBD check once this is released in DCS
}

Perun.Mission = {}
Perun.Mission.Slots = {}
Perun.Mission.Slots["blue"] = {}
Perun.Mission.Slots["red"] = {}
Perun.Mission.Players = {}
Perun.Mission.Units = {}

Perun.HandlerArgCounts = {}

Perun.HandlerArgCounts["self_kill"] = 1
Perun.HandlerArgCounts["crash"] = 2
Perun.HandlerArgCounts["eject"] = 2
Perun.HandlerArgCounts["connect"] = 2
Perun.HandlerArgCounts["mission_end"] = 2
Perun.HandlerArgCounts["pilot_death"] = 2
Perun.HandlerArgCounts["takeoff"] = 3
Perun.HandlerArgCounts["landing"] = 3
Perun.HandlerArgCounts["change_slot"] = 3
Perun.HandlerArgCounts["friendly_fire"] = 3
Perun.HandlerArgCounts["disconnect"] = 4
Perun.HandlerArgCounts["kill"] = 7

net.log("[Perun]", "Global variables initialised")

Perun.Stats = {}
Perun.Stats.LastSentTCP = 0
Perun.Stats.LastSentSlots = 0
Perun.Stats.LastBigUpdate = 0
Perun.Stats.LastSentMission = 0
Perun.Stats.PreviousFrameStart = 0
Perun.Stats.LastConnectionError = 0

Perun.func = {};
Perun.func.ensureSlots = function()
    if(not Perun.State.slotsInitialised) then
        net.log("Perun", "initialising slots")

        local blue = DCS.getAvailableSlots("blue");
        for k, unit in pairs(blue) do
            Perun.Mission.Slots["blue"][unit["unitId"]] = unit
        end

        local red = DCS.getAvailableSlots("red");
        for k, unit in pairs(red) do
            Perun.Mission.Slots["red"][unit["unitId"]] = unit
        end

        Perun.State.slotsInitialised = true;
    end
end

Perun.func.ensureUnitId = function(unitId)
    if(not Perun.Mission.Units[unitId]) then
        local unitData = DCS.getUnitType(unitId)
        Perun.Mission.Units[unitId] = unitData
        local payload = {}
        payload["unit"] = unitData

        Perun.func.sendData(net.lua2json(unitData))
    end
end

Perun.func.sendData = function(payload)

    local now = DCS.getRealTime()

    local connected, reconnected = Perun.DLL.tcpSend(payload)
    net.log("[Perun]", connected, reconnected)
    if(connected < 1) and (now > Perun.Stats.LastConnectionError + Perun.ReconnectTimeout) then
        Perun.AddLog("ERROR - TCP connection is not available",0)

        if Perun.BroadcastPerunErrors > 0 then
            local _all_players = net.get_player_list()
            for i, playerId in ipairs(_all_players) do
                net.send_chat_to(Perun.ConnectionError , playerId)
            end
        end

        Perun.Stats.LastConnectionError = now
    else
        Perun.Stats.LastSentTCP = now

        if(reconnected > 0) then
            net.log("Perun", "TCP reconnected")
            Perun.Stats.LastBigUpdate = 0
            Perun.Stats.LastSentSlots = 0
            Perun.Stats.LastSentMission = 0
        end
    end
end

Perun.Handlers = {}
Perun.Handlers["event"] = function(eventName)
    local data = {};
    data["event"] = eventName;
    data["hash"] = Perun.MissionHash
    data['datetime']=os.date('%Y-%m-%d %H:%M:%S')

    return data;
end

Perun.Handlers["friendly_fire"] = function(playerID, weaponName, victimPlayerID)
    local data = Perun.Handlers["event"]("friendly_fire")

    data["killer"] = playerID;
    data["victim"] = victimPlayerID;
    data["weapon"] = weaponName;

    return data;
end

Perun.Handlers["mission_end"] = function(winner, msg)
    local data = Perun.Handlers["event"]("mission_end")

    data["winner"] = winner;
    data["message"] = msg;

    return data;
end

Perun.Handlers["kill"] = function(killerPlayerID, killerUnitType, killerSide, victimPlayerID, victimUnitType, victimSide, weaponName)
    local data = Perun.Handlers["event"]("kill")

    data["killer"] = {killerPlayerID, killerUnitType, killerSide}
    data["victim"] = {victimPlayerID, victimUnitType, victimSide}
    data["weapon"] = weaponName;

    return data;
end

Perun.Handlers["self_kill"] = function(playerID)
    local data = Perun.Handlers["event"]("self_kill")
    data["who"] = playerID;

    return data;
end

Perun.Handlers["change_slot"] = function(playerID, slotID, prevSide)
    local details = net.get_player_info(playerID, "side")
    Perun.Mission.Players[playerID]["side"] = details

    local data = Perun.Handlers["event"]("change_slot")
    data["previousSlot"] = slotID
    data["previousSide"] = prevSide

    return data;
end

Perun.Handlers["connect"] = function(playerID, name)
    net.log("Perun", "Getting player details")
    local details = net.get_player_info(playerID)
    Perun.Mission.Players[playerID] = details

    net.log("Perun", "Preparing event")
    local data = Perun.Handlers["event"]("connect")
    data["who"] = playerID
    data["details"] = details;

    return data;
end

Perun.Handlers["disconnect"] = function(playerID, name, playerSide, reason_code)
    local data = Perun.Handlers["event"]("disconnect")
    data["who"] = playerID;
    data["details"] = {name, playerSide, reason_code}

    Perun.Mission.Players[playerID] = nil

    return data;
end
--"crash", playerID, unit_missionID
Perun.Handlers["crash"] = function(playerID, unit_missionID)
    Perun.func.ensureUnitId(unit_missionID)

    local data = Perun.Handlers["event"]("crash")
    data["who"] = playerID;
    data["unitId"] = unit_missionID;

    return data;
end
--"eject", playerID, unit_missionID
Perun.Handlers["eject"] = function(playerID, unit_missionID)
    Perun.func.ensureUnitId(unit_missionID)

    local data = Perun.Handlers["event"]("eject")
    data["who"] = playerID;
    data["unitId"] = unit_missionID;

    return data;
end
--"takeoff", playerID, unit_missionID, airdromeName
Perun.Handlers["takeoff"] = function(playerID, unit_missionID, airdromeName)
    Perun.func.ensureUnitId(unit_missionID)

    local data = Perun.Handlers["event"]("takeoff")
    data["who"] = playerID;
    data["unitId"] = unit_missionID;
    data["from"] = airdromeName;

    return data;
end
--"landing", playerID, unit_missionID, airdromeName
Perun.Handlers["landing"] = function(playerID, unit_missionID, airdromeName)
    Perun.func.ensureUnitId(unit_missionID)

    local data = Perun.Handlers["event"]("landing")
    data["who"] = playerID;
    data["unitId"] = unit_missionID;
    data["at"] = airdromeName;

    return data;
end
--"pilot_death", playerID, unit_missionID
Perun.Handlers["pilot_death"] = function(playerID, unit_missionID)
    Perun.func.ensureUnitId(unit_missionID)

    local data = Perun.Handlers["event"]("pilot_death")
    data["who"] = playerID;
    data["unitId"] = unit_missionID;

    return data;
end

net.log("[Perun] Handlers defined")

--- ################################ Helper function definitions ################################

Perun.AddLog = function(text, LogLevel)
    -- Adds logs to DCS.log file
    if Perun.DebugMode >= LogLevel then
        net.log("[Perun]", text)
    end
end

Perun.generate_mission_hash = function()
    -- Generates unique simulation mission hash
    return DCS.getMissionName( ).."@".. Perun.Instance .. "@" .. Perun.Version .. "@".. os.date('%Y%m%d_%H%M%S');
end

--- ################################ TCP Connection ################################

Perun.ConnectToPerun = function ()

    Perun.AddLog(string.format("Connecting to TCP server %s:%i", Perun.host, Perun.port), 0);

    local status = Perun.DLL.tcpConnect(Perun.host, Perun.port);

    if(status == 0) then
        Perun.AddLog(string.format("Connection failed: %s:%i", Perun.host, Perun.port), 0);
    else
        Perun.State.connected = true;
    end
end

--- ################################ Log functions ################################

Perun.LogChat = function(playerID, msg, all)

    local data = Perun.Handlers["event"]("chat");

    data['player']= Perun.Mission.Players[playerID]
    data['msg']=msg
    data['messageToAll']=all
    data['missionhash']=Perun.MissionHash

    Perun.func.sendData(net.lua2json(data));
end

Perun.LogEvent = function(log_type, log_content, log_arg_1, log_arg_2)
    -- Logs events messages
    local data = Perun.Handlers["event"]("customLog");
    data['log_type']= log_type
    data['log_arg_1']= log_arg_1
    data['log_arg_2']= log_arg_2
    data['log_content']=log_content
    data['log_missionhash']=Perun.MissionHash

    Perun.AddLog("Sending event data, event: " .. log_type .. ", arg1:" .. log_arg_1 .. ", arg2:" .. log_arg_2 .. ", content: " .. log_content,1)

    Perun.func.sendData(net.lua2json(data));
end

--- ################################ Event callbacks ################################

Perun.onSimulationStart = function()
    -- Simulation was started
    Perun.MissionHash=Perun.generate_mission_hash()
    Perun.LogEvent("SimStart","Mission " .. Perun.MissionHash .. " started",nil,nil);
    Perun.lastSentMission = 0 -- reset so mission information will be send
end

Perun.onSimulationStop = function()
    -- Simulation was stopped
    Perun.LogEvent("SimStop","Mission " .. Perun.MissionHash .. " finished",nil,nil);
    Perun.MissionHash=Perun.generate_mission_hash();

    Perun.Mission = {}
    Perun.Mission.Slots = {}
    Perun.Mission.Slots["blue"] = {}
    Perun.Mission.Slots["red"] = {}
    Perun.Mission.Players = {}
    Perun.Mission.Units = {}
end

Perun.onPlayerDisconnect = function(id, err_code)
    -- Player disconnected
    Perun.LogEvent("disconnect", "Player " .. id .. " disconnected.",nil,nil);

    return
end

Perun.onPlayerStop = function (id)
    -- Player left the simulation (happens right before a disconnect, if player exited by desire)


    Perun.LogEvent("quit", "Player " .. id .. " quit the server.",nil,nil);
    return
end

Perun.onSimulationFrame = function()

    if(not Perun.State.slotsInitialised) then
        local foo = Perun.func.ensureSlots;

        local status, error = pcall(foo);
        if(status) then
            Perun.AddLog( "slot information loaded")
        else
            Perun.AddLog( "error getting slot information", error)
        end
    end

    local now = DCS.getRealTime()
    --net.log("Perun", "big updates check, now", now, "last sent slots", Perun.Stats.LastSentSlots, "last sent mission", Perun.Stats.LastSentMission)

    if(now - Perun.Stats.LastBigUpdate > Perun.bigUpdatesSeconds) then
        if(now > Perun.Stats.LastSentSlots + Perun.refreshSlotsSeconds) then
            local payload = Perun.Handlers["event"]("slots")
            payload["slots"] = Perun.Mission.Slots;

            local json = net.lua2json(payload)
            --net.log("Perun, slots", json)
            Perun.func.sendData(json)
            Perun.Stats.LastSentSlots = now
            Perun.Stats.LastBigUpdate = now
        elseif (now > Perun.Stats.LastSentMission + Perun.refreshMissionSeconds) then
            local payload = Perun.Handlers["event"]("mission")
            payload["mission"] = DCS.getCurrentMission()["mission"]

            local json = net.lua2json(payload);
            --net.log("Perun, mission", json)
            Perun.func.sendData(json)
            Perun.Stats.LastSentMission = now
            Perun.Stats.LastBigUpdate = now
        end
    end

    if(now > Perun.LastSentTCP + Perun.RefreshKeepAlive) then
        Perun.func.sendData(" ")
    end

    Perun.Stats.PreviousFrameStart = DCS.getRealTime()
end

Perun.onPlayerStart = function (playerId)
    -- Player entered cocpit

    net.send_chat_to(Perun.MOTD_line1, playerId);
    net.send_chat_to(Perun.MOTD_line2, playerId);

end

Perun.onPlayerTrySendChat = function (playerID, msg, all)
    -- Somebody tries to send chat message
    if (msg == Perun.ConnectionError or msg == Perun.MOTD_line1 or msg == Perun.MOTD_line2) then
        return msg;
    end

    Perun.LogChat(playerID,msg,all)
    return msg;
end

Perun.onGameEvent = function (eventName,arg1,arg2,arg3,arg4,arg5,arg6,arg7)
    local argCount = Perun.HandlerArgCounts[eventName]
    local handler = Perun.Handlers[eventName]

    local payload = {}
    if(argCount == 1) then
        payload = handler(arg1)
    elseif argCount == 2 then
        payload = handler(arg1, arg2)
    elseif argCount == 3 then
        payload = handler(arg1, arg2, arg3)
    elseif argCount == 4 then
        payload = handler(arg1, arg2, arg3, arg4)
    else
        payload = handler(arg1, arg2, arg3, arg4, arg5, arg6, arg7)
    end

    local json = net.lua2json(payload)
    Perun.func.sendData(json)
end

-- ########### Finalize and set callbacks ###########
if DCS.isServer() then
    -- If this game instance is hosting multiplayer game, start Perun
    Perun.DLL.StartOfApp();												                                -- Perun dll StartOfApp callback
    Perun.MissionHash=Perun.generate_mission_hash();													-- Generate initial missionhash
    DCS.setUserCallbacks(Perun);																		-- Set user callbacs,  map DCS event handlers with functions defined above
    Perun.ConnectToPerun();																			    -- Connect to Perun server
    Perun.AddLog("Loaded - Perun for DCS World - version: " .. Perun.Version,0)		    -- Display perun information in log
end


