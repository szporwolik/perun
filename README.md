![Discord](https://img.shields.io/discord/641759751780302866)
![alt text](https://img.shields.io/github/license/szporwolik/perun.svg "MIT")
![alt text](https://img.shields.io/github/release-pre/szporwolik/perun.svg "Latest release")
![alt text](https://img.shields.io/github/release-date-pre/szporwolik/perun.svg "Latest release date")

![Perun logo](https://i.imgur.com/PrIkqNA.png)

# Perun for DCS World
Included lua script extracts data from DCS World multiplayer server and sends information to TCP port for further processing. 

Provided windows app puts received TCP packets to MySQL database; additionaly Perun windows application can be used to merge LotATC and DCS SRS data in one database making Perun for DCS World wannabe ultimate integration tool for the server admins.

However, this software is intended to be used by experienced users - the output is MySQL data; you will need to process/display it yourself.

![Perun in action](https://i.imgur.com/JIlLENa.png)
![Data flow](https://i.imgur.com/JbNu77l.png)

Discord server: https://discord.gg/MTahREx

## Prerequisites
Core:
   * MySQL database with read/write access running at min. 5.7 server (tested with v5.7.21; native JSON support is required)
   * .NET Framework 4.8.0

3rd party applications support:
 * for [DCS SRS](https://github.com/ciribob/DCS-SimpleRadioStandalone/releases) integration location of the clients-list.json file will be required (by default: SRS Server folder), "Auto Export List" option has to be enabled - see [SRS documentation](https://github.com/ciribob/DCS-SimpleRadioStandalone/wiki)
 * for [LotATC](https://www.lotatc.com/) you will need location of stats.json file and proper LotATC configuration with JSON data export enabled - see [LotATC documentation](https://www.lotatc.com/documentation/server_configuration.html)

## Installing
* Download latest [release](https://github.com/szporwolik/perun/releases)
* Copy contents of [01_DCS](https://github.com/szporwolik/perun/tree/master/01_DCS) to your \Scripts folder (located inside DCS folder in your Saved Games)
* Create MySQL database using SQL script located in [03_MySQL](https://github.com/szporwolik/perun/tree/master/03_MySQL); note that you need just a one database per DCS server machine - multiple instances pushing data to the one database are supported

## Running
* Start DCS World and host multiplayer session
  * by default the TCP port 48620 is in use as target port - ensure that port and instance ID in the options sections of the Lua file matches data 
* run the Win32 application
	* set MySQL connection data
	* **optionaly** point LotATC json file location
	* **optionaly** point DCS SRS json file location
	* click connect and leave the app running in the background
	* **data shall now be filling your MySQL database**

### Windows APP - command line arguments (for expert users)
Windows app supports command line arguments, what may be handy in case of multiple instance of DCS servers running at the same machine.

The following arguments are accepted (keep the order!):
* server port (shall be the same as in the lua file!)
* instance ID (shall be the same as in the lua file!)
* path to SRS client-list.json
* path to LotATC stats.json
* 1 - numeric value (without quotes), set 1 for Autostart (Perun will start within 5 secounds after starting up)

Example for windows shortcut:
```
C:\Perun_v1\Perun.exe 48621 1 "G:\DCS SRS\clients-list.json" "C:\Users\DCS\Saved Games\DCS\Mods\tech\LotAtc\stats.json"
```
## Displaying data
Data displaying and handling is not in the current scope of this project. Since the end of 2018 the Perun data displaying has been  hardcoded at forum.gildia.org , website for the Polish DCS community; examples:
![Who is online](https://i.imgur.com/5lVwsJw.png)
![Mission statistics](https://i.imgur.com/uiRSa9e.png)
Due to resource limitations, "Perun for DCS World" will focus on pulling the data from DCS (and external modules) and pushing it to MySQL server, but if anyone would like to develop open sourced PHP applets for data/statistics displaying we're willing to cooperate and support such projects. Unfortunetly it's not possible to share the hardcoded code from forum.gildia.org.

Basic example was provided [HERE](/04_Misc/05_PHP_Example/index.php) , support is available via Perun community at our [Discord](https://discord.gg/MTahREx).

## Troubleshooting - FAQ
- [I keep getting 1305 MySQL error](#i-keep-getting-1305-mysql-error)
- [Carrier landing are not tracked correctly](#carrier-landing-are-not-tracked-correctly)

### I keep getting 1305 MySQL error
That probably means that your database does not support JSON functions, you shall upgrade your MySQL server to at lease 5.7 version.

### I keep getting 1364 MySQL error
Make sure that STRICT_TRANS_TABLES is disabled at your MySQL server, see: https://stackoverflow.com/questions/37964325/how-to-find-and-disable-mysql-strict-mode

### Carrier landing are not tracked correctly
DCS API does not track carrier or FARP operations natively, so there is a trick to achive that. At this point of time to detect FARP operations, the FARPs shall have "FARP" string in the group name (set in mission editor). For carrier operations the string "SHIP" is required within group name.

# API documentation (for expert users)
## MySQL database structure
* table: pe_DataMissionHashes - holds mission hashes, unique hashes created per each mission start at server
* table: pe_DataPlayers - holds players UCIDs together with last login information
* table: pe_DataRaw - holds raw data received from perun windows app, containing debug/version information together with DCS SRS and LotATC data
* table: pe_DataTypes - names of all model depende class names detected in DCS
* table: pe_LogChat - holds chat history
* table: pe_LogEvent - holds event history
* table: pe_LogLogins - holds login to server event history
* table: pe_LogStats - holds static information

![Database Scheme](https://i.imgur.com/dmJ2pIY.png "MIT")

## Data packets - send from lua to TCP port 
* ```ID: 1```, contains version/diagnostic information
	* perun version - for server administration usage
	* player count - actual number of connected players
* ```ID: 2```, contains status data in the following sections
	* mission name - name of the mission file
	* model time - simulation time
	* real time - real time
	* pause - information if server is paused
	* multiplayer - information if this is multiplayer game (always true for dedicated server usage)
	* theather - information about area of operation
	* weather - basic weather information
	* players - connected players
* ```ID: 3```, available slots list and coalitions
	* coalitions - available coalitions
	* slots - available slots
* ```ID: 4```, stores mission data ; idea is to have whole DCS.getCurrentMission() result - currenlty commented out due to performance issues
* ```ID: 50```, chat event
* ```ID: 51```, game event
* ```ID: 52```, player stats ; note that as DSC native stats are not reliable, seperate stats couting methods are used
* ```ID: 53```, player login to DCS server
* ```ID: 100```, DCS SRS's client-list.json
* ```ID: 101```, LotATC's stats.json

# Project information
## Built With
* [VisualStudio 2017 Community](https://visualstudio.microsoft.com/vs/community/) 
* [Notepad++](https://notepad-plus-plus.org/)

## Contributing
Please contact me if you'd like to contribute. Short rule is that all pull request shall come to "dev" branch, pull requests to "master" will be deleted.

## Versioning
We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/szporwolik/perun/tags). 

## Authors
* **Szymon Porwolik** - *Initial work* - [szporwolik](https://github.com/szporwolik)

See also the list of [contributors](https://github.com/szporwolik/perun/contributors) who participated in this project.

## License
This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments
![Gildia.org Logo](https://images.weserv.nl/?url=https://i.imgur.com/nFHxQMy.png&w=140&il)

Thanks to [Gildia DCS](https://forum.gildia.org) Polish community of DCS World pilots.
