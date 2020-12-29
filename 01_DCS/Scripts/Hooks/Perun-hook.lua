-- Perun for DCS World https://github.com/szporwolik/perun -> DCS Hook component
net.log("Loading - Perun")	-- Display perun information in log

-- Initial init
local Perun = {}

-- Load Lua Socket
package.path  = package.path..";"..lfs.currentdir().."/LuaSocket/?.lua"..";"..lfs.writedir() .. "/Mods/services/Perun/lua/?.lua"
package.cpath = package.cpath..";"..lfs.currentdir().."/LuaSocket/?.dll"

-- Load config file
local Config = require "perun_config"
Perun.RefreshStatus = Config.RefreshStatus
Perun.RefreshMission = Config.RefreshMission
Perun.TCPTargetPort = Config.TCPTargetPort
Perun.TCPPerunHost = Config.TCPPerunHost
Perun.Instance = Config.Instance
Perun.MissionStartNoDeathWindow = Config.MissionStartNoDeathWindow
Perun.DebugMode = Config.DebugMode
Perun.MOTD_L1 = Config.MOTD_L1
Perun.MOTD_L2 = Config.MOTD_L2

-- Variable init
Perun.Version = "v0.11.2"
Perun.StatusData = {}
Perun.SlotsData = {}
Perun.MissionData = {}
Perun.ServerData = {}
Perun.StatData = {}
Perun.StatDataLastType = {}
Perun.PlayersTableCache = {}
Perun.MissionHash=""
Perun.ReconnectTimeout = 30
Perun.lastSentStatus = 0
Perun.lastSentMission = 0
Perun.lastSentKeepAlive = 0
Perun.lastConnectionError = 0
Perun.lastTimer = 0
Perun.lastReconnect = (-1) * Perun.ReconnectTimeout
Perun.SendRetries = 2
Perun.RefreshKeepAlive = 3
Perun.socket  = require("socket")
Perun.IsServer = DCS.isServer( )								
Perun.ConnectionError = "[Perun] ERROR: Connection broken - contact server admin!"

-- ################################ Helper function definitions ################################
function stripChars(str)
    -- Hellper functions removes accents characters from string
    -- via https://stackoverflow.com/questions/50459102/replace-accented-characters-in-string-to-standard-with-lua
    local _tableAccents = {}
    _tableAccents["à"] = "a"
    _tableAccents["á"] = "a"
    _tableAccents["â"] = "a"
    _tableAccents["ã"] = "a"
    _tableAccents["ä"] = "a"
    _tableAccents["ç"] = "c"
    _tableAccents["è"] = "e"
    _tableAccents["é"] = "e"
    _tableAccents["ê"] = "e"
    _tableAccents["ë"] = "e"
    _tableAccents["ì"] = "i"
    _tableAccents["í"] = "i"
    _tableAccents["î"] = "i"
    _tableAccents["ï"] = "i"
    _tableAccents["ñ"] = "n"
    _tableAccents["ò"] = "o"
    _tableAccents["ó"] = "o"
    _tableAccents["ô"] = "o"
    _tableAccents["õ"] = "o"
    _tableAccents["ö"] = "o"
    _tableAccents["ù"] = "u"
    _tableAccents["ú"] = "u"
    _tableAccents["û"] = "u"
    _tableAccents["ü"] = "u"
    _tableAccents["ý"] = "y"
    _tableAccents["ÿ"] = "y"
    _tableAccents["À"] = "A"
    _tableAccents["Á"] = "A"
    _tableAccents["Â"] = "A"
    _tableAccents["Ã"] = "A"
    _tableAccents["Ä"] = "A"
    _tableAccents["Ç"] = "C"
    _tableAccents["È"] = "E"
    _tableAccents["É"] = "E"
    _tableAccents["Ê"] = "E"
    _tableAccents["Ë"] = "E"
    _tableAccents["Ì"] = "I"
    _tableAccents["Í"] = "I"
    _tableAccents["Î"] = "I"
    _tableAccents["Ï"] = "I"
    _tableAccents["Ñ"] = "N"
    _tableAccents["Ò"] = "O"
    _tableAccents["Ó"] = "O"
    _tableAccents["Ô"] = "O"
    _tableAccents["Õ"] = "O"
    _tableAccents["Ö"] = "O"
    _tableAccents["Ù"] = "U"
    _tableAccents["Ú"] = "U"
    _tableAccents["Û"] = "U"
    _tableAccents["Ü"] = "U"
    _tableAccents["Ý"] = "Y"

    -- Polish accents
    _tableAccents["ę"] = "e"
    _tableAccents["Ę"] = "Ę"
    _tableAccents["ó"] = "o"
    _tableAccents["Ó"] = "O"
    _tableAccents["ą"] = "a"
    _tableAccents["Ą"] = "A"
    _tableAccents["ś"] = "s"
    _tableAccents["Ś"] = "S"
    _tableAccents["ć"] = "c"
    _tableAccents["Ć"] = "C"
    _tableAccents["ż"] = "z"
    _tableAccents["Ż"] = "Z"
    _tableAccents["ź"] = "z"
    _tableAccents["Ź"] = "Z"
    _tableAccents["ł"] = "l"
    _tableAccents["Ł"] = "L"

    -- TBD additonal characters for other languages

	-- Check string and replace special chars via replacement table
    local _normalizedString = ''
    for _strChar in string.gmatch(str, "([%z\1-\127\194-\244][\128-\191]*)") do
        if _tableAccents[_strChar] ~= nil then
			-- Replace char
            _normalizedString = _normalizedString.._tableAccents[_strChar]
        else
			-- No need to replace
            _normalizedString = _normalizedString.._strChar
        end
    end
    return _normalizedString
end

Perun.GetCategory = function(id)
    -- Helper function returns object category basing on https://pastebin.com/GUAXrd2U
    local _killed_target_category = "Other"
    
	-- Sometimes we get empty object id (seems like DCS API bug)
	if id ~= nil and id ~= "" then
		_killed_target_category = DCS.getUnitTypeAttribute(id, "category")
		
		-- Below, simple hack to get the propper category when DCS API is not returning correct value
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
	end
	
    return _killed_target_category
end

Perun.SideID2Name = function(id)
    -- Helper function returns side name per side (coalition) id
    local _sides = {
        [0] = 'SPECTATOR',
        [1] = 'RED',
        [2] = 'BLUE',
		[3] = 'NEUTRAL',	-- TBD check once this is released in DCS
    }
	if id > 0 then
		return _sides[id]
	else 
		return "?"
	end
end

Perun.AddLog = function(text,LogLevel)
    -- Adds logs to DCS.log file
	LogLevel = LogLevel or 1
	if Perun.DebugMode >= LogLevel then
		net.log("[Perun] ".. text)
	end
end

Perun.GenerateMissionHash = function()
	-- Generates unique simulation mission hash
	return DCS.getMissionName( ).."@".. Perun.Instance .. "@" .. Perun.Version .. "@".. os.date('%Y%m%d_%H%M%S') 
end

Perun.GetMulticrewAllParameters = function (PlayerId)
	-- Gets all multicrew parameters
	local _result = ""
	local _master_type= "?"
	local _master_slot = nil
	local _sub_slot = nil

	local _player_slot = net.get_player_info(PlayerId, 'slot')
	if not _player_slot then
		_player_slot=Perun.PlayersTableCache["p"..PlayerId].slot
	end
	
	if _player_slot and _player_slot ~= '' and not (string.find(_player_slot, 'red') or string.find(_player_slot, 'blue')) then
		-- Player took model
		_master_slot = _player_slot
		_sub_slot =0

		if (not tonumber(_player_slot)) then 
			-- If this is multiseat slot parse master slot and look for seat number
			_t_start, _t_end = string.find(_player_slot, '_%d+')
		
			if _t_start then
				-- This is co-player
				_master_slot = string.sub(_player_slot, 0 , _t_start -1 )
				_sub_slot = string.sub(_player_slot, _t_start + 1, _t_end )
			end
		end
		_master_type = DCS.getUnitType(_master_slot)

	elseif string.find(_player_slot, 'red') or string.find(_player_slot, 'blue') then
		-- Deal with the special slots addded by Combined Arms and Spectators
		if string.find(_player_slot, 'artillery_commander') then
			_master_type = "artillery_commander"
		elseif string.find(_player_slot, 'instructor') then
			_master_type = "instructor"
		elseif string.find(_player_slot, 'forward_observer') then
			_master_type = "forward_observer"
		elseif string.find(_player_slot, 'observer') then
			_master_type = "observer"
		end
		_master_slot = -1
		_sub_slot = 0
	else
		_master_slot = -1
		_sub_slot = -1
	end
	return _master_type,_master_slot,_sub_slot
end

Perun.GetMulticrewParameter = function (PlayerId,Parameter)
	-- Get specific multicrew Parameter
	_master_type,_master_slot,_sub_slot = Perun.GetMulticrewAllParameters(PlayerId)

	if Parameter == "mastertype" then
		return _master_type
	elseif Parameter == "masterslot" then
		return _master_slot
	elseif Parameter == "subslot" then
		return _sub_slot
	elseif Parameter == "subtype" then
		if _sub_slot == 0 then
			return _master_type
		else
			return _master_type .. "_" .. _sub_slot
		end
	else
		return nil
	end
end

Perun.GetMulticrewCrew = function (owner_playerID)
	-- Get all multicrew crew
	_master_type,_master_slot,_sub_slot = Perun.GetMulticrewAllParameters(owner_playerID)

	local _crew = {}
	table.insert(_crew, owner_playerID)
	if _master_type == "F-14B" or _master_type == "Yak-52" or _master_type == "L-39C" or _master_type == "SA342M" or _master_type =="SA342Minigun" or _master_type == "SA342Mistral" or _master_type == "SA342L" or _master_type == "F-14A-135-GR" or _master_type == "C-101EB" or _master_type == "UH-1H" or _master_type == "C-101CC" then -- TBD add additional multicrew model types
		local _owner_side=net.get_player_info(owner_playerID, 'side')
		
		if _master_slot and _master_slot ~= "" then
			-- Search for all players from crew
			local _all_players = net.get_player_list()
			for PlayerIDIndex, _playerID in ipairs(_all_players) do
				 local _playerDetails = net.get_player_info( _playerID )
				 
				 if _playerDetails.side == _owner_side and (_playerDetails.slot == _master_slot  or _playerDetails.slot == _master_slot .. "_1" or _playerDetails.slot == _master_slot .. "_2" or _playerDetails.slot == _master_slot .. "_3" or _playerDetails.slot == _master_slot .. "_4") and _playerID ~= owner_playerID then
					-- Add to crew list
					table.insert(_crew, _playerID)
				 else
					-- Do nothing
				 end
			end
		end
	end

	Perun.AddLog("Multicrew check completed: " .. net.lua2json(_crew),2)
	return _crew
end

Perun.GetMulticrewCrewNames = function (owner_playerID)
	-- Get all crew names and return as string (needed for logging purposes)
	local _pilots_accounted = Perun.GetMulticrewCrew(owner_playerID)
	local _result_text = ""
	for _, pilotID in ipairs(_pilots_accounted) do
		_result_text= _result_text .. net.get_player_info(pilotID, "name") .. ", "
	end
	return _result_text
end

Perun.GetTakeOffLandingEvent = function (takeoff,location)
	-- Get correct event type for statistics basing on objecy which served as runway/farp
	local _operation = ""
	if takeoff == true then
		_operation="tookoff_"
	else
		_operation="landing_"
	end
	
	local _temp_type = ""
	if string.find(location, "FARP",1,true) then
		_temp_type = "FARP"
	elseif string.find(location, "CVN-74 John C. Stennis",1,true) or string.find(location, "LHA-1 Tarawa",1,true) or string.find(location, "SHIP",1,true) then
		_temp_type = "SHIP"
	elseif location ~= "" then
		_temp_type = "AIRFIELD"
	else
		_temp_type = "OTHER"
	end
	
	return _operation .. _temp_type
end

-- ################################ TCP Connection ################################

Perun.ConnectToPerun = function ()
	-- Connects to Perun server
	Perun.AddLog("Connecting to TCP server",2)
	Perun.TCP = assert(Perun.socket.tcp())
	Perun.TCP:settimeout(5000)
	
	_, _err = Perun.TCP:connect(Perun.TCPPerunHost, Perun.TCPTargetPort)
	if _err then
		-- Could not connect
		Perun.AddLog("ERROR - TCP connection error : " .. _err , 1)
	else
		-- Connected
		Perun.AddLog("Success - connected to TCP server",2)
	end
	local _now = DCS.getRealTime()
	Perun.lastReconnect = _now
end

Perun.SendToPerun = function(data_id, data_package)
    -- Prepares and sends data package
    local _TempData={}
    _TempData["type"]=data_id
    _TempData["payload"]=data_package
    _TempData["timestamp"]=os.date('%Y-%m-%d %H:%M:%S')
	_TempData["instance"]=Perun.Instance
	
	-- Build TCP frame
    local _TCPFrame="<SOT>" .. stripChars(net.lua2json(_TempData)) .. "<EOT>"

    -- TCP Part - sending
	local _intStatus = nil
	local _intTries =0
	local _err=nil
	local _dropped =  true

	-- Try to send a few times (defind in settings section)
	local _now = DCS.getRealTime()
	while _intStatus == nil and _intTries < Perun.SendRetries do
		_intStatus, _err = Perun.TCP:send(_TCPFrame) 
		if _err then
			-- Failure, packet was not send
			Perun.AddLog("Packed not send : " .. data_id .. " , error: " .. _err .. ", tries: " .. _intTries,2)
			if _now > Perun.lastReconnect + Perun.ReconnectTimeout then
				Perun.ConnectToPerun()
			end
		else
			-- Succes, packet was send
			Perun.AddLog("Packet send : " .. data_id .. " , tries:" .. _intTries,2)
			_dropped = false
		end
		_intTries=_intTries+1
	end
	
	if _dropped == true then
		-- Add information to log file and send chat message to all that Perun connection is broken
		Perun.AddLog("ERROR - packed dropped : " .. data_id,1)
		if _now > Perun.lastConnectionError + Perun.ReconnectTimeout then
			-- Informs all players that there is Peron error; below hack for DCS net.send_chat not working
			local _all_players = net.get_player_list()
			for PlayerIDIndex, _playerID in ipairs(_all_players) do
				 net.send_chat_to(Perun.ConnectionError , _playerID)
			end
			-- Reset last error send counter
			Perun.lastConnectionError = _now
		end
	end 
end

-- ################################ Log functions ################################

Perun.LogChat = function(playerID,msg,all)
    -- Logs chat messages
    local _TempData={}
    _TempData['player']= net.get_player_info(playerID, "name")
    _TempData['msg']=stripChars(msg)
    _TempData['all']=all
    _TempData['ucid']=net.get_player_info(playerID, 'ucid')
    _TempData['datetime']=os.date('%Y-%m-%d %H:%M:%S')
    _TempData['missionhash']=Perun.MissionHash

	Perun.AddLog("Sending chat message",2)
    Perun.SendToPerun(50,_TempData)
end

Perun.LogEvent = function(log_type,log_content,log_arg_1,log_arg_2)
    -- Logs events messages
    local _TempData={}
    _TempData['log_type']= log_type
	_TempData['log_arg_1']= log_arg_1
	_TempData['log_arg_2']= log_arg_2
    _TempData['log_content']=log_content
    _TempData['log_datetime']=os.date('%Y-%m-%d %H:%M:%S')
    _TempData['log_missionhash']=Perun.MissionHash
	
	if log_arg_1 == nil then
		log_arg_1 = "null"
	end
	if log_arg_2 == nil then
		log_arg_2 = "null"
	end

	Perun.AddLog("Sending event data, event: " .. log_type .. ", arg1:" .. log_arg_1 .. ", arg2:" .. log_arg_2 .. ", content: " .. log_content,2)
    Perun.SendToPerun(51,_TempData)
end

Perun.LogStats = function(playerID)
    -- Log player status
	local _PlayerStatsTable=Perun.LogStatsGet(playerID)
    local _TempData={}
	_TempData['stat_data_perun']=_PlayerStatsTable
	_TempData['stat_data_type']= _PlayerStatsTable['ps_type'];
	_TempData['stat_data_masterslot'] = _PlayerStatsTable['ps_masterslot'];
    _TempData['stat_data_subslot'] = _PlayerStatsTable['ps_subslot'];
	_TempData['stat_ucid']=_PlayerStatsTable['ps_ucid'];
	_TempData['stat_name']=_PlayerStatsTable['ps_name'];
    _TempData['stat_datetime']=os.date('%Y-%m-%d %H:%M:%S')
    _TempData['stat_missionhash']=Perun.MissionHash

	Perun.AddLog("Sending stats data",2)
    Perun.SendToPerun(52,_TempData)
end

Perun.LogAllStats = function()
	-- Log all players data
	local _all_players = net.get_player_list()
	for PlayerIDIndex, _playerID in ipairs(_all_players) do
		if _playerID ~= 1 then
			Perun.LogStats(_playerID)
		end
	end
end

Perun.LogLogin = function(playerID)
    -- Player logged in
    local _TempData={}
    _TempData['login_ucid']=net.get_player_info(playerID, 'ucid')
    _TempData['login_ipaddr']=net.get_player_info(playerID, 'ipaddr')
    _TempData['login_name']=net.get_player_info(playerID, 'name')
    _TempData['login_datetime']=os.date('%Y-%m-%d %H:%M:%S')

	Perun.AddLog("Sending login event",2)
    Perun.SendToPerun(53,_TempData)
end

--- ################################ Calculate stats ################################
		
Perun.LogStatsCount = function(argPlayerID,argAction,argTimer)
	-- Creates or updates Perun statistics array
	local _player_hash=net.get_player_info(argPlayerID, 'ucid')..Perun.GetMulticrewParameter(argPlayerID,"subtype")
	
	-- By default we will be sending stats
	argTimer = argTimer or false
	
	if Perun.StatData[_player_hash] == nil then
		-- Create empty element
		 local _TempData={}
		_TempData['ps_ucid'] = net.get_player_info(argPlayerID, 'ucid')
		_TempData['ps_name'] = net.get_player_info(argPlayerID, 'name')
		_TempData['ps_type'] = Perun.GetMulticrewParameter(argPlayerID,"subtype")
		_TempData['ps_masterslot'] = Perun.GetMulticrewParameter(argPlayerID,"masterslot")
		_TempData['ps_subslot'] = Perun.GetMulticrewParameter(argPlayerID,"subslot")
		_TempData['ps_pvp'] = 0
		_TempData['ps_deaths'] = 0
		_TempData['ps_ejections'] = 0
		_TempData['ps_crashes'] = 0
		_TempData['ps_teamkills'] = 0
		_TempData['ps_time'] = 0
		_TempData['ps_kills_planes'] = 0
		_TempData['ps_kills_helicopters'] = 0
		_TempData['ps_kills_air_defense'] = 0
		_TempData['ps_kills_armor'] = 0
		_TempData['ps_kills_unarmed'] = 0
		_TempData['ps_kills_infantry'] = 0
		_TempData['ps_kills_ships'] = 0
		_TempData['ps_kills_fortification'] = 0
		_TempData['ps_kills_artillery'] = 0
		_TempData['ps_kills_other'] = 0
		_TempData['ps_airfield_takeoffs'] = 0
		_TempData['ps_airfield_landings'] = 0
		_TempData['ps_ship_takeoffs'] = 0
		_TempData['ps_ship_landings'] = 0
		_TempData['ps_farp_takeoffs'] = 0
		_TempData['ps_farp_landings'] = 0
		_TempData['ps_other_takeoffs'] = 0
		_TempData['ps_other_landings'] = 0

		Perun.StatData[_player_hash]=_TempData
	end
	
	if Perun.GetMulticrewParameter(argPlayerID,"subtype") ~= nil then
		Perun.StatDataLastType[net.get_player_info(argPlayerID, 'ucid')]=_player_hash
	else
		-- Do nothing
	end 
	
	if argAction == "eject" then
		Perun.StatData[_player_hash]['ps_ejections']=Perun.StatData[_player_hash]['ps_ejections']+1
	elseif  argAction == "pilot_death" then
		if DCS.getModelTime() > Perun.MissionStartNoDeathWindow then
			-- we do not track deaths during mission startup due to spawning issues
			Perun.StatData[_player_hash]['ps_deaths']=Perun.StatData[_player_hash]['ps_deaths']+1
		end
	elseif  argAction == "friendly_fire" then
		Perun.StatData[_player_hash]['ps_teamkills']=Perun.StatData[_player_hash]['ps_teamkills']+1
	elseif  argAction == "crash" then
		Perun.StatData[_player_hash]['ps_crashes']=Perun.StatData[_player_hash]['ps_crashes']+1
	elseif  argAction == "landing_FARP" then
		Perun.StatData[_player_hash]['ps_farp_landings']=Perun.StatData[_player_hash]['ps_farp_landings']+1
	elseif  argAction == "landing_SHIP" then
		Perun.StatData[_player_hash]['ps_ship_landings']=Perun.StatData[_player_hash]['ps_ship_landings']+1
	elseif  argAction == "landing_AIRFIELD" then
		Perun.StatData[_player_hash]['ps_airfield_landings']=Perun.StatData[_player_hash]['ps_airfield_landings']+1
	elseif  argAction == "tookoff_FARP" then
		Perun.StatData[_player_hash]['ps_farp_takeoffs']=Perun.StatData[_player_hash]['ps_farp_takeoffs']+1
	elseif  argAction == "tookoff_SHIP" then
		Perun.StatData[_player_hash]['ps_ship_takeoffs']=Perun.StatData[_player_hash]['ps_ship_takeoffs']+1
	elseif  argAction == "tookoff_AIRFIELD" then
		Perun.StatData[_player_hash]['ps_airfield_takeoffs']=Perun.StatData[_player_hash]['ps_airfield_takeoffs']+1
	elseif  argAction == "kill_Planes" then
		Perun.StatData[_player_hash]['ps_kills_planes']=Perun.StatData[_player_hash]['ps_kills_planes']+1
	elseif  argAction == "kill_Helicopters" then
		Perun.StatData[_player_hash]['ps_kills_helicopters']=Perun.StatData[_player_hash]['ps_kills_helicopters']+1
	elseif  argAction == "kill_Ships" then
		Perun.StatData[_player_hash]['ps_kills_ships']=Perun.StatData[_player_hash]['ps_kills_ships']+1
	elseif  argAction == "kill_Air_Defence" then
		Perun.StatData[_player_hash]['ps_kills_air_defense']=Perun.StatData[_player_hash]['ps_kills_air_defense']+1
	elseif  argAction == "kill_Unarmed" then
		Perun.StatData[_player_hash]['ps_kills_unarmed']=Perun.StatData[_player_hash]['ps_kills_unarmed']+1
	elseif  argAction == "kill_Armor" then
		Perun.StatData[_player_hash]['ps_kills_armor']=Perun.StatData[_player_hash]['ps_kills_armor']+1
	elseif  argAction == "kill_Infantry" then
		Perun.StatData[_player_hash]['ps_kills_infantry']=Perun.StatData[_player_hash]['ps_kills_infantry']+1
	elseif  argAction == "kill_Fortification" then
		Perun.StatData[_player_hash]['ps_kills_fortification']=Perun.StatData[_player_hash]['ps_kills_fortification']+1
	elseif  argAction == "kill_Artillery" then
		Perun.StatData[_player_hash]['ps_kills_artillery']=Perun.StatData[_player_hash]['ps_kills_artillery']+1	
	elseif  argAction == "kill_Other" then
		Perun.StatData[_player_hash]['ps_kills_other']=Perun.StatData[_player_hash]['ps_kills_other']+1
	elseif  argAction == "kill_PvP" then
		Perun.StatData[_player_hash]['ps_pvp']=Perun.StatData[_player_hash]['ps_pvp']+1
	elseif  argAction == "landing_OTHER" then
		Perun.StatData[_player_hash]['ps_other_landings']=Perun.StatData[_player_hash]['ps_other_landings']+1
	elseif  argAction == "tookoff_OTHER" then
		Perun.StatData[_player_hash]['ps_other_takeoffs']=Perun.StatData[_player_hash]['ps_other_takeoffs']+1
	elseif  argAction == "timer" then
		Perun.StatData[_player_hash]['ps_time']=Perun.StatData[_player_hash]['ps_time']+1
	end

	-- Always update slots
	Perun.StatData[_player_hash]['ps_masterslot'] = Perun.GetMulticrewParameter(argPlayerID,"masterslot")
	Perun.StatData[_player_hash]['ps_subslot'] = Perun.GetMulticrewParameter(argPlayerID,"subslot")
	
	Perun.AddLog("Stats data prepared",2)
	
	-- If this is timer request do not send data to database
	if argTimer ~= true then
		Perun.LogStats(argPlayerID);
	end
end

Perun.LogStatsCountCrew = function (MasterPilotID,ActionType)
	-- Change stats for the whole crew
	local _pilots_accounted = Perun.GetMulticrewCrew(MasterPilotID)
	for _, pilotID in ipairs(_pilots_accounted) do
		Perun.LogStatsCount(pilotID,ActionType)
	end
end
-- ################################ Data preparation ################################

Perun.LogStatsGet = function(playerID)
	-- Gets Perun statistics array per player 
	local next = next -- Make next function local - this improves performance
	local _player_hash = nil
	
	local _ucid = net.get_player_info(playerID, 'ucid')
	if not _ucid then
		_ucid=Perun.PlayersTableCache["p"..playerID].ucid
	end
	
	if next(Perun.StatDataLastType) == nil then
		-- Array is empty
		_player_hash=_ucid..Perun.GetMulticrewParameter(playerID,"subtype")
	elseif Perun.StatDataLastType[net.get_player_info(playerID, 'ucid')]== nil then
		-- Last type entry is empty
		_player_hash=_ucid..Perun.GetMulticrewParameter(playerID,"subtype")
	else
		-- Return last type entry
		_player_hash=Perun.StatDataLastType[_ucid]
	end
	
	local next = next -- Make next function local - this improves performance
	if  next(Perun.StatData) == nil then
		-- Array is empty
		Perun.LogStatsCount(playerID,'init') -- Init statistics
	end
	if Perun.StatData[_player_hash]== nil then
		-- Stats for player are empty
		Perun.LogStatsCount(playerID,'init') -- Init statistics
	end

	Perun.AddLog("Getting stats data",2)
	return Perun.StatData[_player_hash];
end

Perun.UpdateStatus = function()
 -- Main function for status updates

    -- Diagnostic data
		-- Update version data
		Perun.ServerData['v_dcs_hook']=Perun.Version

		-- Update clients data - count connected players
		_playerlist=net.get_player_list()
		_count = 0
		for _ in pairs(_playerlist) do _count = _count + 1 end
		Perun.ServerData['c_players']=_count

		-- Send
		Perun.AddLog("Sending server data",2)
		Perun.SendToPerun(1,Perun.ServerData)

    -- Status data - update all subsections
		-- 1 - Mission data
		local _TempData={}
		_TempData['name']=DCS.getMissionName()
		_TempData['modeltime']=DCS.getModelTime()
		_TempData['realtime']=DCS.getRealTime()
		_TempData['pause']=DCS.getPause()
		_TempData['multiplayer']=DCS.isMultiplayer()
		_TempData['theatre'] = Perun.MissionData['mission']['theatre']
		_TempData['weather'] = Perun.MissionData['mission']['weather']
		Perun.StatusData["mission"] = _TempData

		-- 2 - Players data
		_PlayersTable={}
		for _k, _i in ipairs(_playerlist) do
			_PlayersTable[_k]=net.get_player_info(_i)
		end
		Perun.StatusData["players"] = _PlayersTable

		-- Send
		Perun.AddLog("Sending status data",2)
		Perun.SendToPerun(2,Perun.StatusData)
end

Perun.UpdateSlots = function()
	-- Update and send slots data

	Perun.SlotsData['coalitions']=DCS.getAvailableCoalitions()
	Perun.SlotsData['slots']={}

	-- Build up slot table
	for _j, _i in pairs(Perun.SlotsData['coalitions']) do
		Perun.SlotsData['slots'][_j]=DCS.getAvailableSlots(_j)
		
		for _sj, _si in pairs(Perun.SlotsData['slots'][_j]) do
			Perun.SlotsData['slots'][_j][_sj]['countryName']= nil
			Perun.SlotsData['slots'][_j][_sj]['onboard_num']= nil
			Perun.SlotsData['slots'][_j][_sj]['groupSize']= nil
			Perun.SlotsData['slots'][_j][_sj]['groupName']= nil
			Perun.SlotsData['slots'][_j][_sj]['callsign']= nil
			Perun.SlotsData['slots'][_j][_sj]['task']= nil
			Perun.SlotsData['slots'][_j][_sj]['airdromeId']= nil
			Perun.SlotsData['slots'][_j][_sj]['helipadName']= nil
		end
	end

	Perun.AddLog("Sending slots data",2)
	Perun.SendToPerun(3,Perun.SlotsData)
end

Perun.UpdateMission = function()
    -- Main function for mission information updates
	Perun.AddLog("Updating mission data",2)
	
	-- Get mission information
    Perun.MissionData=DCS.getCurrentMission()

	Perun.AddLog("Mission data updated",2)
end

--- ################################ Event callbacks ################################

Perun.onSimulationStart = function()
	-- Simulation was started
    Perun.MissionHash=Perun.GenerateMissionHash()
    Perun.LogEvent("SimStart","Mission " .. Perun.MissionHash .. " started",nil,nil);
	Perun.StatData = {}
	Perun.StatDataLastType = {}
	Perun.PlayersTableCache = {}
end

Perun.onSimulationStop = function()
	-- Simulation was stopped
    Perun.LogEvent("SimStop","Mission " .. Perun.MissionHash .. " finished",nil,nil);
	Perun.MissionHash=Perun.GenerateMissionHash();
	Perun.StatData = {}
	Perun.StatDataLastType = {}
	Perun.LogAllStats()
	Perun.PlayersTableCache = {}
end

Perun.onPlayerDisconnect = function(id, err_code)
	-- Player disconnected
	Perun.LogEvent("disconnect", "Player " .. id .. " disconnected.(?)",nil,nil);
	
	return 
end

Perun.onPlayerStop = function (id)
    -- Player left the simulation (happens right before a disconnect, if player exited by desire)
	Perun.LogEvent("quit", "Player " .. id .. " quit the server.",nil,nil);
	return
end

Perun.onSimulationFrame = function()
	-- Repeat for each simulator frame
    local _now = DCS.getRealTime()

    -- First run
	if Perun.lastSentMission == 0 and Perun.lastSentStatus == 0 then
		Perun.lastSentMission = _now
		Perun.UpdateMission()
		Perun.UpdateSlots()
    end

    -- Send mission update and update JSON
    if _now > Perun.lastSentMission + Perun.RefreshMission then
        Perun.lastSentMission = _now

        Perun.UpdateMission()
    end

    -- Send status update
    if _now > Perun.lastSentStatus + Perun.RefreshStatus then
        Perun.lastSentStatus = _now

        Perun.UpdateStatus() 
    end
	
	-- Send keepalive
	if _now > Perun.lastSentKeepAlive + Perun.RefreshKeepAlive then 
		Perun.lastSentKeepAlive = _now
		Perun.SendToPerun(0,nil) 
	end
	
	-- Calucalate time on slot per each of players
	if _now > Perun.lastTimer + 60 then
		Perun.lastTimer = _now;
		local _all_players = net.get_player_list()
		for PlayerIDIndex, _playerID in ipairs(_all_players) do
			if _playerID ~= 1 then
				Perun.LogStatsCount(_playerID,"timer",true)
			end
		end
	end
	
end

Perun.onPlayerStart = function (id)
	-- Player entered cocpit
    net.send_chat_to(Perun.MOTD_L1, id);
    net.send_chat_to(Perun.MOTD_L2, id);
end

Perun.onPlayerTrySendChat = function (playerID, msg, all)
	-- Somebody tries to send chat message
    if msg~=Perun.MOTD_L1 and msg~=Perun.MOTD_L2 and msg~=Perun.ConnectionError then
        Perun.LogChat(playerID,msg,all)
    end

    return msg
end

Perun.onGameEvent = function (eventName,arg1,arg2,arg3,arg4,arg5,arg6,arg7)
	-- Game event has occured
	Perun.AddLog("Event handler for ".. eventName .. " started",2)
	
    if eventName == "friendly_fire" then
        --"friendly_fire", playerID, weaponName, victimPlayerID
		if arg2 == "" then
			arg2 = "Cannon"
		end
		
        Perun.LogEvent(eventName,Perun.SideID2Name( net.get_player_info(arg1, "side")) .. " player(s) " .. Perun.GetMulticrewCrewNames(arg1) .." killed friendly " .. Perun.GetMulticrewCrewNames(arg3) .. " using " .. arg2,nil,nil);

    elseif eventName == "mission_end" then
        --"mission_end", winner, msg
        Perun.LogEvent(eventName,"Mission finished, winner " .. arg1 .. " message: " .. arg2,nil,nil);

    elseif eventName == "kill" then
        --"kill", killerPlayerID, killerUnitType, killerSide, victimPlayerID, victimUnitType, victimSide, weaponName
		local _temp_victims=""
		if net.get_player_info(arg4, "name") ~= nil then
            _temp_victims = " player(s) ".. Perun.GetMulticrewCrewNames(arg4) .. " ";
            
			Perun.LogStats(arg4);
        else
            _temp_victims = " AI ";
        end

        if net.get_player_info(arg1, "name") ~= nil then
			_temp_killers = " player(s) ".. Perun.GetMulticrewCrewNames(arg1) .." ";
			
			_temp_event_type=""
			if arg3 ~= arg6 then
				_temp_category=Perun.GetCategory(arg5)
				
				if _temp_category == "Planes" then
					_temp_event_type="kill_Planes"
				elseif _temp_category == "Helicopters" then
					_temp_event_type="kill_Helicopters"
				elseif _temp_category == "Ships" then
					_temp_event_type="kill_Ships"
				elseif _temp_category == "Air Defence" then
					_temp_event_type="kill_Air_Defence"
				elseif _temp_category == "Unarmed" then
					_temp_event_type="kill_Unarmed"
				elseif _temp_category == "Armor" then
					_temp_event_type="kill_Armor"
				elseif _temp_category == "Infantry" then
					_temp_event_type="kill_Infantry"
				elseif _temp_category == "Fortification" then
					_temp_event_type="kill_Fortification"
				elseif _temp_category == "Artillery" or _temp_category == "MissilesSS" then
					_temp_event_type="kill_Artillery"
				else 
					_temp_event_type="kill_Other"
				end
				if net.get_player_info(arg4, "name") ~= nil and arg3 ~= arg6 then
					Perun.LogStatsCountCrew (arg1,"kill_PvP")
				end
			else
				_temp_event_type="friendly_fire"
			end
			
			Perun.LogStatsCountCrew (arg1,_temp_event_type)
			
        else
            _temp_killers = " AI ";
        end
		
		if arg7 == "" then
			arg7 = "Cannon"
		end
		
		local victim_vehicle = arg5
		if victim_vehicle == "" then
			victim_vehicle = "?"
		end
		
		Perun.LogEvent(eventName,Perun.SideID2Name(arg3) .. _temp_killers .. " in " .. arg2 .. " killed " .. Perun.SideID2Name(arg6) .. _temp_victims .. " in " .. victim_vehicle  .. " using " .. arg7 .. " [".. Perun.GetCategory(arg5).."]",arg7,Perun.GetCategory(arg5));

    elseif eventName == "self_kill" then
        --"self_kill", playerID
		Perun.LogStats(arg1);
        Perun.LogEvent(eventName,net.get_player_info(arg1, "name") .. " killed himself",nil,nil);

    elseif eventName == "change_slot" then
        --"change_slot", playerID, slotID, prevSide
		
		_master_type,_master_slot,_sub_slot = Perun.GetMulticrewAllParameters(arg1)
		if _sub_slot == nil then
			_sub_slot =""
		else
			_sub_slot =" (" .. _sub_slot .. ")  "
		end       
	    Perun.LogEvent(eventName,Perun.SideID2Name( net.get_player_info(arg1, "side")) .. " player " .. net.get_player_info(arg1, "name") .. " changed slot to " .. _master_type .. " " .. _sub_slot,nil,nil);
       	
		Perun.LogStats(arg1);
		Perun.LogStatsCount(arg1,"init")
		Perun.PlayersTableCache["p"..arg1]=net.get_player_info(arg1);

    elseif eventName == "connect" then
        --"connect", playerID, name
        Perun.LogLogin(arg1);
        Perun.LogEvent(eventName,"Player "..net.get_player_info(arg1, "name") .. " connected",nil,nil);
		Perun.PlayersTableCache["p"..arg1]=net.get_player_info(arg1);

    elseif eventName == "disconnect" then
        --"disconnect", playerID, name, playerSide, reason_code
        Perun.LogEvent(eventName,"Player " ..  arg2 .. " disconnected (".. arg4 .. ")." ,arg4,nil);
		Perun.LogStats(arg1);

    elseif eventName == "crash" then
        --"crash", playerID, unit_missionID
		Perun.LogStatsCountCrew (arg1,"crash")
		Perun.LogEvent(eventName, Perun.SideID2Name( net.get_player_info(arg1, "side")) .. " player(s) " .. Perun.GetMulticrewCrewNames(arg1)  .. " crashed in " .. DCS.getUnitType(arg2),nil,nil);

    elseif eventName == "eject" then
        --"eject", playerID, unit_missionID
		Perun.LogStatsCountCrew (arg1,"eject") -- TBD crew or initiator only?
		Perun.LogEvent(eventName, Perun.SideID2Name( net.get_player_info(arg1, "side")) .. " player(s) " .. Perun.GetMulticrewCrewNames(arg1) .. " ejected " .. DCS.getUnitType(arg2),nil,nil);

    elseif eventName == "takeoff" then
        --"takeoff", playerID, unit_missionID, airdromeName
        if arg3 ~= "" then
            _temp_airfield = " from " .. arg3;
        else
            _temp_airfield = "";
        end

		Perun.LogStatsCountCrew (arg1,Perun.GetTakeOffLandingEvent(true,arg3))
		Perun.LogEvent(eventName, Perun.SideID2Name( net.get_player_info(arg1, "side")) .. " player(s) " .. Perun.GetMulticrewCrewNames(arg1) .. " took off in ".. DCS.getUnitType(arg2) .. _temp_airfield,arg3,nil);

    elseif eventName == "landing" then
        --"landing", playerID, unit_missionID, airdromeName
        if arg3 ~= "" then
            _temp_airfield = " at " .. arg3;
        else
            _temp_airfield ="";
        end

		Perun.LogStatsCountCrew (arg1,Perun.GetTakeOffLandingEvent(false,arg3))
		Perun.LogEvent(eventName, Perun.SideID2Name( net.get_player_info(arg1, "side")) .. " player(s) " .. Perun.GetMulticrewCrewNames(arg1) .. " landed in " .. DCS.getUnitType(arg2).. _temp_airfield,arg3,nil);

    elseif eventName == "pilot_death" then
        --"pilot_death", playerID, unit_missionID
		Perun.LogStatsCountCrew (arg1,"pilot_death")  -- TBD crew or initiator only?
		Perun.LogEvent(eventName, Perun.SideID2Name( net.get_player_info(arg1, "side")) .. " player(s) " .. Perun.GetMulticrewCrewNames(arg1) .. " in " .. DCS.getUnitType(arg2) .. " died",nil,nil);

    else
        Perun.LogEvent(eventName,"Unknown event type",nil,nil);

    end
	Perun.AddLog("Event handler for " .. eventName .. " finished",2)
end

-- ########### Finalize and set callbacks ###########
if Perun.IsServer then
	-- If this game instance is hosting multiplayer game, start Perun
	Perun.MissionHash=Perun.GenerateMissionHash()														-- Generate initial missionhash
	DCS.setUserCallbacks(Perun)																			-- Set user callbacs,  map DCS event handlers with functions defined above
	net.log("Loaded - Perun - VladMordock: " .. Perun.Version )											-- Display perun information in log
	Perun.ConnectToPerun()																				-- Connect to Perun server
end
