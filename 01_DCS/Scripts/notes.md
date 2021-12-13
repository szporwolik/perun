

|call              |return              |  meaning          |  
|--------          |----                |----                        |  
|`DCS.getAvailableSlots("blue")`|TBD|slots available for a side|
|`net.get_player_list()`|[1, 2]|players in the mission. As in, occupied player slots in the server|
|||
|||
|`DCS.getRealTime()` |	`37.5899104`    |  seconds.micros after mission was started  |
|`DCS.getModelTime()`|`37.413`| 



Callbacks:  

|callback|event||  
|----|----|----|  
|onMissionLoadBegin|||  
|onMissionLoadProgress|||   
|onMissionLoadEnd, |||  
|onSimulationStart, |||  
|onSimulationStop, |||  
|onSimulationFrame, |||  
|onSimulationPause, |||  
|onSimulationResume, |||  
|onGameEvent, |||  
|onNetConnect, |||  
|onNetMissionChanged,|||   
|onNetConnect, |||  
|onNetDisconnect, |||  
|onPlayerTryConnect, |||  
|onPlayerConnect, |_connect||  
|onPlayerDisconnect, |disconnect||  
|onPlayerStart, |||  
|onPlayerStop, |||  
|onPlayerTrySendChat, |||  
|onPlayerTryChangeSlot|||  
|onPlayerChangeSlot,|change_slot||   


# Q's:
Is unit ID tracking needed?

# ToDos:
* file per mission hash
* start of mission cpp callback?
* hash as argument
* end as callback
* investigate DCS namespace (for pairs?)

* file creation and writing to file should only happen when there is something to be written

* mission update should happen every X seconds, only
* connection should only affect message to players, nothing else!
* detect monotonic time flow - back in time? new mission.