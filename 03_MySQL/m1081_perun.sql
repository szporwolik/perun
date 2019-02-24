
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
-- Struktura tabeli dla tabeli `pe_DataRaw`
--

DROP TABLE IF EXISTS `pe_DataRaw`;
CREATE TABLE IF NOT EXISTS `pe_DataRaw` (
  `pe_dataraw_type` tinyint(4) NOT NULL AUTO_INCREMENT,
  `pe_dataraw_payload` text NOT NULL,
  `pe_dataraw_updated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`pe_dataraw_type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `pe_LogChat`
--

DROP TABLE IF EXISTS `pe_LogChat`;
CREATE TABLE IF NOT EXISTS `pe_LogChat` (
  `pe_LogChat_id` bigint(20) NOT NULL AUTO_INCREMENT,
  `pe_LogChat_datetime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
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
  `pe_LogEvent_missionhash` varchar(150) DEFAULT NULL,
  `pe_LogEvent_datetime` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `pe_LogEvent_type` varchar(100) NOT NULL,
  `pe_LogEvent_content` text NOT NULL,
  PRIMARY KEY (`pe_LogEvent_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
