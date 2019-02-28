-- Perun for DCS World https://github.com/szporwolik/perun -> DCS Hook component

-- Initial init
local Perun = {}
package.path  = package.path..";"..lfs.currentdir().."/LuaSocket/?.lua"
package.cpath = package.cpath..";"..lfs.currentdir().."/LuaSocket/?.dll"

-- ########### SETTINGS ###########

Perun.RefreshStatus = 15 												-- base refresh rate in seconds to send status update (values lower than 60 may affect performance!)
Perun.RefreshMission = 60 												-- refresh rate in seconds to send mission information  (values lower than 60 may affect performance!)
Perun.JsonStatusLocation = "Scripts\\Json\\" 							-- folder relative do user's SaveGames DCS folder -> status file updated each RefreshMission
Perun.UDPTargetPort = 48620												-- UDP port to send data to
Perun.MOTD_L1 = "Witamy na serwerze Gildia.org !"						-- Message send to players connecting the server - Line 1
Perun.MOTD_L2 = "Wymagamy obecnosci na 255Mhz (DCS SRS)"				-- Message send to players connecting the server - Line 2

-- ########### END OF SETTINGS ###########


-- Variable init
Perun.Version = "v0.3.7"
Perun.StatusData = {}
Perun.SlotsData = {}
Perun.MissionData = {}
Perun.ServerData = {}
Perun.MissionHash=""
Perun.lastSentStatus =0
Perun.lastSentMission =0
Perun.JsonStatusLocation = lfs.writedir() .. Perun.JsonStatusLocation
Perun.socket  = require("socket")

Perun.UDP = assert(Perun.socket.udp())
Perun.UDP:settimeout(0)
Perun.UDP:setsockname("*", 48621)
Perun.UDP:setpeername("127.0.0.1",Perun.UDPTargetPort)

-- ########### Helper function definitions ###########
function stripChars(str)
    -- remove accents characters from string
    -- via https://stackoverflow.com/questions/50459102/replace-accented-characters-in-string-to-standard-with-lua TBD: rewrite
    tableAccents = {}
    tableAccents["à"] = "a"
    tableAccents["á"] = "a"
    tableAccents["â"] = "a"
    tableAccents["ã"] = "a"
    tableAccents["ä"] = "a"
    tableAccents["ç"] = "c"
    tableAccents["è"] = "e"
    tableAccents["é"] = "e"
    tableAccents["ê"] = "e"
    tableAccents["ë"] = "e"
    tableAccents["ì"] = "i"
    tableAccents["í"] = "i"
    tableAccents["î"] = "i"
    tableAccents["ï"] = "i"
    tableAccents["ñ"] = "n"
    tableAccents["ò"] = "o"
    tableAccents["ó"] = "o"
    tableAccents["ô"] = "o"
    tableAccents["õ"] = "o"
    tableAccents["ö"] = "o"
    tableAccents["ù"] = "u"
    tableAccents["ú"] = "u"
    tableAccents["û"] = "u"
    tableAccents["ü"] = "u"
    tableAccents["ý"] = "y"
    tableAccents["ÿ"] = "y"
    tableAccents["À"] = "A"
    tableAccents["Á"] = "A"
    tableAccents["Â"] = "A"
    tableAccents["Ã"] = "A"
    tableAccents["Ä"] = "A"
    tableAccents["Ç"] = "C"
    tableAccents["È"] = "E"
    tableAccents["É"] = "E"
    tableAccents["Ê"] = "E"
    tableAccents["Ë"] = "E"
    tableAccents["Ì"] = "I"
    tableAccents["Í"] = "I"
    tableAccents["Î"] = "I"
    tableAccents["Ï"] = "I"
    tableAccents["Ñ"] = "N"
    tableAccents["Ò"] = "O"
    tableAccents["Ó"] = "O"
    tableAccents["Ô"] = "O"
    tableAccents["Õ"] = "O"
    tableAccents["Ö"] = "O"
    tableAccents["Ù"] = "U"
    tableAccents["Ú"] = "U"
    tableAccents["Û"] = "U"
    tableAccents["Ü"] = "U"
    tableAccents["Ý"] = "Y"

    -- Polish accents
    tableAccents["ę"] = "e"
    tableAccents["Ę"] = "Ę"
    tableAccents["ó"] = "o"
    tableAccents["Ó"] = "O"
    tableAccents["ą"] = "a"
    tableAccents["Ą"] = "A"
    tableAccents["ś"] = "s"
    tableAccents["Ś"] = "S"
    tableAccents["ć"] = "c"
    tableAccents["Ć"] = "C"
    tableAccents["ż"] = "z"
    tableAccents["Ż"] = "Z"
    tableAccents["ź"] = "z"
    tableAccents["Ź"] = "Z"
    tableAccents["ł"] = "l"
    tableAccents["Ł"] = "L"

    -- TBD additonal characters

    normalizedString = ''

    for strChar in string.gmatch(str, "([%z\1-\127\194-\244][\128-\191]*)") do
        if tableAccents[strChar] ~= nil then
            normalizedString = normalizedString..tableAccents[strChar]
        else
            normalizedString = normalizedString..strChar
        end
    end
    return normalizedString
end

Perun.GetCategory = function(id)
    -- Returns object category basing on ID
    -- Helper function via  https://pastebin.com/GUAXrd2U TBD: rewrite
    local _killed_target_category = DCS.getUnitTypeAttribute(id, "category")
    if _killed_target_category == nil then
        local _killed_target_cat_check_ship = DCS.getUnitTypeAttribute(id, "DeckLevel")
        local _killed_target_cat_check_plane = DCS.getUnitTypeAttribute(id, "WingSpan")
        if _killed_target_cat_check_ship ~= nil and _killed_target_cat_check_plane == nil then
            _killed_target_category = "Ships"
        elseif _killed_target_cat_check_ship == nil and _killed_target_cat_check_plane ~= nil then
            _killed_target_category = "Planes"
        else
            _killed_target_category = "Helicopters"
        end
    end
    return _killed_target_category
end

Perun.SideID2Name = function(id)
    -- Helper function returns side name per side (coalition) id
    local sides = {
        [0] = 'SPECTATOR',
        [1] = 'RED',
        [2] = 'BLUE',
    }
    return sides[id]
end

-- ########### Main code ###########
Perun.AddLog = function(text)
    -- Adds logs to DCS.log file
    net.log("Perun : ".. text)
end

Perun.UpdateJsonStatus = function()
    -- Updates status json file
    TempData={}
    TempData["1"]=Perun.ServerData
    TempData["2"]=Perun.StatusData
    TempData["3"]=Perun.SlotsData
    -- TempData["4"]=Perun.MissionData -- TBD: hangs for some large missions

    _temp=net.lua2json(TempData)

    perun_export = io.open(Perun.JsonStatusLocation .. "perun_status_data.json", "w")
    perun_export:write(_temp .. "\n")
    perun_export:close()

end

Perun.Send = function(data_id, data_package)
    -- Sends data package
    TempData={}
    TempData["type"]=data_id
    TempData["payload"]=data_package
    TempData["timestamp"]=os.date('%Y-%m-%d %H:%M:%S')

    temp=net.lua2json(TempData)
    temp=stripChars(temp)

    Perun.UDP:send(temp)
    Perun.AddLog("Packet send")
end

Perun.UpdateStatusPart = function(part_id, data_package)
    -- Helper for status update container
    Perun.StatusData[part_id] = data_package
end

Perun.UpdateStatus = function()
    -- Main function for status updates

    -- Diagnostic data
		-- Update version data
		Perun.ServerData['v_dcs_hook']=Perun.Version

		-- Update server data
		_table=net.get_player_list()
		_count = 0
		for _ in pairs(_table) do _count = _count + 1 end
		Perun.ServerData['c_players']=_count

		-- Send
		Perun.Send(1,Perun.ServerData)

    -- Status data - update all subsections
		-- 1 - Mission
		temp={}
		temp['name']=DCS.getMissionName()
		temp['modeltime']=DCS.getModelTime()
		temp['realtime']=DCS.getRealTime()
		temp['pause']=DCS.getPause()
		temp['multiplayer']=DCS.isMultiplayer()
		temp['theatre'] = Perun.MissionData['mission']['theatre']
		temp['weather'] = Perun.MissionData['mission']['weather']
		Perun.UpdateStatusPart("mission",temp)

		-- 2 - Players
		_temp = net.get_player_list()
		_temp2={}
		for k, i in ipairs(_temp) do
			_temp2[k]=net.get_player_info(i)
		end
		Perun.UpdateStatusPart("players",_temp2)

		-- Send
		Perun.Send(2,Perun.StatusData)

    -- Update slots data
		Perun.SlotsData['coalitions']=DCS.getAvailableCoalitions()
		Perun.SlotsData['slots']={}

		for j, i in pairs(Perun.SlotsData['coalitions']) do
			Perun.SlotsData['slots'][j]=DCS.getAvailableSlots(j)
		end
		-- Send
		Perun.Send(3,Perun.SlotsData)
end


Perun.UpdateMission = function()
    -- Main function for mission information updates
    Perun.MissionData=DCS.getCurrentMission()
end

Perun.LogChat = function(playerID,msg,all)
    -- Log chat messages

    data={}
    data['player']= net.get_player_info(playerID, "name")
    data['msg']=stripChars(msg)
    data['all']=all
    data['ucid']=net.get_player_info(playerID, 'ucid')
    data['datetime']=os.date('%Y-%m-%d %H:%M:%S')
    data['missionhash']=Perun.MissionHash

    Perun.Send(50,data)
end

Perun.LogEvent = function(log_type,log_content)
    -- Log chat messages

    data={}
    data['log_type']= log_type
    data['log_content']=log_content
    data['log_datetime']=os.date('%Y-%m-%d %H:%M:%S')
    data['log_missionhash']=Perun.MissionHash

    Perun.Send(51,data)
end

Perun.LogStats = function(playerID)
    -- Log player status

    -- TBD : not working at the moment WIP
    p_stats={}
    p_stats['PS_CAR']=net.get_stat(playerID,2)
    p_stats['PS_PLANE']=net.get_stat(playerID,3)
    p_stats['PS_SHIP']=net.get_stat(playerID,4)
    p_stats['PS_SCORE']=net.get_stat(playerID,5)
    p_stats['PS_LAND']=net.get_stat(playerID,6)
    p_stats['PS_PING']=net.get_stat(playerID,0)
    p_stats['PS_CRASH']=net.get_stat(playerID,1)
    p_stats['PS_EJECT']=net.get_stat(playerID,7)
    p_stats['debug']="ok"

    data={}
    data['stat_data']=p_stats
    data['stat_ucid']=net.get_player_info(playerID, 'ucid')
    data['stat_datetime']=os.date('%Y-%m-%d %H:%M:%S')
    data['stat_missionhash']=Perun.MissionHash

    Perun.Send(52,data)
end

Perun.LogLogin = function(playerID)
    -- Player logged in

    data={}
    data['login_ucid']=net.get_player_info(playerID, 'ucid')
    data['login_ipaddr']=net.get_player_info(playerID, 'ipaddr')
    data['login_name']=net.get_player_info(playerID, 'name')
    data['login_datetime']=os.date('%Y-%m-%d %H:%M:%S')

    Perun.Send(53,data)
end

--- ########### Event callbacks ###########

Perun.onSimulationStart = function()
    Perun.MissionHash=DCS.getMissionName( ).."@"..os.date('%Y%m%d_%H%M%S');
    Perun.LogEvent("SimStart","Mission " .. Perun.MissionHash .. " started");
end

Perun.onSimulationStop = function()
    Perun.LogEvent("SimStop","Mission " .. Perun.MissionHash .. " finished");
end

Perun.onPlayerDisconnect= function(id, err_code)
    Perun.LogEvent("disconnect", "Player " .. net.get_player_info(id, "name") .. " disconnected; " .. err_code);
end

Perun.onSimulationFrame = function()
    local _now = DCS.getRealTime()

    -- First run
    if Perun.lastSentMission ==0 and Perun.lastSentStatus ==0 then
        Perun.UpdateMission()
    end

    -- Send mission update and update JSON
    if _now > Perun.lastSentMission + Perun.RefreshMission then
        Perun.lastSentMission = _now

        Perun.UpdateMission()
        Perun.UpdateJsonStatus()
    end

    -- Send status update
    if _now > Perun.lastSentStatus + Perun.RefreshStatus then
        Perun.lastSentStatus = _now

        Perun.UpdateStatus()
    end



end


Perun.onPlayerStart = function (id)
    net.send_chat(Perun.MOTD_L1, id);
    net.send_chat(Perun.MOTD_L2, id);
end

Perun.onPlayerTrySendChat = function (playerID, msg, all)
    if msg~=Perun.MOTD_L1 and msg~=Perun.MOTD_L2 then
        Perun.LogChat(playerID,msg,all)
    end

    return msg
end

Perun.onGameEvent = function (eventName,arg1,arg2,arg3,arg4,arg5,arg6,arg7)

    if eventName == "friendly_fire" then
        --"friendly_fire", playerID, weaponName, victimPlayerID
        Perun.LogEvent(eventName,Perun.SideID2Name( net.get_player_info(arg1, "side")) .. " player " .. net.get_player_info(arg1, "name").." killed friendy " .. net.get_player_info(arg3, "name") .. " using " .. arg2);
        Perun.LogStats(arg1);

    elseif eventName == "mission_end" then
        --"mission_end", winner, msg
        Perun.LogEvent(eventName,"Mission finished, winner " .. arg1 .. " message: " .. arg2);

    elseif eventName == "kill" then
        --"kill", killerPlayerID, killerUnitType, killerSide, victimPlayerID, victimUnitType, victimSide, weaponName
        if net.get_player_info(arg4, "name") ~= nil then
            _temp = " player ".. net.get_player_info(arg4, "name") .." ";
            Perun.LogStats(arg4);
        else
            _temp = " AI ";
        end

        if net.get_player_info(arg1, "name") ~= nil then
            _temp2 = " player ".. net.get_player_info(arg1, "name") .." ";
            Perun.LogStats(arg1);
        else
            _temp2 = " AI ";
        end

        Perun.LogEvent(eventName,Perun.SideID2Name(arg3) .. _temp2 .. " in " .. arg2 .. " killed " .. Perun.SideID2Name(arg6) .. _temp .. " in " .. arg5  .. " using " .. arg7 .. " [".. Perun.GetCategory(arg5).."]");


    elseif eventName == "self_kill" then
        --"self_kill", playerID
        Perun.LogEvent(eventName,net.get_player_info(arg1, "name") .. " killed himself");
        Perun.LogStats(arg1);

    elseif eventName == "change_slot" then
        --"change_slot", playerID, slotID, prevSide

        if DCS.getUnitType(arg2) ~= nil and DCS.getUnitType(arg2) ~= "" then
            _temp = DCS.getUnitType(arg2);
        else
            _temp = "SPECTATOR";
        end

        Perun.LogEvent(eventName,Perun.SideID2Name( net.get_player_info(arg1, "side")) .. " player " .. net.get_player_info(arg1, "name") .. " changed slot to " .. _temp);
        Perun.LogStats(arg1);

    elseif eventName == "connect" then
        --"connect", playerID, name
        Perun.LogLogin(arg1);
        Perun.LogEvent(eventName,"Player "..net.get_player_info(arg1, "name") .. " connected");

    elseif eventName == "disconnect" then
        --"disconnect", playerID, name, playerSide, reason_code
        Perun.LogEvent(eventName, Perun.SideID2Name(arg3) .. " player " ..net.get_player_info(arg1, "name") .. " disconnected");
        Perun.LogStats(arg1);

    elseif eventName == "crash" then
        --"crash", playerID, unit_missionID
        Perun.LogEvent(eventName, Perun.SideID2Name( net.get_player_info(arg1, "side")) .. " player " .. net.get_player_info(arg1, "name") .. " crashed in " .. DCS.getUnitType(arg2));
        Perun.LogStats(arg1);

    elseif eventName == "eject" then
        --"eject", playerID, unit_missionID
        Perun.LogEvent(eventName, Perun.SideID2Name( net.get_player_info(arg1, "side")) .. " player " .. net.get_player_info(arg1, "name") .. " ejected " .. DCS.getUnitType(arg2));
        Perun.LogStats(arg1);

    elseif eventName == "takeoff" then
        --"takeoff", playerID, unit_missionID, airdromeName
        if arg3 then
            _temp = " from " .. arg3;
        else
            _temp = "";
        end

        Perun.LogEvent(eventName, Perun.SideID2Name( net.get_player_info(arg1, "side")) .. " player " .. net.get_player_info(arg1, "name") .. " took off in ".. DCS.getUnitType(arg2) .. _temp);
        Perun.LogStats(arg1);
    elseif eventName == "landing" then
        --"landing", playerID, unit_missionID, airdromeName
        if arg3 then
            _temp = " at " .. arg3;
        else
            _temp ="";
        end

        Perun.LogEvent(eventName, Perun.SideID2Name( net.get_player_info(arg1, "side")) .. " player " .. net.get_player_info(arg1, "name") .. " landed in " .. DCS.getUnitType(arg2).. _temp);
        Perun.LogStats(arg1);
    elseif eventName == "pilot_death" then
        --"pilot_death", playerID, unit_missionID
        Perun.LogEvent(eventName, Perun.SideID2Name( net.get_player_info(arg1, "side")) .. " player " .. net.get_player_info(arg1, "name") .. " in " .. DCS.getUnitType(arg2) .. " died");
        Perun.LogStats(arg1);

    else
        Perun.LogEvent(eventName,"Unknown event type");
    end

end

-- ########### Finalize and set callbacks ###########
DCS.setUserCallbacks(Perun)
net.log("Loaded - Perun - VladMordock - " .. Perun.Version )
