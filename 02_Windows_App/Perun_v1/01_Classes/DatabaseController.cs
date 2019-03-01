// This class handles MySQL communication
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;

internal class DatabaseController
{
    public static string strMySQLConnectionString;              // MySQL connection string

    public static void SendToMySql(string strRawUDPFrame)
    {
        // Main function to send data to mysql
        dynamic strUDPFrame = JsonConvert.DeserializeObject(strRawUDPFrame); // Deserialize raw data
        string strUDPFrameType = strUDPFrame.type;
        string strUDPFrameTimestamp = strUDPFrame.timestamp;
        string strUDPFramePayload = "";
        string strSQLQueryTxt = "";
        

        // Some frames may come without timestamp, use database currrent timestampe then
        if (strUDPFrameTimestamp != null)
        {
            strUDPFrameTimestamp = "'" + strUDPFrameTimestamp + "'";
        }
        else
        {
            strUDPFrameTimestamp = "CURRENT_TIMESTAMP()";
        }

        // Modify specific types
        if (strUDPFrameType == "1") 
        {
            strUDPFrame.payload["v_win"] = "v" + Globals.strPerunVersion; // Inject app version information
        }

        // Specific SQL 
        if (strUDPFrameType == "50")
        {
            // Add entry to chat log
            strSQLQueryTxt = "INSERT INTO `pe_DataPlayers` (`pe_DataPlayers_ucid`) SELECT '" + strUDPFrame.payload.ucid + "' FROM DUAL WHERE NOT EXISTS (SELECT * FROM `pe_DataPlayers` where `pe_DataPlayers_ucid` = '" + strUDPFrame.payload.ucid + "' );";
            strSQLQueryTxt += "UPDATE  `pe_DataPlayers` SET `pe_DataPlayers_updated` = " + strUDPFrameTimestamp + ",`pe_DataPlayers_lastname`='" + strUDPFrame.payload.player + "' WHERE `pe_DataPlayers_ucid`='" + strUDPFrame.payload.ucid + "' ;";
            strSQLQueryTxt += "INSERT INTO `pe_DataMissionHashes` (`pe_DataMissionHashes_hash`) SELECT '" + strUDPFrame.payload.missionhash + "' FROM DUAL WHERE NOT EXISTS (SELECT * FROM `pe_DataMissionHashes` where `pe_DataMissionHashes_hash` ='" + strUDPFrame.payload.missionhash + "' );";
            strSQLQueryTxt += "UPDATE `pe_DataMissionHashes` SET `pe_DataMissionHashes_datetime` = " + strUDPFrameTimestamp + " WHERE `pe_DataMissionHashes_hash` = '" + strUDPFrame.payload.missionhash + "';";
            strSQLQueryTxt += "INSERT INTO `pe_LogChat` (`pe_LogChat_id`,`pe_LogChat_datetime`, `pe_LogChat_playerid`, `pe_LogChat_msg`, `pe_LogChat_all`,`pe_LogChat_missionhash_id`) VALUES (NULL,'" + strUDPFrame.payload.datetime + "', (SELECT `pe_DataPlayers_id` from `pe_DataPlayers` WHERE `pe_DataPlayers_ucid` = '" + strUDPFrame.payload.ucid + "'), '" + strUDPFrame.payload.msg + "', '" + strUDPFrame.payload.all + "',(SELECT `pe_DataMissionHashes_id` FROM `pe_DataMissionHashes` WHERE `pe_DataMissionHashes_hash` = '" + strUDPFrame.payload.missionhash + "'));";
        }
        else if (strUDPFrameType == "51")
        {
            // Add entry to event log
            strSQLQueryTxt = "INSERT INTO `pe_DataMissionHashes` (`pe_DataMissionHashes_hash`) SELECT '" + strUDPFrame.payload.log_missionhash + "' FROM DUAL WHERE NOT EXISTS (SELECT * FROM `pe_DataMissionHashes` where `pe_DataMissionHashes_hash` = '" + strUDPFrame.payload.log_missionhash + "');";
            strSQLQueryTxt += "UPDATE `pe_DataMissionHashes` SET `pe_DataMissionHashes_datetime` = " + strUDPFrameTimestamp + " WHERE `pe_DataMissionHashes_hash` = '" + strUDPFrame.payload.log_missionhash + "';";
            strSQLQueryTxt += "INSERT INTO `pe_LogEvent` (`pe_LogEvent_id`, `pe_LogEvent_datetime`, `pe_LogEvent_type`, `pe_LogEvent_content`,`pe_LogEvent_missionhash_id`) VALUES ( NULL, '" + strUDPFrame.payload.log_datetime + "', '" + strUDPFrame.payload.log_type + "', '" + strUDPFrame.payload.log_content + "', (SELECT `pe_DataMissionHashes_id` FROM `pe_DataMissionHashes` WHERE `pe_DataMissionHashes_hash` = '" + strUDPFrame.payload.log_missionhash + "'));";
        }
        else if (strUDPFrameType == "52")
        {
            // Update user stats
            strUDPFramePayload = JsonConvert.SerializeObject(strUDPFrame.payload.stat_data); // Deserialize payload

            strSQLQueryTxt = "INSERT INTO `pe_DataMissionHashes` (`pe_DataMissionHashes_hash`) SELECT '" + strUDPFrame.payload.stat_missionhash + "' FROM DUAL WHERE NOT EXISTS (SELECT * FROM `pe_DataMissionHashes` where `pe_DataMissionHashes_hash` = '" + strUDPFrame.payload.stat_missionhash + "');";
            strSQLQueryTxt += "UPDATE `pe_DataMissionHashes` SET `pe_DataMissionHashes_datetime` = " + strUDPFrameTimestamp + " WHERE `pe_DataMissionHashes_hash` = '" + strUDPFrame.payload.stat_missionhash + "';";
            strSQLQueryTxt += "INSERT INTO `pe_DataPlayers` (`pe_DataPlayers_ucid`) SELECT '" + strUDPFrame.payload.stat_ucid + "' FROM DUAL WHERE NOT EXISTS (SELECT * FROM `pe_DataPlayers` where `pe_DataPlayers_ucid` = '" + strUDPFrame.payload.stat_ucid + "');";
            strSQLQueryTxt += "UPDATE `pe_DataPlayers` SET `pe_DataPlayers_updated`=" + strUDPFrameTimestamp + " WHERE `pe_DataPlayers_ucid`='" + strUDPFrame.payload.stat_ucid + "';";
            strSQLQueryTxt += "INSERT INTO `pe_LogStats` (`pe_LogStats_playerid`,`pe_LogStats_missionhash_id`) SELECT (SELECT `pe_DataPlayers_id` from `pe_DataPlayers` WHERE `pe_DataPlayers_ucid` = '" + strUDPFrame.payload.stat_ucid + "'), (SELECT `pe_DataMissionHashes_id` FROM `pe_DataMissionHashes` WHERE `pe_DataMissionHashes_hash` = '" + strUDPFrame.payload.stat_missionhash + "') FROM DUAL WHERE NOT EXISTS (SELECT * FROM `pe_LogStats` WHERE `pe_LogStats_missionhash_id`=(SELECT `pe_DataMissionHashes_id` FROM `pe_DataMissionHashes` WHERE `pe_DataMissionHashes_hash` = '" + strUDPFrame.payload.stat_missionhash + "') AND `pe_LogStats_playerid` =  (SELECT `pe_DataPlayers_id` from `pe_DataPlayers` WHERE `pe_DataPlayers_ucid` = '" + strUDPFrame.payload.stat_ucid + "') );";
            strSQLQueryTxt += "UPDATE `pe_LogStats` SET `pe_LogStats_datetime`='" + strUDPFrame.payload.stat_datetime + "',`pe_LogStats_playerid` =  (SELECT `pe_DataPlayers_id` from `pe_DataPlayers` WHERE `pe_DataPlayers_ucid` = '" + strUDPFrame.payload.stat_ucid + "'),`pe_LogStats_debug`=JSON_QUOTE('" + strUDPFramePayload + "'),`pe_LogStats_missionhash_id`=(SELECT `pe_DataMissionHashes_id` FROM `pe_DataMissionHashes` WHERE `pe_DataMissionHashes_hash` = '" + strUDPFrame.payload.stat_missionhash + "') WHERE `pe_LogStats_missionhash_id`=(SELECT `pe_DataMissionHashes_id` FROM `pe_DataMissionHashes` WHERE `pe_DataMissionHashes_hash` = '" + strUDPFrame.payload.stat_missionhash + "') AND `pe_LogStats_playerid` =  (SELECT `pe_DataPlayers_id` from `pe_DataPlayers` WHERE `pe_DataPlayers_ucid` = '" + strUDPFrame.payload.stat_ucid + "');";
        }
        else if (strUDPFrameType == "53")
        {
            // User logged in to DCS server

            strSQLQueryTxt = "INSERT INTO `pe_DataPlayers` (`pe_DataPlayers_ucid`) SELECT '" + strUDPFrame.payload.login_ucid + "' FROM DUAL WHERE NOT EXISTS (SELECT * FROM `pe_DataPlayers` where pe_DataPlayers_ucid='" + strUDPFrame.payload.login_ucid + "');";
            strSQLQueryTxt += "UPDATE `pe_DataPlayers` SET  pe_DataPlayers_lastip='" + strUDPFrame.payload.login_ipaddr + "', pe_DataPlayers_lastname='" + strUDPFrame.payload.login_name + "',pe_DataPlayers_updated='" + strUDPFrame.payload.login_datetime + "' WHERE `pe_DataPlayers_ucid`= '" + strUDPFrame.payload.login_ucid + "';";
            strSQLQueryTxt += "INSERT INTO `pe_LogLogins` (`pe_LogLogins_datetime`, `pe_LogLogins_playerid`, `pe_LogLogins_name`, `pe_LogLogins_ip`) VALUES ('" + strUDPFrame.payload.login_datetime + "', (SELECT pe_DataPlayers_id from pe_DataPlayers WHERE pe_DataPlayers_ucid = '" + strUDPFrame.payload.login_ucid + "'), '" + strUDPFrame.payload.login_name + "', '" + strUDPFrame.payload.login_ipaddr + "');";
        }
        else
        {
            // General definition used for 1-10 type packets
            strUDPFramePayload = JsonConvert.SerializeObject(strUDPFrame.payload); // Deserialize payload

            strSQLQueryTxt = "INSERT INTO `pe_DataRaw` (`pe_dataraw_type`) SELECT '" + strUDPFrameType + "' FROM DUAL WHERE NOT EXISTS (SELECT * FROM `pe_DataRaw` WHERE pe_dataraw_type = '" + strUDPFrameType + "' );";
            strSQLQueryTxt += "UPDATE `pe_DataRaw` SET `pe_dataraw_payload` = JSON_QUOTE('" + strUDPFramePayload + "'), `pe_dataraw_updated`=" + strUDPFrameTimestamp + " WHERE `pe_dataraw_type`=" + strUDPFrameType + ";";
        }

        // Connect to mysql and execute sql
        MySqlConnection connMySQL = new MySqlConnection(DatabaseController.strMySQLConnectionString);
        try
        {
            Console.WriteLine("Sending data to MySQL - Begin");
            connMySQL.Open();

            MySqlCommand cmdMySQL = new MySqlCommand(strSQLQueryTxt, connMySQL);
            MySqlDataReader rdrMySQL = cmdMySQL.ExecuteReader();

            while (rdrMySQL.Read())
            {
                Console.WriteLine(rdrMySQL[0] + " -- " + rdrMySQL[1]);
            }
            rdrMySQL.Close();
            PerunHelper.LogHistoryAdd(ref Globals.arrLogHistory, "MySQL updated, package type: " + strUDPFrameType);
        }
        catch (ArgumentException a_ex)
        {
            // General exception found
            Console.WriteLine(a_ex.ToString());
            PerunHelper.LogHistoryAdd(ref Globals.arrLogHistory, "ERROR MySQL - package type: " + strUDPFrameType);
        }
        catch (MySqlException m_ex)
        {
            // MySQL exception found
            PerunHelper.LogHistoryAdd(ref Globals.arrLogHistory, "ERROR MySQL - package type: " + strUDPFrameType);
            switch (m_ex.Number)
            {
                case 1042: // Unable to connect to any of the specified MySQL hosts (Check Server,Port)
                    PerunHelper.LogHistoryAdd(ref Globals.arrLogHistory, "ERROR MySQL - unable to connect");
                    break;
                case 0: // Access denied (Check DB name,username,password)
                    PerunHelper.LogHistoryAdd(ref Globals.arrLogHistory, "ERROR MySQL - access denied");
                    break;
                default:
                    PerunHelper.LogHistoryAdd(ref Globals.arrLogHistory, "ERROR MySQL - " + m_ex.Number);
                    break;
            }
        }
        connMySQL.Close();
        Console.WriteLine("Sending data to MySQL - Done");
    }
}