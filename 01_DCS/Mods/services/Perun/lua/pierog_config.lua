-- Perun for DCS World https://github.com/szporwolik/perun -> DCS Hook config component
local config = {}

-- ###################### SETTINGS - DO NOT MODIFY OUTSIDE THIS SECTION #############################
-- Connection
config.host = "localhost"				-- (string) [default: "localhost"] IP adress of the Perun instance or "localhost"
config.port = 48621						-- (int) [default: 48621] TCP port to send data to
config.delimiters = {"<SOT>", "<EOT>"}
config.Instance = 1						-- (int) [default: 1] Id number of instance (if multiple DCS instances run on the same machine)
config.keepAliveSeconds = 5             --
config.logPath = "c:\\temp"              -- path for communication logs

config.bigUpdatesSeconds = 100          -- big updates (mission, unit IDs, slots) have to wait between being sent
-- deprecated, decrease greatly
config.refreshSlotsSeconds = 1200        -- delay between slot updates are sent
config.refreshMissionSeconds = 1200        -- delay between mission update is sent

-- Localisation
-- Server MOTD, sent to ALL players when entering cockpit
config.MOTD_line1 = "[Pierog] Welcome to our server!";
config.MOTD_line2 = "[Pierog] Stats and event data integrated with Pierog for DCS World";
-- (string) Information to send to players when Perun connection is broken
config.ConnectionError = "[Pierog] ERROR: Connection broken - contact server admin!"

-- Misc
config.BroadcastPierogErrors = 1															-- (int) [0 (default),1] Value greater than 0 will broadcast chat message about missing connection to Perun
config.BroadcastErrorEachSeconds = 600

-- Debug
config.DebugMode = 2																		-- (int) [0 (default),1,2] Value greater than 0 will display Perun information in DCS log file, values: 1 - minimal verbose, 2 - all log information will be logged

-- ###################### END OF SETTINGS - DO NOT MODIFY OUTSIDE THIS SECTION ######################
return config
