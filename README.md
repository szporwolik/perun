# Perun for DCS World

This toolset extracts data from DCS World server and export it to local json file and UDP port. **Optional** windows app puts JSON data to MySQL database.

## Getting Started

You can use this toolkit with or without Windows app. Simple DCS export scripts will create JSON file which can be used for further processing.

### Prerequisites

Core:
 * For JSON Export:
   * DCS World stable or DCS World beta
 * For UDP Export:
   * Windows app which handles UDP readouts
 * **(Optional)** For MySQL Export:
   * MySQL server with read/write access

3rd party support:
 * for [DCS SRS](https://github.com/ciribob/DCS-SimpleRadioStandalone/releases) integration location of the clients-list.json file and SRS configuration with JSON data export enabled - see [SRS documentation](https://github.com/ciribob/DCS-SimpleRadioStandalone/wiki)
 * for [LotATC](https://www.lotatc.com/) integration location of the stats.json file and LotATC configuration with JSON data export enabled - see [LotATC documentation](https://www.lotatc.com/documentation/server_configuration.html)

### Installing

* Download latest [release](https://github.com/szporwolik/perun/releases) **optionaly** together with Win32 binary file for MySQL export
* Copy contents of DCS folder to you \Scripts folder (inside DCS folder in your Saved Games)
* Add the following line to your Export.lua : TBD
* Run the Win32 application
* Start DCS World and host multiplayer session
  * by default the JSON is written into : TBD
  * by default the UDP port TBD is in use

## Testing

TBD

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
