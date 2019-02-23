-- Perun for DCS World https://github.com/szporwolik/perun -> DCS Hook component

-- Initial init
	local Perun = {}
	package.path  = package.path..";"..lfs.currentdir().."/LuaSocket/?.lua"
	package.cpath = package.cpath..";"..lfs.currentdir().."/LuaSocket/?.dll"
	
-- ########### SETTINGS ###########
	
	Perun.Refresh = 15 											-- base refresh rate in secounds (values lower than 60 may affect performance!)
	Perun.JsonLocation = "Scripts\\Json\\perun_export.json" 	-- relatve do user's SaveGames DCS folder
	Perun.UDPTargetPort = 48620									-- UDP port to send data to
	
-- ########### END OF SETTINGS ###########

-- Variable init
	Perun.Version = "v0.1.1"
	Perun.StatusData = {}
	Perun.MissionData = {}
	Perun.VersionData = {}
	Perun.lastSent =0
	Perun.JsonLocation = lfs.writedir() .. Perun.JsonLocation
	Perun.socket  = require("socket")
	Perun.UDP = assert(Perun.socket.udp())
	Perun.UDP:settimeout(0)
	Perun.UDP:setsockname("*", 48621)
	Perun.UDP:setpeername("127.0.0.1",Perun.UDPTargetPort)
	Perun.UDP:send("{'type':999,'payload':'Perun is onlne!'}")	-- send welcome message

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
		TempData["3"]=Perun.MissionData
		
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
					temp['filename']=DCS.getMissionFilename()
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
			Perun.MissionData['name']=DCS.getMissionName()
			Perun.MissionData['description']=DCS.getMissionDescription()
			Perun.MissionData['filename']=DCS.getMissionFilename()
			Perun.MissionData['modeltime']=DCS.getModelTime()
			Perun.MissionData['realtime']=DCS.getRealTime()
			Perun.MissionData['data']=DCS.getCurrentMission()
			Perun.MissionData['coalitions']=DCS.getAvailableCoalitions()
			Perun.MissionData['slots']={}
			
			for j, i in pairs(Perun.MissionData['coalitions']) do
				Perun.MissionData['slots'][j]=DCS.getAvailableSlots(j) 
			end
		-- Send
			Perun.Send(3,Perun.MissionData)
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

-- Finalize and set callbacks 
	DCS.setUserCallbacks(Perun)
	net.log("Loaded - Perun - VladMordock - " .. Perun.Version )