
SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Baza danych: `m1081_perun`
--
CREATE DATABASE IF NOT EXISTS `m1081_perun` DEFAULT CHARACTER SET utf8 COLLATE utf8_general_ci;
USE `m1081_perun`;

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `pe_DataMissionHashes`
--

DROP TABLE IF EXISTS `pe_DataMissionHashes`;
CREATE TABLE IF NOT EXISTS `pe_DataMissionHashes` (
  `pe_DataMissionHashes_id` bigint(20) NOT NULL AUTO_INCREMENT,
  `pe_DataMissionHashes_instance` smallint(5) UNSIGNED NOT NULL,
  `pe_DataMissionHashes_hash` varchar(150) NOT NULL,
  `pe_DataMissionHashes_datetime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`pe_DataMissionHashes_id`),
  UNIQUE KEY `UNIQUE_hash` (`pe_DataMissionHashes_hash`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `pe_DataPlayers`
--

DROP TABLE IF EXISTS `pe_DataPlayers`;
CREATE TABLE IF NOT EXISTS `pe_DataPlayers` (
  `pe_DataPlayers_id` bigint(20) NOT NULL AUTO_INCREMENT,
  `pe_DataPlayers_ucid` varchar(150) NOT NULL,
  `pe_DataPlayers_lastip` varchar(100) NOT NULL,
  `pe_DataPlayers_lastname` varchar(150) NOT NULL,
  `pe_DataPlayers_updated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`pe_DataPlayers_id`),
  UNIQUE KEY `UNIQUE_UCID` (`pe_DataPlayers_ucid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `pe_DataRaw`
--

DROP TABLE IF EXISTS `pe_DataRaw`;
CREATE TABLE IF NOT EXISTS `pe_DataRaw` (
  `pe_dataraw_type` tinyint(4) NOT NULL,
  `pe_dataraw_instance` smallint(5) UNSIGNED NOT NULL,
  `pe_dataraw_payload` text NOT NULL,
  `pe_dataraw_updated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`pe_dataraw_type`,`pe_dataraw_instance`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `pe_LogChat`
--

DROP TABLE IF EXISTS `pe_LogChat`;
CREATE TABLE IF NOT EXISTS `pe_LogChat` (
  `pe_LogChat_id` bigint(20) NOT NULL AUTO_INCREMENT,
  `pe_LogChat_datetime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `pe_LogChat_missionhash_id` bigint(20) DEFAULT NULL,
  `pe_LogChat_playerid` varchar(100) NOT NULL,
  `pe_LogChat_msg` text NOT NULL,
  `pe_LogChat_all` varchar(10) NOT NULL,
  PRIMARY KEY (`pe_LogChat_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `pe_LogEvent`
--

DROP TABLE IF EXISTS `pe_LogEvent`;
CREATE TABLE IF NOT EXISTS `pe_LogEvent` (
  `pe_LogEvent_id` bigint(20) NOT NULL AUTO_INCREMENT,
  `pe_LogEvent_missionhash_id` bigint(20) DEFAULT NULL,
  `pe_LogEvent_datetime` datetime DEFAULT CURRENT_TIMESTAMP,
  `pe_LogEvent_type` varchar(100) NOT NULL,
  `pe_LogEvent_content` text NOT NULL,
  PRIMARY KEY (`pe_LogEvent_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `pe_LogLogins`
--

DROP TABLE IF EXISTS `pe_LogLogins`;
CREATE TABLE IF NOT EXISTS `pe_LogLogins` (
  `pe_LogLogins_id` bigint(20) NOT NULL AUTO_INCREMENT,
  `pe_LogLogins_datetime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `pe_LogLogins_instance` smallint(5) UNSIGNED NOT NULL,
  `pe_LogLogins_playerid` bigint(20) DEFAULT NULL,
  `pe_LogLogins_name` varchar(150) NOT NULL,
  `pe_LogLogins_ip` varchar(100) NOT NULL,
  PRIMARY KEY (`pe_LogLogins_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `pe_LogStats`
--

DROP TABLE IF EXISTS `pe_LogStats`;
CREATE TABLE IF NOT EXISTS `pe_LogStats` (
  `pe_LogStats_id` bigint(20) NOT NULL AUTO_INCREMENT,
  `pe_LogStats_missionhash_id` bigint(20) DEFAULT NULL,
  `pe_LogStats_datetime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `pe_LogStats_playerid` bigint(20) DEFAULT NULL,
  `pe_LogStats_debug` text,
  PRIMARY KEY (`pe_LogStats_id`),
  UNIQUE KEY `UNIQUE_STATS_PER_MISSION_AND_UCID` (`pe_LogStats_playerid`,`pe_LogStats_missionhash_id`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
