// This class handles MySQL communication
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;

public class DatabaseController
{
    public string DatabaseConnectionString;                 // MySQL connection string
    public MySqlConnection DatabaseConnection;              // MySQL connection
    public bool DatabaseStatus;                             // MySQL connections status

    public int SendToMySql(string RawTCPFrame, bool CheckConnection = false)
    {
        // Main function to send data to mysql
        string SQLQueryTxt = "";                                             // MySQL Query string
        int ReturnValue = 1;                                            // Default return value [1 for success]

        // Below is used to check if we still have database connection
        if (CheckConnection)
        {
            RawTCPFrame = "{\"type\": \"-1\"}";
        }

        // Parse frame
        dynamic TCPFrame;
        string TCPFrameType;
        string TCPFrameTimestamp;
        string TCPFrameInstance;
        try
        {
            TCPFrame = JsonConvert.DeserializeObject(RawTCPFrame); // Deserialize raw data
            TCPFrameType = TCPFrame.type;
            TCPFrameTimestamp = (TCPFrame.timestamp != null) ? $"'{TCPFrame.timestamp}'" : "CURRENT_TIMESTAMP()";    // Frame timestamp (Some frames may come without timestamp, use database timestamp then)
            TCPFrameInstance = TCPFrame.instance;
        }
        catch (Exception)
        {
            LogController.instance.LogError("ERROR - SendToMySql cannot deserialize tcp frame !", RawTCPFrame);
            return 0;
        }

        string TCPFramePayload; // Frame payload (we will deserialise only for specific frame types)

        // Connect to mysql and execute sql
        try
        {
            DatabaseConnection = new MySqlConnection(DatabaseConnectionString);

            try
            {
                Console.WriteLine("Sending data to MySQL - Begin");
                DatabaseConnection.Open();
                DatabaseStatus = true;
                
                
                string LogInfo = "";
                int LogDirection = 1;
                MySqlCommand DatabaseCommand = null;

                // For frames 1 to 10 we will put those into data raw tables
                if (Int32.Parse(TCPFrameType) >= 1 && Int32.Parse(TCPFrameType) <= 10)
                {
                    SQLQueryTxt = "INSERT INTO `pe_DataRaw` (`pe_dataraw_type`,`pe_dataraw_instance`) SELECT '" + TCPFrameType + "','" + TCPFrameInstance + "' FROM DUAL WHERE NOT EXISTS (SELECT * FROM `pe_DataRaw` WHERE `pe_dataraw_type` = '" + TCPFrameType + "' AND `pe_dataraw_instance` = " + TCPFrameInstance + ");";
                    SQLQueryTxt += "UPDATE `pe_DataRaw` SET `pe_dataraw_payload` = @PAR_TCPFramePayload, `pe_dataraw_updated`=" + TCPFrameTimestamp + " WHERE `pe_dataraw_type`=" + TCPFrameType + " AND `pe_dataraw_instance` = " + TCPFrameInstance + ";";
                } 
                else
                {
                    SQLQueryTxt = "";
                }

                switch (Int32.Parse(TCPFrameType))
                {
                    case -1:
                        // Keep alive message
                        LogInfo = $"Reading MySQL config";
                        LogDirection = 3;

                        SQLQueryTxt = "SELECT `pe_Config_payload` FROM `pe_Config` WHERE `pe_Config_id` = 1;";

                        DatabaseCommand = new MySqlCommand(SQLQueryTxt, DatabaseConnection);
                        break;

                    case 1:
                        // General status information (diagnostic information)
                        LogInfo = $"Status data updated";

                        TCPFrame.payload.server["v_win"] = "v" + Globals.VersionPerun; // Inject app version information (for database)
                        Globals.VersionDCSHook = TCPFrame.payload.v_dcs_hook; // Pull DCS hook version (for app)

                        SQLQueryTxt += "INSERT INTO `pe_OnlineStatus` (`pe_OnlineStatus_instance`) SELECT @PAR_TCPFrameInstance FROM DUAL WHERE NOT EXISTS(SELECT * FROM `pe_OnlineStatus` WHERE `pe_OnlineStatus_instance` = @PAR_TCPFrameInstance);";
                        SQLQueryTxt += "UPDATE `pe_OnlineStatus` SET `pe_OnlineStatus_perunversion_winapp` = '" + TCPFrame.payload.server.v_win + "', `pe_OnlineStatus_perunversion_dcshook` ='" + TCPFrame.payload.server.v_dcs_hook + "' WHERE `pe_OnlineStatus_instance` = @PAR_TCPFrameInstance ;";
                        SQLQueryTxt += "DELETE FROM pe_OnlinePlayers WHERE pe_OnlinePlayers_instance = @PAR_TCPFrameInstance;";

                        int player_count = 0;
                        foreach (var record_player in TCPFrame.payload.clients)
                        {
                            SQLQueryTxt += "INSERT INTO `pe_OnlinePlayers` (`pe_OnlinePlayers_id`, `pe_OnlinePlayers_instance`, `pe_OnlinePlayers_ping`, `pe_OnlinePlayers_side`, `pe_OnlinePlayers_slot`, `pe_OnlinePlayers_ucid`, `pe_OnlinePlayers_name`) VALUES (" + record_player.id + ", @PAR_TCPFrameInstance, '" + record_player.ping + "', '" + record_player.side + "', '" + record_player.slot + "', '" + record_player.ucid + $"',@PAR_UserName_{player_count});";
                            player_count++;
                            Globals.CurrentMission.PlayerCount = player_count; // Save information for GUI - player count
                        }
                        SQLQueryTxt += "INSERT INTO `pe_OnlineStatus` (`pe_OnlineStatus_instance`) SELECT @PAR_TCPFrameInstance FROM DUAL WHERE NOT EXISTS(SELECT * FROM `pe_OnlineStatus` WHERE `pe_OnlineStatus_instance` = @PAR_TCPFrameInstance);";
                        SQLQueryTxt += "UPDATE `pe_OnlineStatus` SET `pe_OnlineStatus_mission_theatre` = @PAR_MissionTheathre, `pe_OnlineStatus_mission_name` = @PAR_MissionName , `pe_OnlineStatus_server_pause` = @PAR_MissionPause, `pe_OnlineStatus_mission_multiplayer` = '" + TCPFrame.payload.mission.multiplayer + "', `pe_OnlineStatus_server_realtime` = '" + TCPFrame.payload.mission.realtime + "', `pe_OnlineStatus_mission_modeltime` = '" + TCPFrame.payload.mission.modeltime + "', `pe_OnlineStatus_server_players` =  " + player_count + " WHERE `pe_OnlineStatus_instance` = @PAR_TCPFrameInstance;";

                        DatabaseCommand = new MySqlCommand(SQLQueryTxt, DatabaseConnection);
                        TCPFramePayload = JsonConvert.SerializeObject(TCPFrame.payload); // Deserialize payload TUTAJ DODAC CATCH TBD
                        DatabaseCommand.Parameters.AddWithValue("@PAR_TCPFramePayload", TCPFramePayload);
                        DatabaseCommand.Parameters.AddWithValue("@PAR_TCPFrameInstance", TCPFrameInstance);
                        DatabaseCommand.Parameters.AddWithValue("@PAR_MissionName", TCPFrame.payload.mission.name);
                        DatabaseCommand.Parameters.AddWithValue("@PAR_MissionTheathre", TCPFrame.payload.mission.theatre);
                        DatabaseCommand.Parameters.AddWithValue("@PAR_MissionPause", TCPFrame.payload.mission.pause);

                        player_count = 0;
                        foreach (var record_player in TCPFrame.payload.clients)
                        {
                            DatabaseCommand.Parameters.AddWithValue($"@PAR_UserName_{player_count}", TCPFrame.payload.clients[player_count].name);
                            player_count++;
                        }

                        // Save for GUI
                        Globals.CurrentMission.Theatre = TCPFrame.payload.mission.theatre;  // Mission theatre
                        Globals.CurrentMission.Mission = TCPFrame.payload.mission.name;     // Mission name
                        Globals.CurrentMission.Pause = TCPFrame.payload.mission.pause;      // Mission pause

                        Globals.CurrentMission.ModelTime = TCPFrame.payload.mission.modeltime;      // Mission time
                        Globals.CurrentMission.RealTime = TCPFrame.payload.mission.realtime;        // Since start of server
                        break;

                    case 2:
                        // Slots Data (basic mission etc)
                        LogInfo = $"Slots data updated";

                        DatabaseCommand = new MySqlCommand(SQLQueryTxt, DatabaseConnection);
                        TCPFramePayload = JsonConvert.SerializeObject(TCPFrame.payload); // Deserialize payload
                        DatabaseCommand.Parameters.AddWithValue("@PAR_TCPFramePayload", TCPFramePayload);

                        break;
                    case 50:
                        // Chat entry
                        LogInfo = $"Player \"{TCPFrame.payload.player}\" -> chat message saved";

                        SQLQueryTxt = "INSERT INTO `pe_DataPlayers` (`pe_DataPlayers_ucid`) SELECT '" + TCPFrame.payload.ucid + "' FROM DUAL WHERE NOT EXISTS (SELECT * FROM `pe_DataPlayers` where `pe_DataPlayers_ucid` = '" + TCPFrame.payload.ucid + "' );";
                        SQLQueryTxt += "UPDATE  `pe_DataPlayers` SET `pe_DataPlayers_updated` = " + TCPFrameTimestamp + ",`pe_DataPlayers_lastname`=@PAR_payload_player WHERE `pe_DataPlayers_ucid`='" + TCPFrame.payload.ucid + "' ;";
                        SQLQueryTxt += "INSERT INTO `pe_DataMissionHashes` (`pe_DataMissionHashes_hash`,`pe_DataMissionHashes_instance`) SELECT '" + TCPFrame.payload.missionhash + "','" + TCPFrameInstance + "' FROM DUAL WHERE NOT EXISTS (SELECT * FROM `pe_DataMissionHashes` where `pe_DataMissionHashes_hash` ='" + TCPFrame.payload.missionhash + "' AND `pe_DataMissionHashes_instance`=" + TCPFrameInstance + ");";
                        SQLQueryTxt += "UPDATE `pe_DataMissionHashes` SET `pe_DataMissionHashes_datetime` = " + TCPFrameTimestamp + " WHERE `pe_DataMissionHashes_hash` = '" + TCPFrame.payload.missionhash + "' AND `pe_DataMissionHashes_instance`=" + TCPFrameInstance + " ;";
                        SQLQueryTxt += "INSERT INTO `pe_LogChat` (`pe_LogChat_id`,`pe_LogChat_datetime`, `pe_LogChat_playerid`, `pe_LogChat_msg`, `pe_LogChat_all`,`pe_LogChat_missionhash_id`) VALUES (NULL,'" + TCPFrame.payload.datetime + "', (SELECT `pe_DataPlayers_id` from `pe_DataPlayers` WHERE `pe_DataPlayers_ucid` = '" + TCPFrame.payload.ucid + "'), @PAR_payload_msg, '" + TCPFrame.payload.all + "',(SELECT `pe_DataMissionHashes_id` FROM `pe_DataMissionHashes` WHERE `pe_DataMissionHashes_hash` = '" + TCPFrame.payload.missionhash + "'));";

                        DatabaseCommand = new MySqlCommand(SQLQueryTxt, DatabaseConnection);

                        DatabaseCommand.Parameters.AddWithValue("@PAR_payload_player", TCPFrame.payload.player);
                        DatabaseCommand.Parameters.AddWithValue("@PAR_payload_msg", TCPFrame.payload.msg);

                        break;

                    case 51:
                        // Add entry to event log
                        LogInfo = $"Game event \"{TCPFrame.payload.log_content}\" -> saved";

                        SQLQueryTxt = "INSERT INTO `pe_DataMissionHashes` (`pe_DataMissionHashes_hash`,`pe_DataMissionHashes_instance`) SELECT '" + TCPFrame.payload.log_missionhash + "','" + TCPFrameInstance + "' FROM DUAL WHERE NOT EXISTS (SELECT * FROM `pe_DataMissionHashes` where `pe_DataMissionHashes_hash` = '" + TCPFrame.payload.log_missionhash + "' AND `pe_DataMissionHashes_instance`=" + TCPFrameInstance + ");";
                        SQLQueryTxt += "UPDATE `pe_DataMissionHashes` SET `pe_DataMissionHashes_datetime` = " + TCPFrameTimestamp + " WHERE `pe_DataMissionHashes_hash` = '" + TCPFrame.payload.log_missionhash + "' AND `pe_DataMissionHashes_instance`=" + TCPFrameInstance + ";";
                        SQLQueryTxt += "INSERT INTO `pe_LogEvent` (`pe_LogEvent_arg1`,`pe_LogEvent_arg2`,`pe_LogEvent_id`, `pe_LogEvent_datetime`, `pe_LogEvent_type`, `pe_LogEvent_content`,`pe_LogEvent_missionhash_id`) VALUES ('" + TCPFrame.payload.log_arg_1 + "','" + TCPFrame.payload.log_arg_2 + "', NULL, '" + TCPFrame.payload.log_datetime + "', '" + TCPFrame.payload.log_type + "', @PAR_log_content, (SELECT `pe_DataMissionHashes_id` FROM `pe_DataMissionHashes` WHERE `pe_DataMissionHashes_hash` = '" + TCPFrame.payload.log_missionhash + "'));";

                        DatabaseCommand = new MySqlCommand(SQLQueryTxt, DatabaseConnection);

                        DatabaseCommand.Parameters.AddWithValue("@PAR_log_content", TCPFrame.payload.log_content);

                        break;

                    case 52:
                        // Update user stats
                        LogInfo = $"Player \"{TCPFrame.payload.stat_name}\" -> stats updated";

                        SQLQueryTxt = "INSERT INTO `pe_DataMissionHashes` (`pe_DataMissionHashes_hash`,`pe_DataMissionHashes_instance`) SELECT '" + TCPFrame.payload.stat_missionhash + "','" + TCPFrameInstance + "' FROM DUAL WHERE NOT EXISTS (SELECT * FROM `pe_DataMissionHashes` where `pe_DataMissionHashes_hash` = '" + TCPFrame.payload.stat_missionhash + "' AND `pe_DataMissionHashes_instance`=" + TCPFrameInstance + ");";
                        SQLQueryTxt += "UPDATE `pe_DataMissionHashes` SET `pe_DataMissionHashes_datetime` = " + TCPFrameTimestamp + " WHERE `pe_DataMissionHashes_hash` = '" + TCPFrame.payload.stat_missionhash + "' AND `pe_DataMissionHashes_instance`=" + TCPFrameInstance + " ;";
                        SQLQueryTxt += "INSERT INTO `pe_DataPlayers` (`pe_DataPlayers_ucid`) SELECT '" + TCPFrame.payload.stat_ucid + "' FROM DUAL WHERE NOT EXISTS (SELECT * FROM `pe_DataPlayers` where `pe_DataPlayers_ucid` = '" + TCPFrame.payload.stat_ucid + "');";
                        SQLQueryTxt += "UPDATE `pe_DataPlayers` SET `pe_DataPlayers_updated`=" + TCPFrameTimestamp + " WHERE `pe_DataPlayers_ucid`='" + TCPFrame.payload.stat_ucid + "';";
                        SQLQueryTxt += "INSERT INTO `pe_DataTypes` (`pe_DataTypes_name`) SELECT '" + TCPFrame.payload.stat_data_type + "' FROM DUAL WHERE NOT EXISTS (SELECT * FROM `pe_DataTypes` where `pe_DataTypes_name` = '" + TCPFrame.payload.stat_data_type + "');";
                        SQLQueryTxt += "INSERT INTO `pe_LogStats` (`pe_LogStats_playerid`,`pe_LogStats_missionhash_id`,`pe_LogStats_typeid`) SELECT (SELECT `pe_DataPlayers_id` from `pe_DataPlayers` WHERE `pe_DataPlayers_ucid` = '" + TCPFrame.payload.stat_ucid + "'), (SELECT `pe_DataMissionHashes_id` FROM `pe_DataMissionHashes` WHERE `pe_DataMissionHashes_hash` = '" + TCPFrame.payload.stat_missionhash + "'), (SELECT pe_DataTypes_id FROM `pe_DataTypes` where `pe_DataTypes_name` = '" + TCPFrame.payload.stat_data_type + "') FROM DUAL WHERE NOT EXISTS (SELECT * FROM `pe_LogStats` WHERE `pe_LogStats_missionhash_id`=(SELECT `pe_DataMissionHashes_id` FROM `pe_DataMissionHashes` WHERE `pe_DataMissionHashes_hash` = '" + TCPFrame.payload.stat_missionhash + "') AND `pe_LogStats_playerid` =  (SELECT `pe_DataPlayers_id` from `pe_DataPlayers` WHERE `pe_DataPlayers_ucid` = '" + TCPFrame.payload.stat_ucid + "') AND `pe_LogStats_typeid`= (SELECT pe_DataTypes_id FROM `pe_DataTypes` where `pe_DataTypes_name` = '" + TCPFrame.payload.stat_data_type + "'));";
                        SQLQueryTxt += "UPDATE `pe_LogStats` SET `ps_time`=" + TCPFrame.payload.stat_data_perun.ps_time + ",`pe_LogStats_seat`=" + TCPFrame.payload.stat_data_subslot + ",`pe_LogStats_masterslot`=" + TCPFrame.payload.stat_data_masterslot + ",`ps_kills_fortification`=" + TCPFrame.payload.stat_data_perun.ps_kills_fortification + ",`ps_kills_artillery`=" + TCPFrame.payload.stat_data_perun.ps_kills_artillery + ",`ps_other_landings`=" + TCPFrame.payload.stat_data_perun.ps_other_landings + ",`ps_other_takeoffs`=" + TCPFrame.payload.stat_data_perun.ps_other_takeoffs + ",`ps_pvp`=" + TCPFrame.payload.stat_data_perun.ps_pvp + ",`ps_deaths`=" + TCPFrame.payload.stat_data_perun.ps_deaths + ",`ps_ejections`=" + TCPFrame.payload.stat_data_perun.ps_ejections + ",`ps_crashes`=" + TCPFrame.payload.stat_data_perun.ps_crashes + ",`ps_teamkills`=" + TCPFrame.payload.stat_data_perun.ps_teamkills + ",`ps_kills_planes`=" + TCPFrame.payload.stat_data_perun.ps_kills_planes + ",`ps_kills_helicopters`=" + TCPFrame.payload.stat_data_perun.ps_kills_helicopters + ",`ps_kills_air_defense`=" + TCPFrame.payload.stat_data_perun.ps_kills_air_defense + ",`ps_kills_armor`=" + TCPFrame.payload.stat_data_perun.ps_kills_armor + ",`ps_kills_unarmed`=" + TCPFrame.payload.stat_data_perun.ps_kills_unarmed + ",`ps_kills_infantry`=" + TCPFrame.payload.stat_data_perun.ps_kills_infantry + ",`ps_kills_ships`=" + TCPFrame.payload.stat_data_perun.ps_kills_ships + ",`ps_kills_other`=" + TCPFrame.payload.stat_data_perun.ps_kills_other + ",`ps_airfield_takeoffs`=" + TCPFrame.payload.stat_data_perun.ps_airfield_takeoffs + ",`ps_airfield_landings`=" + TCPFrame.payload.stat_data_perun.ps_airfield_landings + ",`ps_ship_takeoffs`=" + TCPFrame.payload.stat_data_perun.ps_ship_takeoffs + ",`ps_ship_landings`=" + TCPFrame.payload.stat_data_perun.ps_ship_landings + ",`ps_farp_takeoffs`=" + TCPFrame.payload.stat_data_perun.ps_farp_takeoffs + ",`ps_farp_landings`=" + TCPFrame.payload.stat_data_perun.ps_farp_landings + ", `pe_LogStats_datetime`='" + TCPFrame.payload.stat_datetime + "',`pe_LogStats_playerid` =  (SELECT `pe_DataPlayers_id` from `pe_DataPlayers` WHERE `pe_DataPlayers_ucid` = '" + TCPFrame.payload.stat_ucid + "'),`pe_LogStats_missionhash_id`=(SELECT `pe_DataMissionHashes_id` FROM `pe_DataMissionHashes` WHERE `pe_DataMissionHashes_hash` = '" + TCPFrame.payload.stat_missionhash + "') WHERE `pe_LogStats_missionhash_id`=(SELECT `pe_DataMissionHashes_id` FROM `pe_DataMissionHashes` WHERE `pe_DataMissionHashes_hash` = '" + TCPFrame.payload.stat_missionhash + "') AND `pe_LogStats_playerid` =  (SELECT `pe_DataPlayers_id` from `pe_DataPlayers` WHERE `pe_DataPlayers_ucid` = '" + TCPFrame.payload.stat_ucid + "') AND `pe_LogStats_typeid`=(SELECT pe_DataTypes_id FROM `pe_DataTypes` where `pe_DataTypes_name` = '" + TCPFrame.payload.stat_data_type + "');";
                        
                        DatabaseCommand = new MySqlCommand(SQLQueryTxt, DatabaseConnection);

                        break;

                    case 53:
                        // User logged in to DCS server
                        LogInfo = $"Player \"{TCPFrame.payload.login_name}\" -> logged in";

                        SQLQueryTxt = "INSERT INTO `pe_DataPlayers` (`pe_DataPlayers_ucid`) SELECT '" + TCPFrame.payload.login_ucid + "' FROM DUAL WHERE NOT EXISTS (SELECT * FROM `pe_DataPlayers` where pe_DataPlayers_ucid='" + TCPFrame.payload.login_ucid + "');";
                        SQLQueryTxt += "UPDATE `pe_DataPlayers` SET  pe_DataPlayers_lastip='" + TCPFrame.payload.login_ipaddr + "', pe_DataPlayers_lastname=@PAR_login_name,pe_DataPlayers_updated='" + TCPFrame.payload.login_datetime + "' WHERE `pe_DataPlayers_ucid`= '" + TCPFrame.payload.login_ucid + "';";
                        SQLQueryTxt += "INSERT INTO `pe_LogLogins` (`pe_LogLogins_datetime`, `pe_LogLogins_playerid`, `pe_LogLogins_name`, `pe_LogLogins_ip`,`pe_LogLogins_instance`) VALUES ('" + TCPFrame.payload.login_datetime + "', (SELECT pe_DataPlayers_id from pe_DataPlayers WHERE pe_DataPlayers_ucid = '" + TCPFrame.payload.login_ucid + "'), @PAR_login_name, '" + TCPFrame.payload.login_ipaddr + "','" + TCPFrameInstance + "');";

                        DatabaseCommand = new MySqlCommand(SQLQueryTxt, DatabaseConnection);
                        
                        DatabaseCommand.Parameters.AddWithValue("@PAR_login_name", TCPFrame.payload.login_name);

                        break;

                    default:
                        // General definition used for other packets

                        // Switch to insert correct log information
                        switch (Int32.Parse(TCPFrameType))
                        {
                            case 3:
                                LogInfo = "Mission data updated";
                                break;
                            case 100:
                                LogInfo = "DCS SRS data updated";
                                break;
                            case 101:
                                LogInfo = "LotATC data updated";
                                break;
                            default:
                                LogInfo = "Unknown data updated";
                                break;
                        }

                        SQLQueryTxt = "INSERT INTO `pe_DataRaw` (`pe_dataraw_type`,`pe_dataraw_instance`) SELECT '" + TCPFrameType + "','" + TCPFrameInstance + "' FROM DUAL WHERE NOT EXISTS (SELECT * FROM `pe_DataRaw` WHERE `pe_dataraw_type` = '" + TCPFrameType + "' AND `pe_dataraw_instance` = " + TCPFrameInstance + ");";
                        SQLQueryTxt += "UPDATE `pe_DataRaw` SET `pe_dataraw_payload` = @PAR_TCPFramePayload, `pe_dataraw_updated`=" + TCPFrameTimestamp + " WHERE `pe_dataraw_type`=" + TCPFrameType + " AND `pe_dataraw_instance` = " + TCPFrameInstance + ";";

                        DatabaseCommand = new MySqlCommand(SQLQueryTxt, DatabaseConnection);

                        TCPFramePayload = JsonConvert.SerializeObject(TCPFrame.payload); // Deserialize payload
                        DatabaseCommand.Parameters.AddWithValue("@PAR_TCPFramePayload", TCPFramePayload);

                        break;
                }

                // End of add parameters
                MySqlDataReader DatabaseReader = DatabaseCommand.ExecuteReader();
                PerunHelper.LogDebug(ref Globals.AppLogHistory, LogInfo, LogDirection, 0, TCPFrameType);

                // Handle reading from database
                if (DatabaseReader.HasRows)
                {
                    while (DatabaseReader.Read())
                    {
                        PerunHelper.LogDebug(ref Globals.AppLogHistory, $"Received MySQL response, fields: {DatabaseReader.FieldCount}", 2, 0, TCPFrameType);
                        if (TCPFrameType == "-1")
                        {
                            Globals.VersionDatabase = DatabaseReader.GetString(0);
                        }
                    }
                }
                DatabaseReader.Close();
            }
            catch (ArgumentException a_ex)
            {
                // General exception found
                Console.WriteLine(a_ex.ToString());
                PerunHelper.LogError(ref Globals.AppLogHistory, $"ERROR MySQL - error: {a_ex.Message}", 1, 1, TCPFrameType);
                Globals.ErrorsGame++;
                DatabaseStatus = false;
                ReturnValue = 0;
            }
            catch (MySqlException m_ex)
            {
                // MySQL exception found
                switch (m_ex.Number)
                {
                    case 0: // Access denied (Check DB name,username,password)
                        PerunHelper.LogError(ref Globals.AppLogHistory, $"ERROR MySQL - access denied, error: {m_ex.Message}", 1, 1, TCPFrameType);
                        DatabaseStatus = false;
                        ReturnValue = 0;
                        break;
                    case 1042: // Unable to connect to any of the specified MySQL hosts (Check Server,Port)
                        PerunHelper.LogError(ref Globals.AppLogHistory, $"ERROR MySQL - unable to connect, error: {m_ex.Message}", 1, 1, TCPFrameType);
                        DatabaseStatus = false;
                        ReturnValue = 0;
                        break;
                    case 1064: // Incorrect query
                        PerunHelper.LogError(ref Globals.AppLogHistory, $"ERROR MySQL - query: {SQLQueryTxt}", 1, 1, TCPFrameType);
                        PerunHelper.LogError(ref Globals.AppLogHistory, $"ERROR MySQL - error: {m_ex.Message}", 1, 1, TCPFrameType);
                        DatabaseStatus = true; // True as connection is not broken
                        ReturnValue = 1; // Return a value to remove this query from quae
                        break;
                    default:    // Default error handler
                        PerunHelper.LogError(ref Globals.AppLogHistory, $"ERROR MySQL - error id: {m_ex.Number}", 1, 1, TCPFrameType);
                        PerunHelper.LogError(ref Globals.AppLogHistory, $"ERROR MySQL - query: {SQLQueryTxt}", 1, 1, TCPFrameType);
                        PerunHelper.LogError(ref Globals.AppLogHistory, $"ERROR MySQL - error: {m_ex.Message}", 1, 1, TCPFrameType);
                        DatabaseStatus = false;
                        ReturnValue = 0;
                        break;
                }
                Globals.ErrorsGame++;

            }
            catch (Exception e)
            {
                // General exception found
                Console.WriteLine(e.ToString());
                PerunHelper.LogError(ref Globals.AppLogHistory, $"ERROR General - error: {e.Message}", 1, 1, TCPFrameType);
                Globals.ErrorsGame++;
                DatabaseStatus = false;
                ReturnValue = 0;
            }
            DatabaseConnection.Close();
            Console.WriteLine("Sending data to MySQL - Done");
        }
        catch (ArgumentException x_ex)
        {
            // MySQL connection error
            PerunHelper.LogError(ref Globals.AppLogHistory, $"ERROR MySQL - unable to connect, error: {x_ex.Message}",1,1, TCPFrameType);
            ReturnValue = 0;
            Globals.ErrorsGame++;
        }

        return ReturnValue;
    }
}
