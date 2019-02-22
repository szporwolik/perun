-- Perun hook by VladMordock

local Perun = {}

Perun.Version = "0.1"
Perun.Refresh = 15
Perun.StatusData = {}
Perun.MissionData = {}

local _lastSent = 0;

local json_path = lfs.writedir() .."Scripts\\Json\\perun_export.json"

package.path  = package.path..";"..lfs.currentdir().."/LuaSocket/?.lua"
package.cpath = package.cpath..";"..lfs.currentdir().."/LuaSocket/?.dll"

socket  = require("socket")
Perun.UDP = assert(socket.udp())
Perun.UDP:settimeout(0)
Perun.UDP:setsockname("*", 48621)
Perun.UDP:setpeername("127.0.0.1",48620)

Perun.UDP:send("Perun online!")
	
Perun.AddLog = function(text)
	net.log("Perun : ".. text)
end

Perun.UpdateJson = function()
	TempData={}
	TempData["1"]=Perun.StatusData
	TempData["2"]=Perun.MissionData
	
	io.open(json_path,"w"):close()
	perun_export = io.open(json_path, "w")
	perun_export:write(net.lua2json(TempData) .. "\n")
	perun_export:close()
end

Perun.Send = function(data_id, data_package)
	TempData={}
	TempData["type"]=data_id
	TempData["payload"]=Perun.StatusData
	
	temp=net.lua2json(TempData)
	Perun.UDP:send(temp)
	Perun.AddLog("Packet send")
end

Perun.UpdateStatusPart = function(part_id, data_package)
	Perun.StatusData[part_id] = data_package
end

Perun.UpdateStatus = function()
	-- 1 - Update time
		Perun.UpdateStatusPart("updated","TODO: OS TIME")
		
	-- 2 - Mission
			temp={}
			temp['name']=DCS.getMissionName()
			temp['filename']=DCS.getMissionFilename()
			temp['modeltime']=DCS.getModelTime()
			temp['realtime']=DCS.getRealTime()
			temp['pause']=DCS.getPause()
			temp['multiplayer']=DCS.isMultiplayer()
		Perun.UpdateStatusPart("mission",temp)
		
	-- 3 - Players
			temp = net.get_player_list()
			for _, i in ipairs(temp) do
				temp[i]=net.get_player_info(i)
			end
		Perun.UpdateStatusPart("players",temp)	
	
	-- Send
		Perun.Send(1,Perun.StatusData)
end

Perun.UpdateMission = function()
	-- Mission data
		Perun.MissionData['updated']="TODO: OS TIME"
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
		Perun.Send(2,Perun.MissionData)
end


--- Event callback

Perun.onSimulationFrame = function()
    local _now = DCS.getRealTime()

    if _now > _lastSent + Perun.Refresh then
        _lastSent = _now 
		
		Perun.UpdateMission()
		Perun.UpdateStatus()
		Perun.UpdateJson()
    end

end

Perun.onMissionLoadEnd = function()
	Perun.UpdateMission()
	Perun.UpdateJson()
end

--- Finalize and set callbacks 

DCS.setUserCallbacks(Perun)
net.log("Loaded - Perun - VladMordock - " .. Perun.Version )