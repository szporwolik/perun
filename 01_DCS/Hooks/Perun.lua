-- Perun for DCS World https://github.com/szporwolik/perun -> DCS Hook component

-- Initial init
	local Perun = {}
	package.path  = package.path..";"..lfs.currentdir().."/LuaSocket/?.lua"
	package.cpath = package.cpath..";"..lfs.currentdir().."/LuaSocket/?.dll"
	
-- ########### SETTINGS ###########
	
	Perun.Refresh = 15 														-- base refresh rate in secounds (values lower than 60 may affect performance!)
	Perun.JsonLocation = "Scripts\\Json\\perun_export.json" 				-- relatve do user's SaveGames DCS folder
	Perun.UDPTargetPort = 48620												-- UDP port to send data to
	Perun.MOTD_L1 = "Witamy na serwerze Gildia.org !"						-- Message send to players connecting the server - Line 1
	Perun.MOTD_L2 = "Wymagamy obecnosci na 255Mhz (DCS SRS)"				-- Message send to players connecting the server - Line 2
	
-- ########### END OF SETTINGS ###########

-- Variable init
	Perun.Version = "v0.3.0"
	Perun.StatusData = {}
	Perun.SlotsData = {}
	Perun.VersionData = {}
	Perun.lastSent =0
	Perun.JsonLocation = lfs.writedir() .. Perun.JsonLocation
	Perun.socket  = require("socket")
	Perun.UDP = assert(Perun.socket.udp())
	Perun.UDP:settimeout(0)
	Perun.UDP:setsockname("*", 48621)
	Perun.UDP:setpeername("127.0.0.1",Perun.UDPTargetPort)

-- Function definition
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
				Perun.UpdateStatusPart("mission",temp)
				
			-- 2 - Players
					temp = net.get_player_list()
					for _, i in ipairs(temp) do
						temp[i]=net.get_player_info(i)
					end
				Perun.UpdateStatusPart("players",temp)	
		
		-- Send
			Perun.Send(2,Perun.StatusData)
	end

	Perun.UpdateMission = function()
		-- Main function for mission information updates

		-- Update Mission data
			Perun.SlotsData['coalitions']=DCS.getAvailableCoalitions()
			Perun.SlotsData['slots']={}
			
			for j, i in pairs(Perun.SlotsData['coalitions']) do
				Perun.SlotsData['slots'][j]=DCS.getAvailableSlots(j) 
			end
		-- Send
			Perun.Send(3,Perun.SlotsData)
	end
	
	Perun.LogChat = function(playerID,msg,all)
		-- Log chat messages
		
		data={}
		data['player']= net.get_player_info(playerID, "name")
		data['msg']=msg
		data['all']=all
		
		Perun.Send(50,data)
	end
	
	Perun.LogEvent = function(log_type,log_content)
		-- Log chat messages
		
		data={}
		data['log_type']= log_type
		data['log_content']=log_content
		
		Perun.Send(51,data)
	end

--- Event callbacks
	Perun.onSimulationFrame = function()
		local _now = DCS.getRealTime()

		if _now > Perun.lastSent + Perun.Refresh then
			Perun.lastSent = _now 
			
			Perun.UpdateMission()
			Perun.UpdateStatus()
			Perun.UpdateVersion()
			Perun.UpdateJson()
		end

	end

	Perun.onMissionLoadEnd = function()
		Perun.UpdateMission()
		Perun.UpdateJson()
	end
	
	Perun.onPlayerStart = function (id)
		net.send_chat(Perun.MOTD_L1, id);
		net.send_chat(Perun.MOTD_L2, id);
	end

	Perun.onPlayerTrySendChat = function (playerID, msg, all)
		Perun.LogChat(playerID,msg,all)
		return msg
	end
	
	Perun.onGameEvent = function (eventName,arg1,arg2,arg3,arg4)
	
			
		if eventName == "friendly_fire" then
		    --"friendly_fire", playerID, weaponName, victimPlayerID
			Perun.LogEvent(eventName,net.get_player_info(arg1, "name").." killed " .. net.get_player_info(arg3, "name") .. " using " .. arg2);
			
		elseif eventName == "mission_end" then
		    --"mission_end", winner, msg
			Perun.LogEvent(eventName,"Mission end, winner " .. arg1 .. " message: " .. arg2);
			
		elseif eventName == "kill" then
			--"kill", killerPlayerID, killerUnitType, killerSide, victimPlayerID, victimUnitType, victimSide, weaponName		
			Perun.LogEvent(eventName,net.get_player_info(arg1, "name").. " in " .. arg3 .. " " .. arg2 .. " killed " .. net.get_player_info(arg4, "name") .. " in " .. arg6 .. " " .. arg5 .. " using " .. arg7);
			
		elseif eventName == "self_kill" then
			--"self_kill", playerID	
			Perun.LogEvent(eventName,net.get_player_info(arg1, "name") .. " killed himself");
			
		elseif eventName == "change_slot" then
			--"change_slot", playerID, slotID, prevSide	
			Perun.LogEvent(eventName,net.get_player_info(arg1, "name") .. " changed slot to " .. arg2);
			
		elseif eventName == "connect" then
		    --"connect", playerID, name
			Perun.LogEvent(eventName,net.get_player_info(arg1, "name") .. " connected");
			
		elseif eventName == "disconnect" then
		    --"disconnect", playerID, name, playerSide, reason_code
			Perun.LogEvent(eventName,net.get_player_info(arg1, "name") .. " disconnected");
			
		elseif eventName == "crash" then
		    --"crash", playerID, unit_missionID
			Perun.LogEvent(eventName,net.get_player_info(arg1, "name") .. " crashed");
			
		elseif eventName == "eject" then
		    --"eject", playerID, unit_missionID
			Perun.LogEvent(eventName,net.get_player_info(arg1, "name") .. " ejected");
			
		elseif eventName == "takeoff" then
		    --"takeoff", playerID, unit_missionID, airdromeName
			Perun.LogEvent(eventName,net.get_player_info(arg1, "name") .. " took off from " .. arg3);
			
		elseif eventName == "landing" then
		    --"landing", playerID, unit_missionID, airdromeName
			Perun.LogEvent(eventName,net.get_player_info(arg1, "name") .. " landed at " .. arg3);
			
		elseif eventName == "pilot_death" then
		    --"pilot_death", playerID, unit_missionID
			Perun.LogEvent(eventName,net.get_player_info(arg1, "name") .. " died");
			
		else
			Perun.LogEvent(eventName,"Unknown event type");
		end
		
	end

-- Finalize and set callbacks 
	DCS.setUserCallbacks(Perun)
	net.log("Loaded - Perun - VladMordock - " .. Perun.Version )