// This class handles MySQL communication
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;

public class DatabaseController
{
    public string DatabaseConnectionString;              // MySQL connection string
    public MySqlConnection DatabaseConnection;
    public bool DatabaseStatus;

    public int SendToMySql(string RawTCPFrame, bool CheckConnection = false)
    {
        // Main function to send data to mysql
        if (CheckConnection)
        {
            RawTCPFrame = "{\"type\": \"-1\"}";
        }
        dynamic TCPFrame = JsonConvert.DeserializeObject(RawTCPFrame); // Deserialize raw data
        string TCPFrameType = TCPFrame.type;
        string TCPFrameTimestamp = TCPFrame.timestamp;
        string TCPFrameInstance = TCPFrame.instance;
        string TCPFramePayload;
        string SQLQueryTxt;
        int ReturnValue = 1;

        // Some frames may come without timestamp, use database currrent timestampe then
        if (TCPFrameTimestamp != null)
        {
            TCPFrameTimestamp = "'" + TCPFrameTimestamp + "'";
        }
        else
        {
            TCPFrameTimestamp = "CURRENT_TIMESTAMP()";
        }

        // Modify specific types
        if (TCPFrameType == "1")
        {
            TCPFrame.payload["v_win"] = "v" + Globals.VersionPerun; // Inject app version information

            // Pull DCS hook version
            Globals.VersionDCSHook = TCPFrame.payload.v_dcs_hook;
        }

        // Specific SQL per each frame type
        if (TCPFrameType == "50")
        {
            // Add entry to chat log
            SQLQueryTxt = "INSERT INTO `pe_DataPlayers` (`pe_DataPlayers_ucid`) SELECT '" + TCPFrame.payload.ucid + "' FROM DUAL WHERE NOT EXISTS (SELECT * FROM `pe_DataPlayers` where `pe_DataPlayers_ucid` = '" + TCPFrame.payload.ucid + "' );";
            SQLQueryTxt += "UPDATE  `pe_DataPlayers` SET `pe_DataPlayers_updated` = " + TCPFrameTimestamp + ",`pe_DataPlayers_lastname`=@PAR_payload_player WHERE `pe_DataPlayers_ucid`='" + TCPFrame.payload.ucid + "' ;";
            SQLQueryTxt += "INSERT INTO `pe_DataMissionHashes` (`pe_DataMissionHashes_hash`,`pe_DataMissionHashes_instance`) SELECT '" + TCPFrame.payload.missionhash + "','" + TCPFrameInstance + "' FROM DUAL WHERE NOT EXISTS (SELECT * FROM `pe_DataMissionHashes` where `pe_DataMissionHashes_hash` ='" + TCPFrame.payload.missionhash + "' AND `pe_DataMissionHashes_instance`=" + TCPFrameInstance + ");";
            SQLQueryTxt += "UPDATE `pe_DataMissionHashes` SET `pe_DataMissionHashes_datetime` = " + TCPFrameTimestamp + " WHERE `pe_DataMissionHashes_hash` = '" + TCPFrame.payload.missionhash + "' AND `pe_DataMissionHashes_instance`=" + TCPFrameInstance + " ;";
            SQLQueryTxt += "INSERT INTO `pe_LogChat` (`pe_LogChat_id`,`pe_LogChat_datetime`, `pe_LogChat_playerid`, `pe_LogChat_msg`, `pe_LogChat_all`,`pe_LogChat_missionhash_id`) VALUES (NULL,'" + TCPFrame.payload.datetime + "', (SELECT `pe_DataPlayers_id` from `pe_DataPlayers` WHERE `pe_DataPlayers_ucid` = '" + TCPFrame.payload.ucid + "'), @PAR_payload_msg, '" + TCPFrame.payload.all + "',(SELECT `pe_DataMissionHashes_id` FROM `pe_DataMissionHashes` WHERE `pe_DataMissionHashes_hash` = '" + TCPFrame.payload.missionhash + "'));";
        }
        else if (TCPFrameType == "51")
        {
            // Add entry to event log
            SQLQueryTxt = "INSERT INTO `pe_DataMissionHashes` (`pe_DataMissionHashes_hash`,`pe_DataMissionHashes_instance`) SELECT '" + TCPFrame.payload.log_missionhash + "','" + TCPFrameInstance + "' FROM DUAL WHERE NOT EXISTS (SELECT * FROM `pe_DataMissionHashes` where `pe_DataMissionHashes_hash` = '" + TCPFrame.payload.log_missionhash + "' AND `pe_DataMissionHashes_instance`=" + TCPFrameInstance + ");";
            SQLQueryTxt += "UPDATE `pe_DataMissionHashes` SET `pe_DataMissionHashes_datetime` = " + TCPFrameTimestamp + " WHERE `pe_DataMissionHashes_hash` = '" + TCPFrame.payload.log_missionhash + "' AND `pe_DataMissionHashes_instance`=" + TCPFrameInstance + ";";
            SQLQueryTxt += "INSERT INTO `pe_LogEvent` (`pe_LogEvent_arg1`,`pe_LogEvent_arg2`,`pe_LogEvent_id`, `pe_LogEvent_datetime`, `pe_LogEvent_type`, `pe_LogEvent_content`,`pe_LogEvent_missionhash_id`) VALUES ('" + TCPFrame.payload.log_arg_1 + "','" + TCPFrame.payload.log_arg_2 + "', NULL, '" + TCPFrame.payload.log_datetime + "', '" + TCPFrame.payload.log_type + "', @PAR_log_content, (SELECT `pe_DataMissionHashes_id` FROM `pe_DataMissionHashes` WHERE `pe_DataMissionHashes_hash` = '" + TCPFrame.payload.log_missionhash + "'));";
        }
        else if (TCPFrameType == "52")
        {
            // Update user stats
            SQLQueryTxt = "INSERT INTO `pe_DataMissionHashes` (`pe_DataMissionHashes_hash`,`pe_DataMissionHashes_instance`) SELECT '" + TCPFrame.payload.stat_missionhash + "','" + TCPFrameInstance + "' FROM DUAL WHERE NOT EXISTS (SELECT * FROM `pe_DataMissionHashes` where `pe_DataMissionHashes_hash` = '" + TCPFrame.payload.stat_missionhash + "' AND `pe_DataMissionHashes_instance`=" + TCPFrameInstance + ");";
            SQLQueryTxt += "UPDATE `pe_DataMissionHashes` SET `pe_DataMissionHashes_datetime` = " + TCPFrameTimestamp + " WHERE `pe_DataMissionHashes_hash` = '" + TCPFrame.payload.stat_missionhash + "' AND `pe_DataMissionHashes_instance`=" + TCPFrameInstance + " ;";
            SQLQueryTxt += "INSERT INTO `pe_DataPlayers` (`pe_DataPlayers_ucid`) SELECT '" + TCPFrame.payload.stat_ucid + "' FROM DUAL WHERE NOT EXISTS (SELECT * FROM `pe_DataPlayers` where `pe_DataPlayers_ucid` = '" + TCPFrame.payload.stat_ucid + "');";
            SQLQueryTxt += "UPDATE `pe_DataPlayers` SET `pe_DataPlayers_updated`=" + TCPFrameTimestamp + " WHERE `pe_DataPlayers_ucid`='" + TCPFrame.payload.stat_ucid + "';";
            SQLQueryTxt += "INSERT INTO `pe_DataTypes` (`pe_DataTypes_name`) SELECT '" + TCPFrame.payload.stat_data_type + "' FROM DUAL WHERE NOT EXISTS (SELECT * FROM `pe_DataTypes` where `pe_DataTypes_name` = '" + TCPFrame.payload.stat_data_type + "');";
            SQLQueryTxt += "INSERT INTO `pe_LogStats` (`pe_LogStats_playerid`,`pe_LogStats_missionhash_id`,`pe_LogStats_typeid`) SELECT (SELECT `pe_DataPlayers_id` from `pe_DataPlayers` WHERE `pe_DataPlayers_ucid` = '" + TCPFrame.payload.stat_ucid + "'), (SELECT `pe_DataMissionHashes_id` FROM `pe_DataMissionHashes` WHERE `pe_DataMissionHashes_hash` = '" + TCPFrame.payload.stat_missionhash + "'), (SELECT pe_DataTypes_id FROM `pe_DataTypes` where `pe_DataTypes_name` = '" + TCPFrame.payload.stat_data_type + "') FROM DUAL WHERE NOT EXISTS (SELECT * FROM `pe_LogStats` WHERE `pe_LogStats_missionhash_id`=(SELECT `pe_DataMissionHashes_id` FROM `pe_DataMissionHashes` WHERE `pe_DataMissionHashes_hash` = '" + TCPFrame.payload.stat_missionhash + "') AND `pe_LogStats_playerid` =  (SELECT `pe_DataPlayers_id` from `pe_DataPlayers` WHERE `pe_DataPlayers_ucid` = '" + TCPFrame.payload.stat_ucid + "') AND `pe_LogStats_typeid`= (SELECT pe_DataTypes_id FROM `pe_DataTypes` where `pe_DataTypes_name` = '" + TCPFrame.payload.stat_data_type + "'));";
            SQLQueryTxt += "UPDATE `pe_LogStats` SET `ps_time`=" + TCPFrame.payload.stat_data_perun.ps_time + ",`pe_LogStats_seat`=" + TCPFrame.payload.stat_data_subslot + ",`pe_LogStats_masterslot`=" + TCPFrame.payload.stat_data_masterslot + ",`ps_kills_fortification`=" + TCPFrame.payload.stat_data_perun.ps_kills_fortification +",`ps_kills_artillery`=" + TCPFrame.payload.stat_data_perun.ps_kills_artillery + ",`ps_other_landings`=" + TCPFrame.payload.stat_data_perun.ps_other_landings + ",`ps_other_takeoffs`=" + TCPFrame.payload.stat_data_perun.ps_other_takeoffs + ",`ps_pvp`=" + TCPFrame.payload.stat_data_perun.ps_pvp + ",`ps_deaths`=" + TCPFrame.payload.stat_data_perun.ps_deaths + ",`ps_ejections`=" + TCPFrame.payload.stat_data_perun.ps_ejections + ",`ps_crashes`=" + TCPFrame.payload.stat_data_perun.ps_crashes + ",`ps_teamkills`=" + TCPFrame.payload.stat_data_perun.ps_teamkills + ",`ps_kills_planes`=" + TCPFrame.payload.stat_data_perun.ps_kills_planes + ",`ps_kills_helicopters`=" + TCPFrame.payload.stat_data_perun.ps_kills_helicopters + ",`ps_kills_air_defense`=" + TCPFrame.payload.stat_data_perun.ps_kills_air_defense + ",`ps_kills_armor`=" + TCPFrame.payload.stat_data_perun.ps_kills_armor + ",`ps_kills_unarmed`=" + TCPFrame.payload.stat_data_perun.ps_kills_unarmed + ",`ps_kills_infantry`=" + TCPFrame.payload.stat_data_perun.ps_kills_infantry + ",`ps_kills_ships`=" + TCPFrame.payload.stat_data_perun.ps_kills_ships + ",`ps_kills_other`=" + TCPFrame.payload.stat_data_perun.ps_kills_other + ",`ps_airfield_takeoffs`=" + TCPFrame.payload.stat_data_perun.ps_airfield_takeoffs + ",`ps_airfield_landings`=" + TCPFrame.payload.stat_data_perun.ps_airfield_landings + ",`ps_ship_takeoffs`=" + TCPFrame.payload.stat_data_perun.ps_ship_takeoffs + ",`ps_ship_landings`=" + TCPFrame.payload.stat_data_perun.ps_ship_landings + ",`ps_farp_takeoffs`=" + TCPFrame.payload.stat_data_perun.ps_farp_takeoffs + ",`ps_farp_landings`=" + TCPFrame.payload.stat_data_perun.ps_farp_landings + ", `pe_LogStats_datetime`='" + TCPFrame.payload.stat_datetime + "',`pe_LogStats_playerid` =  (SELECT `pe_DataPlayers_id` from `pe_DataPlayers` WHERE `pe_DataPlayers_ucid` = '" + TCPFrame.payload.stat_ucid + "'),`pe_LogStats_missionhash_id`=(SELECT `pe_DataMissionHashes_id` FROM `pe_DataMissionHashes` WHERE `pe_DataMissionHashes_hash` = '" + TCPFrame.payload.stat_missionhash + "') WHERE `pe_LogStats_missionhash_id`=(SELECT `pe_DataMissionHashes_id` FROM `pe_DataMissionHashes` WHERE `pe_DataMissionHashes_hash` = '" + TCPFrame.payload.stat_missionhash + "') AND `pe_LogStats_playerid` =  (SELECT `pe_DataPlayers_id` from `pe_DataPlayers` WHERE `pe_DataPlayers_ucid` = '" + TCPFrame.payload.stat_ucid + "') AND `pe_LogStats_typeid`=(SELECT pe_DataTypes_id FROM `pe_DataTypes` where `pe_DataTypes_name` = '" + TCPFrame.payload.stat_data_type + "');";
        }
        else if (TCPFrameType == "53")
        {
            // User logged in to DCS server

            SQLQueryTxt = "INSERT INTO `pe_DataPlayers` (`pe_DataPlayers_ucid`) SELECT '" + TCPFrame.payload.login_ucid + "' FROM DUAL WHERE NOT EXISTS (SELECT * FROM `pe_DataPlayers` where pe_DataPlayers_ucid='" + TCPFrame.payload.login_ucid + "');";
            SQLQueryTxt += "UPDATE `pe_DataPlayers` SET  pe_DataPlayers_lastip='" + TCPFrame.payload.login_ipaddr + "', pe_DataPlayers_lastname=@PAR_login_name,pe_DataPlayers_updated='" + TCPFrame.payload.login_datetime + "' WHERE `pe_DataPlayers_ucid`= '" + TCPFrame.payload.login_ucid + "';";
            SQLQueryTxt += "INSERT INTO `pe_LogLogins` (`pe_LogLogins_datetime`, `pe_LogLogins_playerid`, `pe_LogLogins_name`, `pe_LogLogins_ip`,`pe_LogLogins_instance`) VALUES ('" + TCPFrame.payload.login_datetime + "', (SELECT pe_DataPlayers_id from pe_DataPlayers WHERE pe_DataPlayers_ucid = '" + TCPFrame.payload.login_ucid + "'), @PAR_login_name, '" + TCPFrame.payload.login_ipaddr + "','" + TCPFrameInstance + "');";
        }
        else if (TCPFrameType == "-1")
        {
            // Keep alive message
            SQLQueryTxt = "SELECT `pe_Config_payload` FROM `pe_Config` WHERE `pe_Config_id` = 1;";
        }
        else
        {
            // General definition used for 1-10 type packets
      

            SQLQueryTxt = "INSERT INTO `pe_DataRaw` (`pe_dataraw_type`,`pe_dataraw_instance`) SELECT '" + TCPFrameType + "','" + TCPFrameInstance + "' FROM DUAL WHERE NOT EXISTS (SELECT * FROM `pe_DataRaw` WHERE `pe_dataraw_type` = '" + TCPFrameType + "' AND `pe_dataraw_instance` = " + TCPFrameInstance + ");";
            SQLQueryTxt += "UPDATE `pe_DataRaw` SET `pe_dataraw_payload` = @PAR_TCPFramePayload, `pe_dataraw_updated`=" + TCPFrameTimestamp + " WHERE `pe_dataraw_type`=" + TCPFrameType + " AND `pe_dataraw_instance` = " + TCPFrameInstance + ";";
        }

        // Handling of special data (like parsing JSONs to DB)
        switch (Int32.Parse(TCPFrameType))
        {
            case 1:
                // General status information
                SQLQueryTxt += "INSERT INTO `pe_OnlineStatus` (`pe_OnlineStatus_instance`) SELECT '" + Int32.Parse(TCPFrameInstance) + "' FROM DUAL WHERE NOT EXISTS(SELECT * FROM `pe_OnlineStatus` WHERE `pe_OnlineStatus_instance` = '" + Int32.Parse(TCPFrameInstance) + "');";
                SQLQueryTxt += "UPDATE `pe_OnlineStatus` SET `pe_OnlineStatus_perunversion_winapp` = '" + TCPFrame.payload.v_win + "', `pe_OnlineStatus_perunversion_dcshook` ='" + TCPFrame.payload.v_dcs_hook  + "' WHERE `pe_OnlineStatus_instance` = '" + Int32.Parse(TCPFrameInstance) + "' ;";
                
                break;

            case 2:
                // SRS Data
                SQLQueryTxt += "DELETE FROM pe_OnlinePlayers WHERE pe_OnlinePlayers_instance = " + Int32.Parse(TCPFrameInstance) +";";

                int player_count = 0;
                foreach (var record_player in TCPFrame.payload.players)
                {
                    SQLQueryTxt += "INSERT INTO `pe_OnlinePlayers` (`pe_OnlinePlayers_id`, `pe_OnlinePlayers_instance`, `pe_OnlinePlayers_ping`, `pe_OnlinePlayers_side`, `pe_OnlinePlayers_slot`, `pe_OnlinePlayers_ucid`) VALUES ("+ record_player.id + ", '"+ Int32.Parse(TCPFrameInstance)  + "', '" + record_player.ping + "', '" + record_player.side + "', '" + record_player.slot + "', '" + record_player.ucid + "');";
                    player_count++;
                }

                //SQLQueryTxt += "DELETE FROM pe_OnlineStatus WHERE pe_OnlineStatus_instance = " + Int32.Parse(TCPFrameInstance) + ";";
                SQLQueryTxt += "INSERT INTO `pe_OnlineStatus` (`pe_OnlineStatus_instance`) SELECT '" + Int32.Parse(TCPFrameInstance) + "' FROM DUAL WHERE NOT EXISTS(SELECT * FROM `pe_OnlineStatus` WHERE `pe_OnlineStatus_instance` = '" + Int32.Parse(TCPFrameInstance) + "');";
                SQLQueryTxt += "UPDATE `pe_OnlineStatus` SET `pe_OnlineStatus_theatre` = '" + TCPFrame.payload.mission.theatre + "', `pe_OnlineStatus_name` = '" + TCPFrame.payload.mission.name + "' , `pe_OnlineStatus_pause` = '" + TCPFrame.payload.mission.pause + "', `pe_OnlineStatus_multiplayer` = '" + TCPFrame.payload.mission.multiplayer + "', `pe_OnlineStatus_realtime` = '" + TCPFrame.payload.mission.realtime + "', `pe_OnlineStatus_modeltime` = '" + TCPFrame.payload.mission.modeltime + "', `pe_OnlineStatus_players` =  " + player_count + " WHERE `pe_OnlineStatus_instance` = '" + Int32.Parse(TCPFrameInstance) + "';";

                break;

            default:
                break;
        }


        // Connect to mysql and execute sql
        try
        {
            DatabaseConnection = new MySqlConnection(DatabaseConnectionString);

            try
            {
                Console.WriteLine("Sending data to MySQL - Begin");
                DatabaseConnection.Open();
                DatabaseStatus = true;
                MySqlCommand DatabaseCommand = new MySqlCommand(SQLQueryTxt, DatabaseConnection);

                // Add parameters - prevent SQL injection
                if (TCPFrameType == "50")
                {
                    DatabaseCommand.Parameters.AddWithValue("@PAR_payload_player", TCPFrame.payload.player);
                    DatabaseCommand.Parameters.AddWithValue("@PAR_payload_msg", TCPFrame.payload.msg);
                }
                else if (TCPFrameType == "51")
                {
                    DatabaseCommand.Parameters.AddWithValue("@PAR_log_content", TCPFrame.payload.log_content);
                }
                else if (TCPFrameType == "53")
                {
                    DatabaseCommand.Parameters.AddWithValue("@PAR_login_name", TCPFrame.payload.login_name);
                }
                else
                {
                    TCPFramePayload = JsonConvert.SerializeObject(TCPFrame.payload); // Deserialize payload
                    DatabaseCommand.Parameters.AddWithValue("@PAR_TCPFramePayload", TCPFramePayload);
                }
                // End of add parameters

                    MySqlDataReader DatabaseReader = DatabaseCommand.ExecuteReader();

                if (DatabaseReader.HasRows)
                {
                    while (DatabaseReader.Read())
                    {
                        if (TCPFrameType == "-1")
                        {
                            Globals.VersionDatabase= DatabaseReader.GetString(0);
                        }
                    }
                }
                else
                {
                    // Do nothing
                }

                DatabaseReader.Close();
                
                switch (Int32.Parse(TCPFrameType))
                {
                    case 1:
                        PerunHelper.LogDebug(ref Globals.AppLogHistory, "Connected players: " + TCPFrame.payload["c_players"], 1,0,"1");
                        break;
                    case 2:
                        PerunHelper.LogDebug(ref Globals.AppLogHistory, "Mission: \"" + TCPFrame.payload["mission"]["name"]+"\""+ ", time:" + TCPFrame.payload["mission"]["modeltime"], 1, 0, "2");
                        break;
                    case 3:
                        PerunHelper.LogDebug(ref Globals.AppLogHistory, "Slots data updated", 1, 0, "3");
                        break;
                    case 50:
                        PerunHelper.LogDebug(ref Globals.AppLogHistory, "Player's \""+ TCPFrame.payload.player + "\" chat message saved", 1, 0, "50");
                        break;
                    case 51:
                        PerunHelper.LogDebug(ref Globals.AppLogHistory, "Game event: \""+ TCPFrame.payload.log_content +"\"", 1, 0, "51");
                        break;
                    case 52:
                        PerunHelper.LogDebug(ref Globals.AppLogHistory, "Player's \""+ TCPFrame.payload.stat_name + "\" stats saved", 1, 0, "52");
                        break;
                    case 53:
                        PerunHelper.LogDebug(ref Globals.AppLogHistory, "Player \""+ TCPFrame.payload.login_name + "\" logged in", 1, 0, "53");
                        break;
                    case 100:
                        PerunHelper.LogDebug(ref Globals.AppLogHistory, "SRS data send", 1,0,"100");
                        break;
                    case 101:
                        PerunHelper.LogDebug(ref Globals.AppLogHistory, "LotATC data send", 1,0,"101");
                        break;
                    case -1:
                        break;
                    default:
                        PerunHelper.LogDebug(ref Globals.AppLogHistory, "Data send", 1,0, TCPFrameType);
                        break;
                }
                
            }
            catch (ArgumentException a_ex)
            {
                // General exception found
                Console.WriteLine(a_ex.ToString());
                PerunHelper.LogError(ref Globals.AppLogHistory, "ERROR MySQL - error: " + a_ex.Message,1,1, TCPFrameType);
                Globals.ErrorsGame++;
                DatabaseStatus = false;
                ReturnValue = 0;
            }
            catch (MySqlException m_ex)
            {
                // MySQL exception found
                switch (m_ex.Number)
                {
                    case 1042: // Unable to connect to any of the specified MySQL hosts (Check Server,Port)
                        PerunHelper.LogError(ref Globals.AppLogHistory, "ERROR MySQL - unable to connect, error: " + m_ex.Message,1,1, TCPFrameType);
                        DatabaseStatus = false;
                        ReturnValue = 0;
                        break;
                    case 0: // Access denied (Check DB name,username,password)
                        PerunHelper.LogError(ref Globals.AppLogHistory, "ERROR MySQL - access denied, error: " + m_ex.Message,1,1, TCPFrameType);
                        DatabaseStatus = false;
                        ReturnValue = 0;
                        break;
                    case 1064: // Incorrect query
                        PerunHelper.LogError(ref Globals.AppLogHistory, "ERROR MySQL - query: " + SQLQueryTxt, 1, 1, TCPFrameType);
                        PerunHelper.LogError(ref Globals.AppLogHistory, "ERROR MySQL - error: " + m_ex.Message, 1, 1, TCPFrameType);
                        DatabaseStatus = true; // True as connection is not broken
                        ReturnValue = 1; // Return a value to remove this query from quae
                        break;
                    default:
                        PerunHelper.LogError(ref Globals.AppLogHistory, "ERROR MySQL - error id: " + m_ex.Number,1,1, TCPFrameType);
                        PerunHelper.LogError(ref Globals.AppLogHistory, "ERROR MySQL - query: " + SQLQueryTxt, 1, 1, TCPFrameType);
                        PerunHelper.LogError(ref Globals.AppLogHistory, "ERROR MySQL - error: " + m_ex.Message,1,1, TCPFrameType);
                        DatabaseStatus = false;
                        ReturnValue = 0;
                        break;
                }
                Globals.ErrorsGame++;
                
            }
            DatabaseConnection.Close();
            Console.WriteLine("Sending data to MySQL - Done");
        }
        catch (ArgumentException x_ex)
        {
            PerunHelper.LogError(ref Globals.AppLogHistory, "ERROR MySQL - unable to connect, error: " + x_ex.Message,1,1, TCPFrameType);
            ReturnValue = 0;
            Globals.ErrorsGame++;
        }

        return ReturnValue;
    }
}
