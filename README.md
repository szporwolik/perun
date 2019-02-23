# Perun for DCS World

This toolset extracts data from DCS World server and sends information to the local Json file and UDP port. 

Provided windows app puts JSON data to MySQL database. Additionaly Perun windows app can be used to merge LotATC and DCS SRS data in one database making Perun for DCS World wannabe ultimate integration tool for the server admins.

However this software is intended to be used by experienced users - the output is data Json and MySQL; you will need to process/display it yourself.

### Prerequisites

Core:
 * For JSON Export:
   * DCS World stable or DCS World beta
 * **(Optional)** For MySQL Export:
   * MySQL server with read/write access

3rd party support:
 * for [DCS SRS](https://github.com/ciribob/DCS-SimpleRadioStandalone/releases) integration location of the clients-list.json file will be required (by default: SRS Server folder), "Auto Export List" option has to be enabled - see [SRS documentation](https://github.com/ciribob/DCS-SimpleRadioStandalone/wiki)
 * for [LotATC](https://www.lotatc.com/) you will need location of stats.json file and proper LotATC configuration with JSON data export enabled - see [LotATC documentation](https://www.lotatc.com/documentation/server_configuration.html)

### Installing

* Download latest [release](https://github.com/szporwolik/perun/releases), **optionaly** together with Win32 binary file for MySQL export
* Copy contents of [01_DCS](https://github.com/szporwolik/perun/tree/master/01_DCS) to your \Scripts folder (located inside DCS folder in your Saved Games)
* **optionaly** Create MySQL server using SQL script located in [03_MySQL](https://github.com/szporwolik/perun/tree/master/03_MySQL)
* **optionaly** Run the Win32 application
	* set MySQL connection data
	* point LotATC json file location
	* point DCS SRS json file location
	* click connect and leave the app running in the background
* Start DCS World and host multiplayer session
  * by default the JSON is written into Scripts\Json folder located in your Saved Games DCS folder tree
  * by default the UDP port 48620 is in use as target port

## Data packets
* ID: 1, contains version information
* ID: 2, contains status data in the following sections
	* mission
	* players
* ID: 3, available slots list and coalitions
	* coalitions
	* slots
* ID: 50, chat event
* ID: 51, game event
* ID: 100, DCS SRS's client-list.json
* ID: 101, LotATC's stats.json

## Built With

* [VisualStudio 2017 Community](https://visualstudio.microsoft.com/vs/community/) 
* [Notepad++](https://notepad-plus-plus.org/)
* [TortoiseGit](https://tortoisegit.org/)

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
