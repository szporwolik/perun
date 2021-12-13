local explorer = {}


explorer.onMissionLoadBegin = function()
    net.log("[Explorer]", "on mission load begin");
end
explorer.onMissionLoadProgress = function(progress, message)
    net.log("[Explorer]", "on mission load progress", progress, message);
end
explorer.onMissionLoadEnd = function()
    net.log("[Explorer]", "on mission load end");
end
explorer.onSimulationStart = function()
    net.log("[Explorer]", "on simulation start");
end
explorer.onSimulationStop = function()
    net.log("[Explorer]", "on simulation stop");
end
explorer.onSimulationFrame = function()  end
explorer.onSimulationPause= function()
    net.log("[Explorer]", "on simulation pause");
end
explorer.onSimulationResume= function()
    net.log("[Explorer]", "on simulation resume");
end

explorer.onNetConnect= function()
    et.log("[Explorer]", "onNetConnect");
end
explorer.onNetMissionChanged= function()
    net.log("[Explorer]", "onNetMissionChanged");
end
explorer.onNetConnect= function()
    net.log("[Explorer]", "onNetConnect");
end
explorer.onNetDisconnect= function()
    net.log("[Explorer]", "onNetDisconnect");
end
explorer.onPlayerConnect= function()
    net.log("[Explorer]", "onPlayerConnect");
end
explorer.onPlayerDisconnect= function()
    net.log("[Explorer]", "onPlayerDisconnect");
end
explorer.onPlayerStart= function()
    net.log("[Explorer]", "onPlayerStart");
end
explorer.onPlayerStop= function()
    net.log("[Explorer]", "onPlayerStop");
end
explorer.onPlayerChangeSlot= function()
    net.log("[Explorer]", "onPlayerChangeSlot");
end
explorer.onPlayerTryConnect= function(address, name, ucid, id)
    net.log("[Explorer]", "onPlayerTryConnect", address, name, ucid, id);
end
explorer.onPlayerTrySendChat= function()
    net.log("[Explorer]", "onPlayerTrySendChat");
end
explorer.onPlayerTryChangeSlot= function()
    net.log("[Explorer]", "onPlayerTryChangeSlot");
end

explorer.onGameEvent = function(eventName, arg1, arg2, arg3, arg4, arg5, arg6, arg7)

    local payload = {}
    if(argCount == 1) then
        payload = handler(arg1);
    elseif argCount == 2 then
        payload = handler(arg1, arg2);
    elseif argCount == 3 then
        payload = handler(arg1, arg2, arg3);
    elseif argCount == 4 then
        payload = handler(arg1, arg2, arg3, arg4);
    elseif argCount == 7 then
        payload = handler(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
    end

    local json = net.lua2json(payload);
    net.log("[Explorer]", json);

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


if DCS.isServer() then
    net.log("[Explorer]", "Initialising API explorer");
    DCS.setUserCallbacks(explorer);
end