![alt text](https://img.shields.io/github/license/szporwolik/perun.svg "MIT")
![alt text](https://img.shields.io/github/release-pre/szporwolik/perun.svg "Latest release")
![alt text](https://img.shields.io/github/release-date-pre/szporwolik/perun.svg "Latest release date")


# Perun for DCS World

Included lua script extracts data from DCS World multiplayer server and sends information to TCP port and JSON file for further processing. 

Provided windows app puts received TCP packets to MySQL database; additionaly Perun windows application can be used to merge LotATC and DCS SRS data in one database making Perun for DCS World wannabe ultimate integration tool for the server admins.

However, this software is intended to be used by experienced users - the output is MySQL data and/or JSON file; you will need to process/display it yourself.

![Perun in action](https://i.imgur.com/vHw8Xu5.png)
![Data flow](https://i.imgur.com/JbNu77l.png)

### Prerequisites

Core:
 * For JSON Export:
   * DCS World stable or DCS World beta
 * **(Optional)** For MySQL Export:
   * MySQL database with read/write access (tested with v5.7.21)
   * .NET Framework 4.8.0

3rd party applications support:
 * for [DCS SRS](https://github.com/ciribob/DCS-SimpleRadioStandalone/releases) integration location of the clients-list.json file will be required (by default: SRS Server folder), "Auto Export List" option has to be enabled - see [SRS documentation](https://github.com/ciribob/DCS-SimpleRadioStandalone/wiki)
 * for [LotATC](https://www.lotatc.com/) you will need location of stats.json file and proper LotATC configuration with JSON data export enabled - see [LotATC documentation](https://www.lotatc.com/documentation/server_configuration.html)

### Installing

* Download latest [release](https://github.com/szporwolik/perun/releases)
* Copy contents of [01_DCS](https://github.com/szporwolik/perun/tree/master/01_DCS) to your \Scripts folder (located inside DCS folder in your Saved Games)
* **optionaly** Create MySQL database using SQL script located in [03_MySQL](https://github.com/szporwolik/perun/tree/master/03_MySQL)
* **optionaly** Run the Win32 application
	* set MySQL connection data
	* point LotATC json file location
	* point DCS SRS json file location
	* click connect and leave the app running in the background
* Start DCS World and host multiplayer session
  * by default the JSON is written into Scripts\Json folder located in your Saved Games DCS folder tree
  * by default the TCP port 48620 is in use as target port
* Data shall now be filling your MySQL database

## Data packets (for expert users)
* ```ID: 1```, contains version/diagnostic information
* ```ID: 2```, contains status data in the following sections
	* mission - minimal information about mission
	* players - connected players
* ```ID: 3```, available slots list and coalitions
	* coalitions - available coalitions
	* slots - available slots
* ```ID: 4```, stores mission data 
* ```ID: 50```, chat event
* ```ID: 51```, game event
* ```ID: 52```, player stats 
* ```ID: 53```, player login to DCS server
* ```ID: 100```, DCS SRS's client-list.json
* ```ID: 101```, LotATC's stats.json

## Built With

* [VisualStudio 2017 Community](https://visualstudio.microsoft.com/vs/community/) 
* [Notepad++](https://notepad-plus-plus.org/)

## used 3rd party resources

* [Tango Icon Library](http://tango.freedesktop.org/Tango_Icon_Library)

## Contributing

Please contact me if you'd like to contribute.

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/szporwolik/perun/tags). 

## Authors

* **Szymon Porwolik** - *Initial work* - [szporwolik](https://github.com/szporwolik)

See also the list of [contributors](https://github.com/szporwolik/perun/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

* Thanks to [Gildia DCS](https://forum.gildia.org) Polish community of DCS World pilots.
