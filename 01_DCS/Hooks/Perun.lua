-- Perun for DCS World https://github.com/szporwolik/perun -> DCS Hook component

-- Initial init
local Perun = {}
package.path  = package.path..";"..lfs.currentdir().."/LuaSocket/?.lua"
package.cpath = package.cpath..";"..lfs.currentdir().."/LuaSocket/?.dll"

-- ###################### SETTINGS - DO NOT MODIFY OUTSIDE THIS SECTION #############################

Perun.RefreshStatus = 15 												-- (int) base refresh rate in seconds to send status update (values lower than 60 may affect performance!)
Perun.RefreshMission = 60 												-- (int) refresh rate in seconds to send mission information  (values lower than 60 may affect performance!)
Perun.UDPTargetPort = 48620												-- (int) UDP port to send data to
Perun.UDPSourcePort = 48621												-- (int) UDP port to send data from 
Perun.Instance = 1														-- (int) Id number of instance (if multiple DCS instances are to run at the same PC)
Perun.JsonStatusLocation = "Scripts\\Json\\" 							-- (string) folder relative do user's SaveGames DCS folder -> status file updated each RefreshMission
Perun.MOTD_L1 = "Witamy na serwerze Gildia.org !"						-- (string) Message send to players connecting the server - Line 1
Perun.MOTD_L2 = "Wymagamy obecnosci na 255Mhz (DCS SRS)"				-- (string) Message send to players connecting the server - Line 2

-- ###################### END OF SETTINGS - DO NOT MODIFY OUTSIDE THIS SECTION ######################


-- Variable init
Perun.Version = "v0.5.0"
Perun.StatusData = {}
Perun.SlotsData = {}
Perun.MissionData = {}
Perun.ServerData = {}
Perun.StatData = {}
Perun.StatDataLastType = {}
Perun.MissionHash=""
Perun.lastSentStatus =0
Perun.lastSentMission =0
Perun.JsonStatusLocation = lfs.writedir() .. Perun.JsonStatusLocation
Perun.socket  = require("socket")
Perun.IsServer = true --DCS.isServer( )								-- TBD looks like DCS API error, always returning True
Perun.UDPSourcePort  = Perun.UDPSourcePort + Perun.Instance - 1

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
	TempData["instance"]=Perun.Instance
	
    temp=net.lua2json(TempData)
    temp=stripChars(temp)

    Perun.UDP:send(temp)
    Perun.AddLog("Packet send "..data_id)
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

Perun.LogEvent = function(log_type,log_content,log_arg_1,log_arg_2)
    -- Log chat messages

    data={}
    data['log_type']= log_type
	data['log_arg_1']= log_arg_1
	data['log_arg_2']= log_arg_2
    data['log_content']=log_content
    data['log_datetime']=os.date('%Y-%m-%d %H:%M:%S')
    data['log_missionhash']=Perun.MissionHash

    Perun.Send(51,data)
end

Perun.LogStatsCount = function(argPlayerID,argAction,argType)
	-- Creates or updates Perun statistics array
	_ucid=net.get_player_info(argPlayerID, 'ucid')..argType
	
	if Perun.StatData[_ucid] == nil then
		-- Create empty element
		_temp={}
		_temp['ps_type'] = argType
		_temp['ps_kills'] = 0
		_temp['ps_pvp'] = 0
		_temp['ps_deaths'] = 0
		_temp['ps_ejections'] = 0
		_temp['ps_crashes'] = 0
		_temp['ps_teamkills'] = 0
		_temp['ps_kills_planes'] = 0
		_temp['ps_kills_helicopters'] = 0
		_temp['ps_kills_air_defense'] = 0
		_temp['ps_kills_armor'] = 0
		_temp['ps_kills_unarmed'] = 0
		_temp['ps_kills_infantry'] = 0
		_temp['ps_kills_ships'] = 0
		_temp['ps_kills_other'] = 0
		_temp['ps_airfield_takeoffs'] = 0
		_temp['ps_airfield_landings'] = 0
		_temp['ps_ship_takeoffs'] = 0
		_temp['ps_ship_landings'] = 0
		_temp['ps_farp_takeoffs'] = 0
		_temp['ps_farp_landings'] = 0

		Perun.StatData[_ucid]=_temp
	end
	
	if argType ~= nil then
		Perun.StatData[_ucid]['ps_type']=argType;
		Perun.StatDataLastType[net.get_player_info(argPlayerID, 'ucid')]=_ucid
	else
		-- Do nothing
	end 
	
	if argAction == "eject" then
		Perun.StatData[_ucid]['ps_ejections']=Perun.StatData[_ucid]['ps_ejections']+1
	elseif  argAction == "pilot_death" then
		Perun.StatData[_ucid]['ps_deaths']=Perun.StatData[_ucid]['ps_deaths']+1
	elseif  argAction == "friendly_fire" then
		Perun.StatData[_ucid]['ps_teamkills']=Perun.StatData[_ucid]['ps_teamkills']+1
	elseif  argAction == "crash" then
		Perun.StatData[_ucid]['ps_crashes']=Perun.StatData[_ucid]['ps_crashes']+1
	elseif  argAction == "landing_FARP" then
		Perun.StatData[_ucid]['ps_farp_landings']=Perun.StatData[_ucid]['ps_farp_landings']+1
	elseif  argAction == "landing_SHIP" then
		Perun.StatData[_ucid]['ps_ship_landings']=Perun.StatData[_ucid]['ps_ship_landings']+1
	elseif  argAction == "landing_AIRFIELD" then
		Perun.StatData[_ucid]['ps_airfield_landings']=Perun.StatData[_ucid]['ps_airfield_landings']+1
	elseif  argAction == "tookoff_FARP" then
		Perun.StatData[_ucid]['ps_farp_takeoffs']=Perun.StatData[_ucid]['ps_farp_takeoffs']+1
	elseif  argAction == "tookoff_SHIP" then
		Perun.StatData[_ucid]['ps_ship_takeoffs']=Perun.StatData[_ucid]['ps_ship_takeoffs']+1
	elseif  argAction == "tookoff_AIRFIELD" then
		Perun.StatData[_ucid]['ps_airfield_takeoffs']=Perun.StatData[_ucid]['ps_airfield_takeoffs']+1
	elseif  argAction == "kill_Planes" then
		Perun.StatData[_ucid]['ps_kills_planes']=Perun.StatData[_ucid]['ps_kills_planes']+1
	elseif  argAction == "kill_Helicopters" then
		Perun.StatData[_ucid]['ps_kills_helicopters']=Perun.StatData[_ucid]['ps_kills_helicopters']+1
	elseif  argAction == "kill_Ships" then
		Perun.StatData[_ucid]['ps_kills_ships']=Perun.StatData[_ucid]['ps_kills_ships']+1
	elseif  argAction == "kill_Air_Defence" then
		Perun.StatData[_ucid]['ps_kills_air_defense']=Perun.StatData[_ucid]['ps_kills_air_defense']+1
	elseif  argAction == "kill_Unarmed" then
		Perun.StatData[_ucid]['ps_kills_unarmed']=Perun.StatData[_ucid]['ps_kills_unarmed']+1
	elseif  argAction == "kill_Armor" then
		Perun.StatData[_ucid]['ps_kills_armor']=Perun.StatData[_ucid]['ps_kills_armor']+1
	elseif  argAction == "kill_Infantry" then
		Perun.StatData[_ucid]['ps_kills_infantry']=Perun.StatData[_ucid]['ps_kills_infantry']+1
	elseif  argAction == "kill_Other" then
		Perun.StatData[_ucid]['ps_kills_other']=Perun.StatData[_ucid]['ps_kills_other']+1
	elseif  argAction == "kill_PvP" then
		Perun.StatData[_ucid]['ps_pvp']=Perun.StatData[_ucid]['ps_pvp']+1
	end
	
	Perun.LogStats(argPlayerID);
end

Perun.LogStatsGet = function(playerID)
	-- Gets Perun statistics array per player
	
	local next = next 
	if next(Perun.StatDataLastType) == nil then
		_ucid=net.get_player_info(playerID, 'ucid')..DCS.getUnitType(playerID)
	elseif Perun.StatDataLastType[net.get_player_info(playerID, 'ucid')]== nil then
		_ucid=net.get_player_info(playerID, 'ucid')..DCS.getUnitType(playerID)
	else
		_ucid=Perun.StatDataLastType[net.get_player_info(playerID, 'ucid')]
	end
	
	local next = next 
	if  next(Perun.StatData) == nil then
		Perun.LogStatsCount(playerID,'init')
		
	end
	if Perun.StatData[_ucid]== nil then
		Perun.LogStatsCount(playerID,'init')
	end
	return Perun.StatData[_ucid];
end



Perun.LogStats = function(playerID)
    -- Log player status
	
    -- TBD : DCS stats not working at the moment WIP
    p_stats_dcs={}
    p_stats_dcs['PS_CAR']=net.get_stat(playerID,2)
    p_stats_dcs['PS_PLANE']=net.get_stat(playerID,3)
    p_stats_dcs['PS_SHIP']=net.get_stat(playerID,4)
    p_stats_dcs['PS_SCORE']=net.get_stat(playerID,5)
    p_stats_dcs['PS_LAND']=net.get_stat(playerID,6)
    p_stats_dcs['PS_PING']=net.get_stat(playerID,0)
    p_stats_dcs['PS_CRASH']=net.get_stat(playerID,1)
    p_stats_dcs['PS_EJECT']=net.get_stat(playerID,7)
	
	_temp=Perun.LogStatsGet(playerID)
    data={}
    data['stat_data_dcs']=p_stats_dcs
	data['stat_data_perun']=_temp
	data['stat_data_type']=_temp['ps_type'];
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
    Perun.MissionHash=DCS.getMissionName( ).."@".. Perun.Instance .. "@"..os.date('%Y%m%d_%H%M%S');
    Perun.LogEvent("SimStart","Mission " .. Perun.MissionHash .. " started",nil,nil);
	Perun.StatData = {}
	Perun.StatDataLastType = {}
end

Perun.onSimulationStop = function()
    Perun.LogEvent("SimStop","Mission " .. Perun.MissionHash .. " finished",nil,nil);
	Perun.MissionHash="";
	Perun.StatData = {}
	Perun.StatDataLastType = {}
end

Perun.onPlayerDisconnect = function(id, err_code)
    Perun.LogEvent("disconnect", "Player " .. net.get_player_info(id, "name") .. " disconnected; " .. err_code,net.get_player_info(id, "name"),err_code);
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
    net.send_chat_to(Perun.MOTD_L1, id);
    net.send_chat_to(Perun.MOTD_L2, id);
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
		if arg2 == nil then
			arg2 = "Cannon"
		end
		
        Perun.LogEvent(eventName,Perun.SideID2Name( net.get_player_info(arg1, "side")) .. " player " .. net.get_player_info(arg1, "name").." killed friendy " .. net.get_player_info(arg3, "name") .. " using " .. arg2,nil,nil);
        Perun.LogStatsCount(arg1,"friendly_fire",DCS.getUnitType(arg1))

    elseif eventName == "mission_end" then
        --"mission_end", winner, msg
        Perun.LogEvent(eventName,"Mission finished, winner " .. arg1 .. " message: " .. arg2,nil,nil);

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
            
			if Perun.GetCategory(arg5) == "Planes" then
				Perun.LogStatsCount(arg1,"kill_Planes",arg2)
			elseif Perun.GetCategory(arg5) == "Helicopters" then
				Perun.LogStatsCount(arg1,"kill_Helicopters",arg2)
			elseif Perun.GetCategory(arg5) == "Ships" then
				Perun.LogStatsCount(arg1,"kill_Ships",arg2)
			elseif Perun.GetCategory(arg5) == "Air Defence" then
				Perun.LogStatsCount(arg1,"kill_Air_Defence",arg2)
			elseif Perun.GetCategory(arg5) == "Unarmed" then
				Perun.LogStatsCount(arg1,"kill_Unarmed",arg2)
			elseif Perun.GetCategory(arg5) == "Armor" then
				Perun.LogStatsCount(arg1,"kill_Armor",arg2)
			elseif Perun.GetCategory(arg5) == "Infantry" then
				Perun.LogStatsCount(arg1,"kill_Infantry",arg2)
			else 
				Perun.LogStatsCount(arg1,"kill_Other",arg2)
			end
			
			if net.get_player_info(arg4, "name") ~= nil and killerSide ~= victimSide then
				Perun.LogStatsCount(arg1,"kill_PvP",arg2)
			end
        else
            _temp2 = " AI ";
        end
		
		if arg7 == nil then
			arg7 = "Cannon"
		end
		
        Perun.LogEvent(eventName,Perun.SideID2Name(arg3) .. _temp2 .. " in " .. arg2 .. " killed " .. Perun.SideID2Name(arg6) .. _temp .. " in " .. arg5  .. " using " .. arg7 .. " [".. Perun.GetCategory(arg5).."]",arg7,Perun.GetCategory(arg5));

    elseif eventName == "self_kill" then
        --"self_kill", playerID
        Perun.LogEvent(eventName,net.get_player_info(arg1, "name") .. " killed himself",nil,nil);
        Perun.LogStats(arg1);

    elseif eventName == "change_slot" then
        --"change_slot", playerID, slotID, prevSide

        if DCS.getUnitType(arg2) ~= nil and DCS.getUnitType(arg2) ~= "" then
            _temp = DCS.getUnitType(arg2);
        else
            _temp = "SPECTATOR";
        end

        Perun.LogEvent(eventName,Perun.SideID2Name( net.get_player_info(arg1, "side")) .. " player " .. net.get_player_info(arg1, "name") .. " changed slot to " .. _temp);
       Perun.LogStatsCount(arg1,"init",_temp,nil,nil)

    elseif eventName == "connect" then
        --"connect", playerID, name
        Perun.LogLogin(arg1);
        Perun.LogEvent(eventName,"Player "..net.get_player_info(arg1, "name") .. " connected",nil,nil);

    elseif eventName == "disconnect" then
        --"disconnect", playerID, name, playerSide, reason_code
        Perun.LogEvent(eventName, Perun.SideID2Name(arg3) .. " player " ..net.get_player_info(arg1, "name") .. " disconnected",nil,nil);
        Perun.LogStats(arg1);

    elseif eventName == "crash" then
        --"crash", playerID, unit_missionID
        Perun.LogEvent(eventName, Perun.SideID2Name( net.get_player_info(arg1, "side")) .. " player " .. net.get_player_info(arg1, "name") .. " crashed in " .. DCS.getUnitType(arg2),nil,nil);
		Perun.LogStatsCount(arg1,"crash",DCS.getUnitType(arg2))
    elseif eventName == "eject" then
        --"eject", playerID, unit_missionID
        Perun.LogEvent(eventName, Perun.SideID2Name( net.get_player_info(arg1, "side")) .. " player " .. net.get_player_info(arg1, "name") .. " ejected " .. DCS.getUnitType(arg2),nil,nil);
		Perun.LogStatsCount(arg1,"eject",DCS.getUnitType(arg2));

    elseif eventName == "takeoff" then
        --"takeoff", playerID, unit_missionID, airdromeName
        if arg3 then
            _temp = " from " .. arg3;
        else
            _temp = "";
        end

        Perun.LogEvent(eventName, Perun.SideID2Name( net.get_player_info(arg1, "side")) .. " player " .. net.get_player_info(arg1, "name") .. " took off in ".. DCS.getUnitType(arg2) .. _temp,arg3,nil);
        
		if string.find(arg3, "FARP") then
			Perun.LogStatsCount(arg1,"tookoff_FARP",DCS.getUnitType(arg2))
		elseif string.find(arg3, "CVN-74 John C. Stennis") or string.find(arg3, "LHA-1 Tarawa") then
			Perun.LogStatsCount(arg1,"tookoff_SHIP",DCS.getUnitType(arg2))
		elseif arg3 then
			Perun.LogStatsCount(arg1,"tookoff_AIRFIELD",DCS.getUnitType(arg2))
		else
			-- do nothing
		end
		
    elseif eventName == "landing" then
        --"landing", playerID, unit_missionID, airdromeName
        if arg3 then
            _temp = " at " .. arg3;
        else
            _temp ="";
        end

        Perun.LogEvent(eventName, Perun.SideID2Name( net.get_player_info(arg1, "side")) .. " player " .. net.get_player_info(arg1, "name") .. " landed in " .. DCS.getUnitType(arg2).. _temp,arg3,nil);
		
		if string.find(arg3, "FARP") then
			Perun.LogStatsCount(arg1,"landing_FARP",DCS.getUnitType(arg2))
		elseif string.find(arg3, "CVN-74 John C. Stennis") or string.find(arg3, "LHA-1 Tarawa") then
			Perun.LogStatsCount(arg1,"landing_SHIP",DCS.getUnitType(arg2))
		elseif arg3 then
			Perun.LogStatsCount(arg1,"landing_AIRFIELD",DCS.getUnitType(arg2))
		else
			-- do nothing
		end
		
    elseif eventName == "pilot_death" then
        --"pilot_death", playerID, unit_missionID
        Perun.LogEvent(eventName, Perun.SideID2Name( net.get_player_info(arg1, "side")) .. " player " .. net.get_player_info(arg1, "name") .. " in " .. DCS.getUnitType(arg2) .. " died",nil,nil);
		Perun.LogStatsCount(arg1,"pilot_death",DCS.getUnitType(arg2))
    else
        Perun.LogEvent(eventName,"Unknown event type",nil,nil);
    end

end

-- ########### Finalize and set callbacks ###########
if Perun.IsServer then
	Perun.UDP = assert(Perun.socket.udp())
	Perun.UDP:settimeout(0)
	Perun.UDP:setsockname("*", Perun.UDPSourcePort)
	Perun.UDP:setpeername("127.0.0.1",Perun.UDPTargetPort)

	DCS.setUserCallbacks(Perun)
	net.log("Loaded - Perun - VladMordock - " .. Perun.Version )
end
