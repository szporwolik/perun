-- Perun for DCS World https://github.com/szporwolik/perun -> DCS Hook component

-- Initial init
	local Perun = {}
	package.path  = package.path..";"..lfs.currentdir().."/LuaSocket/?.lua"
	package.cpath = package.cpath..";"..lfs.currentdir().."/LuaSocket/?.dll"
	
-- ########### SETTINGS ###########
	
	Perun.RefreshStatus = 15 												-- base refresh rate in seconds to send status update (values lower than 60 may affect performance!)
	Perun.RefreshMission = 60 												-- refresh rate in seconds to send mission information  (values lower than 60 may affect performance!)
	Perun.JsonLocation = "Scripts\\Json\\perun_export.json" 				-- relatve do user's SaveGames DCS folder
	Perun.UDPTargetPort = 48620												-- UDP port to send data to
	Perun.MOTD_L1 = "Witamy na serwerze Gildia.org !"						-- Message send to players connecting the server - Line 1
	Perun.MOTD_L2 = "Wymagamy obecnosci na 255Mhz (DCS SRS)"				-- Message send to players connecting the server - Line 2
	
-- ########### END OF SETTINGS ###########

-- Variable init
	Perun.Version = "v0.3.6"
	Perun.StatusData = {}
	Perun.SlotsData = {}
	Perun.MissionData = {}
	Perun.VersionData = {}
	Perun.MissionHash=""
	Perun.lastSentStatus =0
	Perun.lastSentMission =0
	Perun.JsonLocation = lfs.writedir() .. Perun.JsonLocation
	Perun.socket  = require("socket")
	Perun.UDP = assert(Perun.socket.udp())
	Perun.UDP:settimeout(0)
	Perun.UDP:setsockname("*", 48621)
	Perun.UDP:setpeername("127.0.0.1",Perun.UDPTargetPort)

-- Function definition
	Perun.GetCategory = function(id)
	    -- via https://pastebin.com/GUAXrd2U
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
		-- Helper function
		local sides = {
			[0] = 'SPECTATOR',
			[1] = 'RED',
			[2] = 'BLUE',
		}
		return sides[id]
	end
	
	Perun.AddLog = function(text)
		-- Adds logs to DCS.log file
		net.log("Perun : ".. text)
	end

	Perun.UpdateJson = function()
		-- Updates main json file
		TempData={}
		TempData["1"]=Perun.VersionData
		TempData["2"]=Perun.StatusData
		TempData["3"]=Perun.SlotsData
		TempData["4"]=Perun.MissionData
		TempData["debug"]=net.get_player_list()
		
		io.open(Perun.JsonLocation,"w"):close()
		perun_export = io.open(Perun.JsonLocation, "w")
		perun_export:write(net.lua2json(TempData) .. "\n")
		perun_export:close()
	end

	Perun.Send = function(data_id, data_package)
		-- Sends data package
		TempData={}
		TempData["type"]=data_id
		TempData["payload"]=data_package
		
		temp=net.lua2json(TempData)
		Perun.UDP:send(temp)
		Perun.AddLog("Packet send")
	end

	Perun.UpdateStatusPart = function(part_id, data_package)
		-- Helper for status update container
		Perun.StatusData[part_id] = data_package
	end

	Perun.UpdateVersion = function()
		-- Main function for debug/version information
		
		-- Update Mission data
			Perun.VersionData['v_dcs_hook']=Perun.Version
			
		-- Send
			Perun.Send(1,Perun.VersionData)
	end
	
	Perun.UpdateStatus = function()
		-- Main function for status updates
		
		-- Update all subsections
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
					for _, i in pairs(_temp) do
						_temp2[i]=net.get_player_info(i)
					end
				Perun.UpdateStatusPart("players",_temp2)	
		
		-- Send
			Perun.Send(2,Perun.StatusData)
	end

	Perun.UpdateSlots = function()
		-- Main function for slot

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

		-- Update Mission data
			Perun.MissionData=DCS.getCurrentMission()

		-- Send
			-- Perun.Send(4,Perun.MissionData)
	end
	
	Perun.LogChat = function(playerID,msg,all)
		-- Log chat messages
		
		data={}
		data['player']= net.get_player_info(playerID, "name")
		data['msg']=msg
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

--- Event callbacks

	Perun.onSimulationStart = function()
		Perun.MissionHash=DCS.getMissionName( ).."@"..os.date('%Y%m%d_%H%M%S');
		Perun.LogEvent("SimStart","Mission" .. Perun.MissionHash .. " started");
	end

	Perun.onSimulationStop = function()
		Perun.LogEvent("SimStop","Mission" .. Perun.MissionHash .. " finished");
	end
	
	Perun.onPlayerDisconnect= function(id, err_code)
		Perun.LogEvent("disconnect", "Player " .. net.get_player_info(id, "name") .. " disconnected; " .. err_code);
	end

	Perun.onSimulationFrame = function()
		local _now = DCS.getRealTime()
		
		-- Send status update
		if _now > Perun.lastSentStatus + Perun.RefreshStatus then
			Perun.lastSentStatus = _now 

			Perun.UpdateStatus()
			Perun.UpdateJson()
		end
		
		-- Send mission update
		if _now > Perun.lastSentMission + Perun.RefreshMission then
			Perun.lastSentMission = _now 
			
			Perun.UpdateSlots()
			Perun.UpdateMission()
			
			Perun.UpdateJson()
		end
		
	end

	Perun.onMissionLoadEnd = function()
		Perun.UpdateMission()
		Perun.UpdateSlots()
		Perun.UpdateVersion()
		Perun.UpdateJson()
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

-- Finalize and set callbacks 
	DCS.setUserCallbacks(Perun)
	net.log("Loaded - Perun - VladMordock - " .. Perun.Version )