-- Perun for DCS World https://github.com/szporwolik/perun -> DCS Hook config component
local PerunConfig = {}

-- ###################### SETTINGS - DO NOT MODIFY OUTSIDE THIS SECTION #############################

PerunConfig.TCPPerunHost = "localhost"														    -- (string) [default: "localhost"] IP adress of the Perun instance or "localhost"
PerunConfig.TCPTargetPort = 48621																-- (int) [default: 48621] TCP port to send data to
PerunConfig.Instance = 3																	        -- (int) [default: 1] Id number of instance (if multiple DCS instances are to run at the same PC)
PerunConfig.RefreshStatus = 60																    -- (int) [default: 60] Base refresh rate in seconds to send status update
PerunConfig.RefreshMission = 120 																-- (int) [default: 120] Refresh rate in seconds to send mission information
PerunConfig.MissionStartNoDeathWindow = 300													    -- (int) [default: 300] Number of secounds after mission start when death of the pilot will not go to statistics, shall avoid death penalty during spawning DCS bugs
PerunConfig.DebugMode = 2																		-- (int) [0 (default),1,2] Value greater than 0 will display Perun information in DCS log file, values: 1 - minimal verbose, 2 - all log information will be logged
PerunConfig.MOTD_L1 = "Witamy na serwerze #1 Gildia.org !"								        -- (string) Message send to players connecting the server - Line 1
PerunConfig.MOTD_L2 = "Wymagamy obecnosci DCS SRS oraz TeamSpeak - szczegoly na forum"		    -- (string) Message send to players connecting the server - Line 2

-- ###################### END OF SETTINGS - DO NOT MODIFY OUTSIDE THIS SECTION ######################
return PerunConfig