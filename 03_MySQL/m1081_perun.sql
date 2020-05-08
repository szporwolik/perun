SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;


DROP TABLE IF EXISTS `pe_Config`;
CREATE TABLE IF NOT EXISTS `pe_Config` (
  `pe_Config_id` int(11) NOT NULL,
  `pe_Config_payload` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`pe_Config_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

INSERT INTO `pe_Config` (`pe_Config_id`, `pe_Config_payload`) VALUES
(1, 'v0.9.5');

DROP TABLE IF EXISTS `pe_DataMissionHashes`;
CREATE TABLE IF NOT EXISTS `pe_DataMissionHashes` (
  `pe_DataMissionHashes_id` bigint(20) NOT NULL AUTO_INCREMENT,
  `pe_DataMissionHashes_instance` smallint(5) UNSIGNED NOT NULL,
  `pe_DataMissionHashes_hash` varchar(150) NOT NULL,
  `pe_DataMissionHashes_datetime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`pe_DataMissionHashes_id`),
  UNIQUE KEY `UNIQUE_hash` (`pe_DataMissionHashes_hash`),
  KEY `pe_DataMissionHashes_instance` (`pe_DataMissionHashes_instance`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `pe_DataPlayers`;
CREATE TABLE IF NOT EXISTS `pe_DataPlayers` (
  `pe_DataPlayers_id` bigint(20) NOT NULL AUTO_INCREMENT,
  `pe_DataPlayers_ucid` varchar(150) NOT NULL,
  `pe_DataPlayers_lastname` varchar(150) DEFAULT NULL,
  `pe_DataPlayers_lastip` varchar(100) DEFAULT NULL,
  `pe_DataPlayers_updated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`pe_DataPlayers_id`),
  UNIQUE KEY `UNIQUE_UCID` (`pe_DataPlayers_ucid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


DROP TABLE IF EXISTS `pe_DataRaw`;
CREATE TABLE IF NOT EXISTS `pe_DataRaw` (
  `pe_dataraw_type` tinyint(4) NOT NULL,
  `pe_dataraw_instance` smallint(5) UNSIGNED NOT NULL,
  `pe_dataraw_payload` text,
  `pe_dataraw_updated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`pe_dataraw_type`,`pe_dataraw_instance`),
  KEY `pe_dataraw_type_pe_dataraw_instance` (`pe_dataraw_type`,`pe_dataraw_instance`),
  KEY `pe_dataraw_updated` (`pe_dataraw_updated`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `pe_DataTypes`;
CREATE TABLE IF NOT EXISTS `pe_DataTypes` (
  `pe_DataTypes_id` int(11) NOT NULL AUTO_INCREMENT,
  `pe_DataTypes_name` varchar(100) NOT NULL,
  `pe_DataTypes_update` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`pe_DataTypes_id`),
  UNIQUE KEY `UNIQUE_TYPE_NAME` (`pe_DataTypes_name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `pe_LogChat`;
CREATE TABLE IF NOT EXISTS `pe_LogChat` (
  `pe_LogChat_id` bigint(20) NOT NULL AUTO_INCREMENT,
  `pe_LogChat_datetime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `pe_LogChat_missionhash_id` bigint(20) DEFAULT NULL,
  `pe_LogChat_playerid` varchar(100) NOT NULL,
  `pe_LogChat_msg` text NOT NULL,
  `pe_LogChat_all` varchar(10) NOT NULL,
  PRIMARY KEY (`pe_LogChat_id`),
  KEY `pe_LogChat_missionhash_id` (`pe_LogChat_missionhash_id`),
  KEY `pe_LogChat_playerid` (`pe_LogChat_playerid`),
  KEY `pe_LogChat_datetime` (`pe_LogChat_datetime`),
  KEY `pe_LogChat_all` (`pe_LogChat_all`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `pe_LogEvent`;
CREATE TABLE IF NOT EXISTS `pe_LogEvent` (
  `pe_LogEvent_id` bigint(20) NOT NULL AUTO_INCREMENT,
  `pe_LogEvent_datetime` datetime DEFAULT CURRENT_TIMESTAMP,
  `pe_LogEvent_missionhash_id` bigint(20) DEFAULT NULL,
  `pe_LogEvent_type` varchar(100) NOT NULL,
  `pe_LogEvent_content` text NOT NULL,
  `pe_LogEvent_arg1` varchar(150) DEFAULT NULL,
  `pe_LogEvent_arg2` varchar(150) DEFAULT NULL,
  `pe_LogEvent_argPlayers` varchar(150) DEFAULT NULL,
  PRIMARY KEY (`pe_LogEvent_id`),
  KEY `pe_LogEvent_missionhash_id` (`pe_LogEvent_missionhash_id`),
  KEY `pe_LogEvent_datetime` (`pe_LogEvent_datetime`),
  KEY `pe_LogEvent_type_2` (`pe_LogEvent_type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `pe_LogLogins`;
CREATE TABLE IF NOT EXISTS `pe_LogLogins` (
  `pe_LogLogins_id` bigint(20) NOT NULL AUTO_INCREMENT,
  `pe_LogLogins_datetime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `pe_LogLogins_instance` smallint(5) UNSIGNED NOT NULL,
  `pe_LogLogins_playerid` bigint(20) DEFAULT NULL,
  `pe_LogLogins_name` varchar(150) NOT NULL,
  `pe_LogLogins_ip` varchar(100) NOT NULL,
  PRIMARY KEY (`pe_LogLogins_id`),
  KEY `pe_LogLogins_playerid` (`pe_LogLogins_playerid`),
  KEY `pe_LogLogins_datetime` (`pe_LogLogins_datetime`),
  KEY `pe_LogLogins_instance` (`pe_LogLogins_instance`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `pe_LogStats`;
CREATE TABLE IF NOT EXISTS `pe_LogStats` (
  `pe_LogStats_id` bigint(20) NOT NULL AUTO_INCREMENT,
  `pe_LogStats_datetime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `pe_LogStats_missionhash_id` bigint(20) DEFAULT NULL,
  `pe_LogStats_playerid` bigint(20) DEFAULT NULL,
  `pe_LogStats_typeid` int(11) DEFAULT NULL,
  `pe_LogStats_masterslot` int(11) DEFAULT NULL,
  `pe_LogStats_seat` int(10) DEFAULT NULL,
  `ps_pvp` int(11) UNSIGNED NOT NULL DEFAULT '0',
  `ps_deaths` int(11) UNSIGNED NOT NULL DEFAULT '0',
  `ps_ejections` int(11) UNSIGNED NOT NULL DEFAULT '0',
  `ps_crashes` int(11) UNSIGNED NOT NULL DEFAULT '0',
  `ps_teamkills` int(11) UNSIGNED NOT NULL DEFAULT '0',
  `ps_kills_planes` int(11) UNSIGNED NOT NULL DEFAULT '0',
  `ps_kills_helicopters` int(11) UNSIGNED NOT NULL DEFAULT '0',
  `ps_kills_air_defense` int(11) UNSIGNED NOT NULL DEFAULT '0',
  `ps_kills_armor` int(11) UNSIGNED NOT NULL DEFAULT '0',
  `ps_kills_unarmed` int(11) UNSIGNED NOT NULL DEFAULT '0',
  `ps_kills_infantry` int(11) UNSIGNED NOT NULL DEFAULT '0',
  `ps_kills_ships` int(11) UNSIGNED NOT NULL DEFAULT '0',
  `ps_kills_fortification` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `ps_kills_other` int(11) UNSIGNED NOT NULL DEFAULT '0',
  `ps_airfield_takeoffs` int(11) UNSIGNED NOT NULL DEFAULT '0',
  `ps_airfield_landings` int(11) UNSIGNED NOT NULL DEFAULT '0',
  `ps_ship_takeoffs` int(11) UNSIGNED NOT NULL DEFAULT '0',
  `ps_ship_landings` int(11) UNSIGNED NOT NULL DEFAULT '0',
  `ps_farp_takeoffs` int(11) UNSIGNED NOT NULL DEFAULT '0',
  `ps_farp_landings` int(11) UNSIGNED NOT NULL DEFAULT '0',
  `ps_other_landings` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `ps_other_takeoffs` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `ps_time` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `pe_LogStats_mstatus` enum('?','RTB','MIA','KIA') DEFAULT NULL,
  PRIMARY KEY (`pe_LogStats_id`),
  UNIQUE KEY `UNIQUE_STATS_PER_MISSION_AND_UCID_AND_TYPE` (`pe_LogStats_playerid`,`pe_LogStats_missionhash_id`,`pe_LogStats_typeid`) USING BTREE,
  KEY `pe_LogStats_missionhash_id` (`pe_LogStats_missionhash_id`),
  KEY `pe_LogStats_playerid` (`pe_LogStats_playerid`),
  KEY `pe_LogStats_typeid` (`pe_LogStats_typeid`),
  KEY `pe_LogStats_masterslot` (`pe_LogStats_masterslot`),
  KEY `pe_LogStats_mstatus` (`pe_LogStats_mstatus`),
  KEY `pe_LogStats_seat` (`pe_LogStats_seat`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TRIGGER IF EXISTS `pe_LogStats_UPDATE`;
DELIMITER $$
CREATE TRIGGER `pe_LogStats_UPDATE` BEFORE UPDATE ON `pe_LogStats` FOR EACH ROW BEGIN
    IF NEW.ps_ejections > 0 THEN
    	SET NEW.pe_LogStats_mstatus = "MIA";
    ELSEIF NEW.ps_deaths > 0 THEN
    	SET NEW.pe_LogStats_mstatus = "KIA";
    ELSEIF (NEW.ps_airfield_landings + NEW.ps_farp_landings + NEW.ps_other_landings + NEW.ps_ship_landings) >= (NEW.ps_airfield_takeoffs + NEW.ps_farp_takeoffs + NEW.ps_other_takeoffs + NEW.ps_ship_takeoffs) THEN
    	IF (NEW.ps_airfield_takeoffs + NEW.ps_farp_takeoffs + NEW.ps_other_takeoffs + NEW.ps_ship_takeoffs) > 0 or (NEW.ps_airfield_landings + NEW.ps_farp_landings + NEW.ps_other_landings + NEW.ps_ship_landings) > 0 THEN
    		SET NEW.pe_LogStats_mstatus = "RTB";
        ELSE 
        	SET NEW.pe_LogStats_mstatus = "?";
        END IF;
    ELSEIF (NEW.ps_airfield_takeoffs + NEW.ps_farp_takeoffs + NEW.ps_other_takeoffs + NEW.ps_ship_takeoffs) > (NEW.ps_airfield_landings + NEW.ps_farp_landings + NEW.ps_other_landings + NEW.ps_ship_landings) and NEW.ps_ejections=0 and NEW.ps_deaths=0 THEN
SET NEW.pe_LogStats_mstatus = "MIA";
    ELSE
    	SET NEW.pe_LogStats_mstatus = "?";
	END IF;						
END
$$
DELIMITER ;


ALTER TABLE `pe_DataMissionHashes` ADD FULLTEXT KEY `pe_DataMissionHashes_hash` (`pe_DataMissionHashes_hash`);

ALTER TABLE `pe_DataPlayers` ADD FULLTEXT KEY `pe_DataPlayers_ucid` (`pe_DataPlayers_ucid`);

ALTER TABLE `pe_DataTypes` ADD FULLTEXT KEY `pe_DataTypes_name` (`pe_DataTypes_name`);

ALTER TABLE `pe_LogChat` ADD FULLTEXT KEY `pe_LogChat_msg` (`pe_LogChat_msg`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
