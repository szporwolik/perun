// This class gathers all global variable

// Class to hold current game state/mission information
class CurrentMissionClass
{

    public string Theatre = "";
    public string Mission = "";
    public string Pause = "";

    public string ToInfoString()
    {
        return this.Mission + " (" + this.Theatre + ") Pause:" + this.Pause;
    }
}

internal class Globals
{
    public static string[] AppLogHistory = new string[10];          // Log history for GUI
    public static bool AppUpdateGUI = true;                         // Flag if log control requires update
    public static string AppTitle = "";                             // Helper to update title
    public static int AppInstanceID = 0;                            // Kepp the instance ID
    public static bool AppForceIconReload = true;                   // Force main window icons reload

    public static bool StatusDatabase = false;                      // Historic db connection status
    public static bool StatusSRS = false;                           // Historic srs connection status
    public static bool StatusLotATC = false;                        // Historic lotatc connection status
    public static bool StatusHistoryConnection = false;             // Historic tcp connection status
    public static bool StatusConnection = false;                    // Flag if is TCP connectionstill alive
    public static int ErrorsDatabase = 0;                           // MySQL - Error counter
    public static int ErrorsGame = 0;                               // TCP connection - Error counter
    public static int ErrorsHistoryGame = 0;                        // TCP connection - historic value of Error counter
    public static int ErrorsSRS = 0;                                // DCS SRS - error counter
    public static int ErrorsLotATC = 0;                             // LotATC - error counter

    public static string VersionDCSHook = "";                       // Version - DCS hook
    public static string VersionDatabase = "";                      // Version - Database
    public static string VersionPerun = "DEBUG";                    // Version - Perun
    public static CurrentMissionClass CurrentMission = new CurrentMissionClass(); // Mission - name
}

