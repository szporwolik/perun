// This class handles MySQL communication
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;

public class DatabaseController
{
    public string strMySQLConnectionString;              // MySQL connection string
    public MySqlConnection connMySQL;
    public bool bStatus;

    public void SendToMySql(string strRawTCPFrame)
    {
        // Main function to send data to mysql
        dynamic strTCPFrame = JsonConvert.DeserializeObject(strRawTCPFrame); // Deserialize raw data
        string strTCPFrameType = strTCPFrame.type;
        string strTCPFrameTimestamp = strTCPFrame.timestamp;
        string strTCPFrameInstance = strTCPFrame.instance;
        string strTCPFramePayload;
        string strTCPFramePayload_Perun;
        string strSQLQueryTxt;


        // Some frames may come without timestamp, use database currrent timestampe then
        if (strTCPFrameTimestamp != null)
        {
            strTCPFrameTimestamp = "'" + strTCPFrameTimestamp + "'";
        }
        else
        {
            strTCPFrameTimestamp = "CURRENT_TIMESTAMP()";
        }

        // Modify specific types
        if (strTCPFrameType == "1")
        {
            strTCPFrame.payload["v_win"] = "v" + Globals.strPerunVersion; // Inject app version information
        }

        // Specific SQL per each frame type
        if (strTCPFrameType == "50")
        {
            // Add entry to chat log
            strSQLQueryTxt = "INSERT INTO `pe_DataPlayers` (`pe_DataPlayers_ucid`) SELECT '" + strTCPFrame.payload.ucid + "' FROM DUAL WHERE NOT EXISTS (SELECT * FROM `pe_DataPlayers` where `pe_DataPlayers_ucid` = '" + strTCPFrame.payload.ucid + "' );";
            strSQLQueryTxt += "UPDATE  `pe_DataPlayers` SET `pe_DataPlayers_updated` = " + strTCPFrameTimestamp + ",`pe_DataPlayers_lastname`='" + strTCPFrame.payload.player + "' WHERE `pe_DataPlayers_ucid`='" + strTCPFrame.payload.ucid + "' ;";
            strSQLQueryTxt += "INSERT INTO `pe_DataMissionHashes` (`pe_DataMissionHashes_hash`,`pe_DataMissionHashes_instance`) SELECT '" + strTCPFrame.payload.missionhash + "','" + strTCPFrameInstance + "' FROM DUAL WHERE NOT EXISTS (SELECT * FROM `pe_DataMissionHashes` where `pe_DataMissionHashes_hash` ='" + strTCPFrame.payload.missionhash + "' AND `pe_DataMissionHashes_instance`=" + strTCPFrameInstance + ");";
            strSQLQueryTxt += "UPDATE `pe_DataMissionHashes` SET `pe_DataMissionHashes_datetime` = " + strTCPFrameTimestamp + " WHERE `pe_DataMissionHashes_hash` = '" + strTCPFrame.payload.missionhash + "' AND `pe_DataMissionHashes_instance`=" + strTCPFrameInstance + " ;";
            strSQLQueryTxt += "INSERT INTO `pe_LogChat` (`pe_LogChat_id`,`pe_LogChat_datetime`, `pe_LogChat_playerid`, `pe_LogChat_msg`, `pe_LogChat_all`,`pe_LogChat_missionhash_id`) VALUES (NULL,'" + strTCPFrame.payload.datetime + "', (SELECT `pe_DataPlayers_id` from `pe_DataPlayers` WHERE `pe_DataPlayers_ucid` = '" + strTCPFrame.payload.ucid + "'), '" + strTCPFrame.payload.msg + "', '" + strTCPFrame.payload.all + "',(SELECT `pe_DataMissionHashes_id` FROM `pe_DataMissionHashes` WHERE `pe_DataMissionHashes_hash` = '" + strTCPFrame.payload.missionhash + "'));";
        }
        else if (strTCPFrameType == "51")
        {
            // Add entry to event log
            strSQLQueryTxt = "INSERT INTO `pe_DataMissionHashes` (`pe_DataMissionHashes_hash`,`pe_DataMissionHashes_instance`) SELECT '" + strTCPFrame.payload.log_missionhash + "','" + strTCPFrameInstance + "' FROM DUAL WHERE NOT EXISTS (SELECT * FROM `pe_DataMissionHashes` where `pe_DataMissionHashes_hash` = '" + strTCPFrame.payload.log_missionhash + "' AND `pe_DataMissionHashes_instance`=" + strTCPFrameInstance + ");";
            strSQLQueryTxt += "UPDATE `pe_DataMissionHashes` SET `pe_DataMissionHashes_datetime` = " + strTCPFrameTimestamp + " WHERE `pe_DataMissionHashes_hash` = '" + strTCPFrame.payload.log_missionhash + "' AND `pe_DataMissionHashes_instance`=" + strTCPFrameInstance + ";";
            strSQLQueryTxt += "INSERT INTO `pe_LogEvent` (`pe_LogEvent_arg1`,`pe_LogEvent_arg2`,`pe_LogEvent_id`, `pe_LogEvent_datetime`, `pe_LogEvent_type`, `pe_LogEvent_content`,`pe_LogEvent_missionhash_id`) VALUES ('" + strTCPFrame.payload.log_arg_1 + "','" + strTCPFrame.payload.log_arg_2 + "', NULL, '" + strTCPFrame.payload.log_datetime + "', '" + strTCPFrame.payload.log_type + "', '" + strTCPFrame.payload.log_content + "', (SELECT `pe_DataMissionHashes_id` FROM `pe_DataMissionHashes` WHERE `pe_DataMissionHashes_hash` = '" + strTCPFrame.payload.log_missionhash + "'));";
        }
        else if (strTCPFrameType == "52")
        {
            // Update user stats
            strTCPFramePayload = JsonConvert.SerializeObject(strTCPFrame.payload.stat_data_dcs); // Deserialize payload
            strTCPFramePayload_Perun = JsonConvert.SerializeObject(strTCPFrame.payload.stat_data_perun); // Deserialize payload

            strSQLQueryTxt = "INSERT INTO `pe_DataMissionHashes` (`pe_DataMissionHashes_hash`,`pe_DataMissionHashes_instance`) SELECT '" + strTCPFrame.payload.stat_missionhash + "','" + strTCPFrameInstance + "' FROM DUAL WHERE NOT EXISTS (SELECT * FROM `pe_DataMissionHashes` where `pe_DataMissionHashes_hash` = '" + strTCPFrame.payload.stat_missionhash + "' AND `pe_DataMissionHashes_instance`=" + strTCPFrameInstance + ");";
            strSQLQueryTxt += "UPDATE `pe_DataMissionHashes` SET `pe_DataMissionHashes_datetime` = " + strTCPFrameTimestamp + " WHERE `pe_DataMissionHashes_hash` = '" + strTCPFrame.payload.stat_missionhash + "' AND `pe_DataMissionHashes_instance`=" + strTCPFrameInstance + " ;";
            strSQLQueryTxt += "INSERT INTO `pe_DataPlayers` (`pe_DataPlayers_ucid`) SELECT '" + strTCPFrame.payload.stat_ucid + "' FROM DUAL WHERE NOT EXISTS (SELECT * FROM `pe_DataPlayers` where `pe_DataPlayers_ucid` = '" + strTCPFrame.payload.stat_ucid + "');";
            strSQLQueryTxt += "UPDATE `pe_DataPlayers` SET `pe_DataPlayers_updated`=" + strTCPFrameTimestamp + " WHERE `pe_DataPlayers_ucid`='" + strTCPFrame.payload.stat_ucid + "';";
            strSQLQueryTxt += "INSERT INTO `pe_DataTypes` (`pe_DataTypes_name`) SELECT '" + strTCPFrame.payload.stat_data_type + "' FROM DUAL WHERE NOT EXISTS (SELECT * FROM `pe_DataTypes` where `pe_DataTypes_name` = '" + strTCPFrame.payload.stat_data_type + "');";
            strSQLQueryTxt += "INSERT INTO `pe_LogStats` (`pe_LogStats_playerid`,`pe_LogStats_missionhash_id`,`pe_LogStats_typeid`) SELECT (SELECT `pe_DataPlayers_id` from `pe_DataPlayers` WHERE `pe_DataPlayers_ucid` = '" + strTCPFrame.payload.stat_ucid + "'), (SELECT `pe_DataMissionHashes_id` FROM `pe_DataMissionHashes` WHERE `pe_DataMissionHashes_hash` = '" + strTCPFrame.payload.stat_missionhash + "'), (SELECT pe_DataTypes_id FROM `pe_DataTypes` where `pe_DataTypes_name` = '" + strTCPFrame.payload.stat_data_type + "') FROM DUAL WHERE NOT EXISTS (SELECT * FROM `pe_LogStats` WHERE `pe_LogStats_missionhash_id`=(SELECT `pe_DataMissionHashes_id` FROM `pe_DataMissionHashes` WHERE `pe_DataMissionHashes_hash` = '" + strTCPFrame.payload.stat_missionhash + "') AND `pe_LogStats_playerid` =  (SELECT `pe_DataPlayers_id` from `pe_DataPlayers` WHERE `pe_DataPlayers_ucid` = '" + strTCPFrame.payload.stat_ucid + "') AND `pe_LogStats_typeid`= (SELECT pe_DataTypes_id FROM `pe_DataTypes` where `pe_DataTypes_name` = '" + strTCPFrame.payload.stat_data_type + "'));";
            strSQLQueryTxt += "UPDATE `pe_LogStats` SET `ps_kills_fortification`=" + strTCPFrame.payload.stat_data_perun.ps_kills_fortification + ",`ps_other_landings`=" + strTCPFrame.payload.stat_data_perun.ps_other_landings + ",`ps_other_takeoffs`=" + strTCPFrame.payload.stat_data_perun.ps_other_takeoffs + ",`ps_pvp`=" + strTCPFrame.payload.stat_data_perun.ps_pvp + ",`ps_deaths`=" + strTCPFrame.payload.stat_data_perun.ps_deaths + ",`ps_ejections`=" + strTCPFrame.payload.stat_data_perun.ps_ejections + ",`ps_crashes`=" + strTCPFrame.payload.stat_data_perun.ps_crashes + ",`ps_teamkills`=" + strTCPFrame.payload.stat_data_perun.ps_teamkills + ",`ps_kills_planes`=" + strTCPFrame.payload.stat_data_perun.ps_kills_planes + ",`ps_kills_helicopters`=" + strTCPFrame.payload.stat_data_perun.ps_kills_helicopters + ",`ps_kills_air_defense`=" + strTCPFrame.payload.stat_data_perun.ps_kills_air_defense + ",`ps_kills_armor`=" + strTCPFrame.payload.stat_data_perun.ps_kills_armor + ",`ps_kills_unarmed`=" + strTCPFrame.payload.stat_data_perun.ps_kills_unarmed + ",`ps_kills_infantry`=" + strTCPFrame.payload.stat_data_perun.ps_kills_infantry + ",`ps_kills_ships`=" + strTCPFrame.payload.stat_data_perun.ps_kills_ships + ",`ps_kills_other`=" + strTCPFrame.payload.stat_data_perun.ps_kills_other + ",`ps_airfield_takeoffs`=" + strTCPFrame.payload.stat_data_perun.ps_airfield_takeoffs + ",`ps_airfield_landings`=" + strTCPFrame.payload.stat_data_perun.ps_airfield_landings + ",`ps_ship_takeoffs`=" + strTCPFrame.payload.stat_data_perun.ps_ship_takeoffs + ",`ps_ship_landings`=" + strTCPFrame.payload.stat_data_perun.ps_ship_landings + ",`ps_farp_takeoffs`=" + strTCPFrame.payload.stat_data_perun.ps_farp_takeoffs + ",`ps_farp_landings`=" + strTCPFrame.payload.stat_data_perun.ps_farp_landings + ", `pe_LogStats_datetime`='" + strTCPFrame.payload.stat_datetime + "',`pe_LogStats_playerid` =  (SELECT `pe_DataPlayers_id` from `pe_DataPlayers` WHERE `pe_DataPlayers_ucid` = '" + strTCPFrame.payload.stat_ucid + "'),`pe_LogStats_missionhash_id`=(SELECT `pe_DataMissionHashes_id` FROM `pe_DataMissionHashes` WHERE `pe_DataMissionHashes_hash` = '" + strTCPFrame.payload.stat_missionhash + "') WHERE `pe_LogStats_missionhash_id`=(SELECT `pe_DataMissionHashes_id` FROM `pe_DataMissionHashes` WHERE `pe_DataMissionHashes_hash` = '" + strTCPFrame.payload.stat_missionhash + "') AND `pe_LogStats_playerid` =  (SELECT `pe_DataPlayers_id` from `pe_DataPlayers` WHERE `pe_DataPlayers_ucid` = '" + strTCPFrame.payload.stat_ucid + "') AND `pe_LogStats_typeid`=(SELECT pe_DataTypes_id FROM `pe_DataTypes` where `pe_DataTypes_name` = '" + strTCPFrame.payload.stat_data_type + "');";
        }
        else if (strTCPFrameType == "53")
        {
            // User logged in to DCS server

            strSQLQueryTxt = "INSERT INTO `pe_DataPlayers` (`pe_DataPlayers_ucid`) SELECT '" + strTCPFrame.payload.login_ucid + "' FROM DUAL WHERE NOT EXISTS (SELECT * FROM `pe_DataPlayers` where pe_DataPlayers_ucid='" + strTCPFrame.payload.login_ucid + "');";
            strSQLQueryTxt += "UPDATE `pe_DataPlayers` SET  pe_DataPlayers_lastip='" + strTCPFrame.payload.login_ipaddr + "', pe_DataPlayers_lastname='" + strTCPFrame.payload.login_name + "',pe_DataPlayers_updated='" + strTCPFrame.payload.login_datetime + "' WHERE `pe_DataPlayers_ucid`= '" + strTCPFrame.payload.login_ucid + "';";
            strSQLQueryTxt += "INSERT INTO `pe_LogLogins` (`pe_LogLogins_datetime`, `pe_LogLogins_playerid`, `pe_LogLogins_name`, `pe_LogLogins_ip`,`pe_LogLogins_instance`) VALUES ('" + strTCPFrame.payload.login_datetime + "', (SELECT pe_DataPlayers_id from pe_DataPlayers WHERE pe_DataPlayers_ucid = '" + strTCPFrame.payload.login_ucid + "'), '" + strTCPFrame.payload.login_name + "', '" + strTCPFrame.payload.login_ipaddr + "','" + strTCPFrameInstance + "');";
        }
        else
        {
            // General definition used for 1-10 type packets
            strTCPFramePayload = JsonConvert.SerializeObject(strTCPFrame.payload); // Deserialize payload

            strSQLQueryTxt = "INSERT INTO `pe_DataRaw` (`pe_dataraw_type`,`pe_dataraw_instance`) SELECT '" + strTCPFrameType + "','" + strTCPFrameInstance + "' FROM DUAL WHERE NOT EXISTS (SELECT * FROM `pe_DataRaw` WHERE `pe_dataraw_type` = '" + strTCPFrameType + "' AND `pe_dataraw_instance` = " + strTCPFrameInstance + ");";
            strSQLQueryTxt += "UPDATE `pe_DataRaw` SET `pe_dataraw_payload` = JSON_QUOTE('" + strTCPFramePayload + "'), `pe_dataraw_updated`=" + strTCPFrameTimestamp + " WHERE `pe_dataraw_type`=" + strTCPFrameType + " AND `pe_dataraw_instance` = " + strTCPFrameInstance + ";";
        }

        // Connect to mysql and execute sql
        try
        {
            connMySQL = new MySqlConnection(strMySQLConnectionString);

            try
            {
                Console.WriteLine("Sending data to MySQL - Begin");
                connMySQL.Open();
                bStatus = true;
                MySqlCommand cmdMySQL = new MySqlCommand(strSQLQueryTxt, connMySQL);
                MySqlDataReader rdrMySQL = cmdMySQL.ExecuteReader();

                while (rdrMySQL.Read())
                {
                    Console.WriteLine(rdrMySQL[0] + " -- " + rdrMySQL[1]);
                }
                rdrMySQL.Close();
                switch (Int32.Parse(strTCPFrameType))
                {
                    case 1:
                        PerunHelper.GUILogHistoryAdd(ref Globals.arrGUILogHistory, "Connected players: " + strTCPFrame.payload["c_players"], 1,0,"1");
                        break;
                    case 2:
                        PerunHelper.GUILogHistoryAdd(ref Globals.arrGUILogHistory, "Mission: \"" + strTCPFrame.payload["mission"]["name"]+"\""+ ", time:" + strTCPFrame.payload["mission"]["modeltime"], 1, 0, "2");
                        break;
                    case 3:
                        PerunHelper.GUILogHistoryAdd(ref Globals.arrGUILogHistory, "Slots data updated", 1, 0, "3");
                        break;
                    case 50:
                        PerunHelper.GUILogHistoryAdd(ref Globals.arrGUILogHistory, "Player "+ strTCPFrame.payload.player + " chat message saved", 1, 0, "50");
                        break;
                    case 51:
                        PerunHelper.GUILogHistoryAdd(ref Globals.arrGUILogHistory, "Game event: "+ strTCPFrame.payload.log_content, 1, 0, "51");
                        break;
                    case 52:
                        PerunHelper.GUILogHistoryAdd(ref Globals.arrGUILogHistory, "Player "+ strTCPFrame.payload.stat_name + " stats saved", 1, 0, "52");
                        break;
                    case 53:
                        PerunHelper.GUILogHistoryAdd(ref Globals.arrGUILogHistory, "Player "+ strTCPFrame.payload.login_name + " logged in", 1, 0, "53");
                        break;
                    case 100:
                        PerunHelper.GUILogHistoryAdd(ref Globals.arrGUILogHistory, "SRS data send", 1,0,"100");
                        break;
                    case 101:
                        PerunHelper.GUILogHistoryAdd(ref Globals.arrGUILogHistory, "LotATC data send", 1,0,"101");
                        break;
                    default:
                        PerunHelper.GUILogHistoryAdd(ref Globals.arrGUILogHistory, "Data send", 1,0, strTCPFrameType);
                        break;
                }
                
            }
            catch (ArgumentException a_ex)
            {
                // General exception found
                Console.WriteLine(a_ex.ToString());
                PerunHelper.GUILogHistoryAdd(ref Globals.arrGUILogHistory, "ERROR MySQL - error: " + a_ex.Message,1,1, strTCPFrameType);
                bStatus = false;
            }
            catch (MySqlException m_ex)
            {
                // MySQL exception found
                switch (m_ex.Number)
                {
                    case 1042: // Unable to connect to any of the specified MySQL hosts (Check Server,Port)
                        PerunHelper.GUILogHistoryAdd(ref Globals.arrGUILogHistory, "ERROR MySQL - unable to connect, error: " + m_ex.Message,1,1, strTCPFrameType);
                        break;
                    case 0: // Access denied (Check DB name,username,password)
                        PerunHelper.GUILogHistoryAdd(ref Globals.arrGUILogHistory, "ERROR MySQL - access denied, error: " + m_ex.Message,1,1, strTCPFrameType);
                        break;
                    default:
                        PerunHelper.GUILogHistoryAdd(ref Globals.arrGUILogHistory, "ERROR MySQL - error id: " + m_ex.Number,1,1, strTCPFrameType);
                        PerunHelper.GUILogHistoryAdd(ref Globals.arrGUILogHistory, "ERROR MySQL - error: " + m_ex.Message,1,1, strTCPFrameType);
                        break;
                }
                bStatus = false;
            }
            connMySQL.Close();
            Console.WriteLine("Sending data to MySQL - Done");
        }
        catch (ArgumentException x_ex)
        {
            PerunHelper.GUILogHistoryAdd(ref Globals.arrGUILogHistory, "ERROR MySQL - unable to connect, error: " + x_ex.Message,1,1, strTCPFrameType);
        }


    }
}
