-- Perun for DCS World https://github.com/szporwolik/perun -> DCS Hook component

-- Initial init
local Perun = {}
package.path  = package.path..";"..lfs.currentdir().."/LuaSocket/?.lua"
package.cpath = package.cpath..";"..lfs.currentdir().."/LuaSocket/?.dll"

-- ###################### SETTINGS - DO NOT MODIFY OUTSIDE THIS SECTION #############################

Perun.RefreshStatus = 15 												-- (int) base refresh rate in seconds to send status update (values lower than 60 may affect performance!)
Perun.RefreshMission = 60 												-- (int) refresh rate in seconds to send mission information  (values lower than 60 may affect performance!)
Perun.TCPTargetPort = 48622												-- (int) TCP port to send data to
Perun.TCPPerunHost = "localhost"										-- (string) IP adress of the Perun instance or "localhost"
Perun.Instance = 2														-- (int) Id number of instance (if multiple DCS instances are to run at the same PC)
Perun.JsonStatusLocation = "Scripts\\Json\\" 							-- (string) folder relative do user's SaveGames DCS folder -> status file updated each RefreshMission
Perun.MOTD_L1 = "Witamy na serwerze Gildia.org !"						-- (string) Message send to players connecting the server - Line 1
Perun.MOTD_L2 = "Wymagamy obecnosci DCS SRS oraz TeamSpeak - szczegoly na forum"		-- (string) Message send to players connecting the server - Line 2

-- ###################### END OF SETTINGS - DO NOT MODIFY OUTSIDE THIS SECTION ######################


-- Variable init
Perun.Version = "v0.8.3"
Perun.StatusData = {}
Perun.SlotsData = {}
Perun.MissionData = {}
Perun.ServerData = {}
Perun.StatData = {}
Perun.StatDataLastType = {}
Perun.MissionHash=""
Perun.lastSentStatus =0
Perun.lastSentMission =0
Perun.lastSentKeepAlive =0
Perun.lastReconnect = 0
Perun.JsonStatusLocation = lfs.writedir() .. Perun.JsonStatusLocation
Perun.socket  = require("socket")
Perun.IsServer = true --DCS.isServer( )								-- TBD looks like DCS API error, always returning True

-- ########### Helper function definitions ###########
function stripChars(str)
    -- remove accents characters from string
    -- via https://stackoverflow.com/questions/50459102/replace-accented-characters-in-string-to-standard-with-lua
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

Perun.UpdateStatusPart = function(part_id, data_package)
    -- Helper for status update container
    Perun.StatusData[part_id] = data_package
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
	Perun.AddLog("Updated JSON")
end

Perun.ConnectToPerun = function ()
	-- Connects to Perun server
	Perun.AddLog("Connecting to TCP server")
	Perun.TCP = assert(Perun.socket.tcp())
	Perun.TCP:settimeout(5000)
	
	_, err = Perun.TCP:connect(Perun.TCPPerunHost, Perun.TCPTargetPort)
	if err then
		Perun.AddLog("ERROR - TCP connection error : " .. err)
	else
		Perun.AddLog("Sucess - connected to TCP server")
		-- Perun.TCP:setoption("keepalive")
		Perun.lastReconnect = _now
	end
end

Perun.SendToPerun = function(data_id, data_package)
    -- Prepares and sends data package
	-- Prepare data
    TempData={}
    TempData["type"]=data_id
    TempData["payload"]=data_package
    TempData["timestamp"]=os.date('%Y-%m-%d %H:%M:%S')
	TempData["instance"]=Perun.Instance
	
    temp=net.lua2json(TempData)
    temp="<SOT>" .. stripChars(temp) .. "<EOT>"

    -- TCP Part - sending
	intStatus = nil
	intTries =0
	err=nil
	while intStatus == nil and intTries < 3 do
		intStatus, err = Perun.TCP:send(temp) 
		if err then
			Perun.AddLog("Packed not send : " .. data_id .. " , error: " .. err .. ", tries: " .. intTries)
			Perun.ConnectToPerun()
		else
			Perun.AddLog("Packet send : " .. data_id .. " , tries:" .. intTries)
		end
		intTries=intTries+1
		err = nil
	end
	if err then
		Perun.AddLog("ERROR - packed dropped : " .. data_id)
	end 
end

Perun.UpdateStatus = function()
    -- Main function for status updates

    -- Diagnostic data
		-- Update version data
		Perun.ServerData['v_dcs_hook']=Perun.Version

		-- Update clients data data
		_table=net.get_player_list()
		_count = 0
		for _ in pairs(_table) do _count = _count + 1 end
		Perun.ServerData['c_players']=_count

		-- Send
		Perun.SendToPerun(1,Perun.ServerData)

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
		Perun.SendToPerun(2,Perun.StatusData)

    -- Update slots data
		Perun.SlotsData['coalitions']=DCS.getAvailableCoalitions()
		Perun.SlotsData['slots']={}

		for j, i in pairs(Perun.SlotsData['coalitions']) do
			Perun.SlotsData['slots'][j]=DCS.getAvailableSlots(j)
			
			for sj, si in pairs(Perun.SlotsData['slots'][j]) do
				Perun.SlotsData['slots'][j][sj]['countryName']= nil
				Perun.SlotsData['slots'][j][sj]['onboard_num']= nil
				Perun.SlotsData['slots'][j][sj]['groupSize']= nil
				Perun.SlotsData['slots'][j][sj]['groupName']= nil
				Perun.SlotsData['slots'][j][sj]['callsign']= nil
				Perun.SlotsData['slots'][j][sj]['task']= nil
				Perun.SlotsData['slots'][j][sj]['airdromeId']= nil
				Perun.SlotsData['slots'][j][sj]['helipadName']= nil
			end
			
		end
		-- Send
		Perun.SendToPerun(3,Perun.SlotsData)
end


Perun.UpdateMission = function()
    -- Main function for mission information updates
    Perun.MissionData=DCS.getCurrentMission()
	-- Perun.SendToPerun(4,Perun.MissionData) -- TBD can cause data transmission troubles
end

Perun.LogChat = function(playerID,msg,all)
    -- Logs chat messages

    data={}
    data['player']= net.get_player_info(playerID, "name")
    data['msg']=stripChars(msg)
    data['all']=all
    data['ucid']=net.get_player_info(playerID, 'ucid')
    data['datetime']=os.date('%Y-%m-%d %H:%M:%S')
    data['missionhash']=Perun.MissionHash

    Perun.SendToPerun(50,data)
end

Perun.LogEvent = function(log_type,log_content,log_arg_1,log_arg_2)
    -- Logs events messages

    data={}
    data['log_type']= log_type
	data['log_arg_1']= log_arg_1
	data['log_arg_2']= log_arg_2
    data['log_content']=log_content
    data['log_datetime']=os.date('%Y-%m-%d %H:%M:%S')
    data['log_missionhash']=Perun.MissionHash

    Perun.SendToPerun(51,data)
end

Perun.LogStatsCount = function(argPlayerID,argAction,argType)
	-- Creates or updates Perun statistics array
	_ucid=net.get_player_info(argPlayerID, 'ucid')..argType
	
	if Perun.StatData[_ucid] == nil then
		-- Create empty element
		_temp={}
		_temp['ps_type'] = argType
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
		_temp['ps_kills_fortification'] = 0
		_temp['ps_kills_other'] = 0
		_temp['ps_airfield_takeoffs'] = 0
		_temp['ps_airfield_landings'] = 0
		_temp['ps_ship_takeoffs'] = 0
		_temp['ps_ship_landings'] = 0
		_temp['ps_farp_takeoffs'] = 0
		_temp['ps_farp_landings'] = 0
		_temp['ps_other_takeoffs'] = 0
		_temp['ps_other_landings'] = 0

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
		if DCS.getModelTime() > 300 then
			-- we do not track deaths during first 5 minutes due to spawning issues TBD
			Perun.StatData[_ucid]['ps_deaths']=Perun.StatData[_ucid]['ps_deaths']+1
		end
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
	elseif  argAction == "kill_Fortification" then
		Perun.StatData[_ucid]['ps_kills_fortification']=Perun.StatData[_ucid]['ps_kills_fortification']+1
	elseif  argAction == "kill_Other" then
		Perun.StatData[_ucid]['ps_kills_other']=Perun.StatData[_ucid]['ps_kills_other']+1
	elseif  argAction == "kill_PvP" then
		Perun.StatData[_ucid]['ps_pvp']=Perun.StatData[_ucid]['ps_pvp']+1
	elseif  argAction == "landing_OTHER" then
		Perun.StatData[_ucid]['ps_other_landings']=Perun.StatData[_ucid]['ps_other_landings']+1
	elseif  argAction == "tookoff_OTHER" then
		Perun.StatData[_ucid]['ps_other_takeoffs']=Perun.StatData[_ucid]['ps_other_takeoffs']+1
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
	data['stat_name']=net.get_player_info(playerID, 'name')
    data['stat_datetime']=os.date('%Y-%m-%d %H:%M:%S')
    data['stat_missionhash']=Perun.MissionHash

    Perun.SendToPerun(52,data)
end

Perun.LogLogin = function(playerID)
    -- Player logged in

    data={}
    data['login_ucid']=net.get_player_info(playerID, 'ucid')
    data['login_ipaddr']=net.get_player_info(playerID, 'ipaddr')
    data['login_name']=net.get_player_info(playerID, 'name')
    data['login_datetime']=os.date('%Y-%m-%d %H:%M:%S')

    Perun.SendToPerun(53,data)
end

Perun.CheckMulticrew = function (owner_playerID,owner_unittype)
	-- Check multicrew and return  list of co-player

	_coplayers = {}
	table.insert(_coplayers, owner_playerID)
	if owner_unittype == "F-14B" or owner_unittype == "Yak-52" or owner_unittype == "L-39C" or owner_unittype == "SA342M" or owner_unittype =="SA342Minigun" or owner_unittype == "SA342Mistral" or owner_unittype == "SA342L" then -- TBD add additional multicrew model types
		
		_owner_slot=net.get_player_info(owner_playerID, 'slot')
		_owner_side=net.get_player_info(owner_playerID, 'side')
			
		-- Check if we are co-player
		_t_start, _t_end = string.find(_owner_slot, '_%d+')
		_sub_slot = nil
		if _t_start then
			-- This is co-player
			_master_slot = string.sub(_owner_slot, 0 , _t_start -1 )
			_sub_slot = string.sub(_owner_slot, _t_start + 1, _t_end )
		else
			_master_slot = _owner_slot

		end
		
		if _master_slot ~= "" then
			-- Search for all players to account for event
			_all_players = net.get_player_list()
			for PlayerIDIndex, playerID in ipairs(_all_players) do
				 local _playerDetails = net.get_player_info( playerID )
				 
				 if _playerDetails.side == _owner_side and (_playerDetails.slot == _master_slot  or _playerDetails.slot == _master_slot .. "_1" or _playerDetails.slot == _master_slot .. "_2") and playerID ~= owner_playerID then
					-- Let's build coplayers list
					table.insert(_coplayers, playerID)
				 else
					-- No coplayers
				 end
			end
		end
	end
	return _coplayers
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
	
	-- Send keepalive
	if _now > Perun.lastSentKeepAlive + 3 then
		Perun.lastSentKeepAlive = _now
		Perun.SendToPerun(0,nil)
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
		if arg2 == "" then
			arg2 = "Cannon"
		end
		
        Perun.LogEvent(eventName,Perun.SideID2Name( net.get_player_info(arg1, "side")) .. " player " .. net.get_player_info(arg1, "name").." killed friendy " .. net.get_player_info(arg3, "name") .. " using " .. arg2,nil,nil);

    elseif eventName == "mission_end" then
        --"mission_end", winner, msg
        Perun.LogEvent(eventName,"Mission finished, winner " .. arg1 .. " message: " .. arg2,nil,nil);

    elseif eventName == "kill" then
        --"kill", killerPlayerID, killerUnitType, killerSide, victimPlayerID, victimUnitType, victimSide, weaponName
		_tempX=""
		if net.get_player_info(arg4, "name") ~= nil then
            _tempX = " player ".. net.get_player_info(arg4, "name") .." ";
            
			Perun.LogStats(arg4);
        else
            _tempX = " AI ";
        end

        if net.get_player_info(arg1, "name") ~= nil then
			_temp2 = " player ".. net.get_player_info(arg1, "name") .." ";
			
			_temp_event_type=""
			if arg3 ~= arg6 then
				if Perun.GetCategory(arg5) == "Planes" then
					_temp_event_type="kill_Planes"
				elseif Perun.GetCategory(arg5) == "Helicopters" then
					_temp_event_type="kill_Helicopters"
				elseif Perun.GetCategory(arg5) == "Ships" then
					_temp_event_type="kill_Ships"
				elseif Perun.GetCategory(arg5) == "Air Defence" then
					_temp_event_type="kill_Air_Defence"
				elseif Perun.GetCategory(arg5) == "Unarmed" then
					_temp_event_type="kill_Unarmed"
				elseif Perun.GetCategory(arg5) == "Armor" then
					_temp_event_type="kill_Armor"
				elseif Perun.GetCategory(arg5) == "Infantry" then
					_temp_event_type="kill_Infantry"
				elseif Perun.GetCategory(arg5) == "Fortification" then
					_temp_event_type="kill_Fortification"
				else 
					_temp_event_type="kill_Other"
				end
				if net.get_player_info(arg4, "name") ~= nil and arg3 ~= arg6 then
					pilots_accounted = Perun.CheckMulticrew(arg1,arg2)
					for _, pilotID in ipairs(pilots_accounted) do
						Perun.LogStatsCount(pilotID,"kill_PvP",DCS.getUnitType(arg2));
					end
				end
			else
				_temp_event_type="friendly_fire"
			end
			
			pilots_accounted = Perun.CheckMulticrew(arg1,arg2)
			for _, pilotID in ipairs(pilots_accounted) do
				Perun.LogStatsCount(pilotID,_temp_event_type,DCS.getUnitType(arg2));
			end
			
        else
            _temp2 = " AI ";
        end
		
		if arg7 == "" then
			arg7 = "Cannon"
		end

		Perun.LogEvent(eventName,Perun.SideID2Name(arg3) .. _temp2 .. " in " .. arg2 .. " killed " .. Perun.SideID2Name(arg6) .. _tempX .. " in " .. arg5  .. " using " .. arg7 .. " [".. Perun.GetCategory(arg5).."]",arg7,Perun.GetCategory(arg5));
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

       Perun.LogEvent(eventName,Perun.SideID2Name( net.get_player_info(arg1, "side")) .. " player " .. net.get_player_info(arg1, "name") .. " changed slot to " .. _temp,nil,nil);
       Perun.LogStatsCount(arg1,"init",_temp)

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
		
		pilots_accounted = Perun.CheckMulticrew(arg1,DCS.getUnitType(arg2))
		for _, pilotID in ipairs(pilots_accounted) do
			Perun.LogStatsCount(pilotID,"crash",DCS.getUnitType(arg2));
		end
    elseif eventName == "eject" then
        --"eject", playerID, unit_missionID
        Perun.LogEvent(eventName, Perun.SideID2Name( net.get_player_info(arg1, "side")) .. " player " .. net.get_player_info(arg1, "name") .. " ejected " .. DCS.getUnitType(arg2),nil,nil);
		
		pilots_accounted = Perun.CheckMulticrew(arg1,DCS.getUnitType(arg2))
		for _, pilotID in ipairs(pilots_accounted) do
			Perun.LogStatsCount(pilotID,"eject",DCS.getUnitType(arg2));
		end
		
    elseif eventName == "takeoff" then
        --"takeoff", playerID, unit_missionID, airdromeName
        if arg3 ~= "" then
            _temp = " from " .. arg3;
        else
            _temp = "";
        end

        Perun.LogEvent(eventName, Perun.SideID2Name( net.get_player_info(arg1, "side")) .. " player " .. net.get_player_info(arg1, "name") .. " took off in ".. DCS.getUnitType(arg2) .. _temp,arg3,nil);
        
		_type = ""
		if string.find(arg3, "FARP",1,true) then
			_type="tookoff_FARP"
		elseif string.find(arg3, "CVN-74 John C. Stennis",1,true) or string.find(arg3, "LHA-1 Tarawa",1,true) then
			_type="tookoff_SHIP"
		elseif arg3 ~= "" then
			_type="tookoff_AIRFIELD"
		else
			_type="tookoff_OTHER"
		end
		
		pilots_accounted = Perun.CheckMulticrew(arg1,DCS.getUnitType(arg2))
		for _, pilotID in ipairs(pilots_accounted) do
			Perun.LogStatsCount(pilotID,_type,DCS.getUnitType(arg2))
		end
		
    elseif eventName == "landing" then
        --"landing", playerID, unit_missionID, airdromeName
        if arg3 ~= "" then
            _temp = " at " .. arg3;
        else
            _temp ="";
        end

        Perun.LogEvent(eventName, Perun.SideID2Name( net.get_player_info(arg1, "side")) .. " player " .. net.get_player_info(arg1, "name") .. " landed in " .. DCS.getUnitType(arg2).. _temp,arg3,nil);
		
		_type = ""
		if string.find(arg3, "FARP",1,true) then
			_type = "landing_FARP"
		elseif string.find(arg3, "CVN-74 John C. Stennis",1,true) or string.find(arg3, "LHA-1 Tarawa",1,true) then
			_type = "landing_SHIP"
		elseif arg3 ~= "" then
			_type = "landing_AIRFIELD"
		else
			_type = "landing_OTHER"
		end
		
		pilots_accounted = Perun.CheckMulticrew(arg1,DCS.getUnitType(arg2))
		for _, pilotID in ipairs(pilots_accounted) do
			Perun.LogStatsCount(pilotID,_type,DCS.getUnitType(arg2))
		end
		
    elseif eventName == "pilot_death" then
        --"pilot_death", playerID, unit_missionID
        Perun.LogEvent(eventName, Perun.SideID2Name( net.get_player_info(arg1, "side")) .. " player " .. net.get_player_info(arg1, "name") .. " in " .. DCS.getUnitType(arg2) .. " died",nil,nil);

		pilots_accounted = Perun.CheckMulticrew(arg1,DCS.getUnitType(arg2))
		for _, pilotID in ipairs(pilots_accounted) do
			Perun.LogStatsCount(pilotID,"pilot_death",DCS.getUnitType(arg2))
		end
		
    else
        Perun.LogEvent(eventName,"Unknown event type",nil,nil);
    end

end

-- ########### Finalize and set callbacks ###########
if Perun.IsServer then
	DCS.setUserCallbacks(Perun)
	net.log("Perun by VladMordock was loaded: " .. Perun.Version )
	Perun.ConnectToPerun()
end
