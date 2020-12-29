-- Perun for DCS World https://github.com/szporwolik/perun -> DCS Hook config component
local Config = {}

-- ###################### SETTINGS - DO NOT MODIFY OUTSIDE THIS SECTION #############################

Config.TCPPerunHost = "localhost"														    -- (string) [default: "localhost"] IP adress of the Perun instance or "localhost"
Config.TCPTargetPort = 48621																-- (int) [default: 48621] TCP port to send data to
Config.Instance = 1																		    -- (int) [default: 1] Id number of instance (if multiple DCS instances are to run at the same PC)
Config.RefreshStatus = 60																    -- (int) [default: 60] Base refresh rate in seconds to send status update
Config.RefreshMission = 120 																-- (int) [default: 120] Refresh rate in seconds to send mission information
Config.MissionStartNoDeathWindow = 300													    -- (int) [default: 300] Number of secounds after mission start when death of the pilot will not go to statistics, shall avoid death penalty during spawning DCS bugs
Config.DebugMode = 1																		-- (int) [0 (default),1,2] Value greater than 0 will display Perun information in DCS log file, values: 1 - minimal verbose, 2 - all log information will be logged
Config.MOTD_L1 = "Witamy na serwerze #1 Gildia.org !"										-- (string) Message send to players connecting the server - Line 1
Config.MOTD_L2 = "Wymagamy obecnosci DCS SRS oraz TeamSpeak - szczegoly na forum"		    -- (string) Message send to players connecting the server - Line 2

-- ###################### END OF SETTINGS - DO NOT MODIFY OUTSIDE THIS SECTION ######################
return Config
