// This class gathers all global variable
using MySql.Data.MySqlClient;

internal class Globals
{
    public static string strPerunVersion = "DEBUG";             // Helper for pulling version definition
    public static string[] arrLogHistory = new string[10];      // Log history for GUI
    public static bool bLogHistoryUpdate = true;               // Mark if log control require update
    public static string strPerunTitleText = "";
    public static int intInstanceId = 0;                // Instance ID
    public static bool bStatusIconsForce = true;        // Force icons reload
    public static bool bdcConnection = false;           // Historic db connection status
    public static bool bTCPServer = false;             // Historic tcp connection status
    public static bool bSRSStatus = false;              // Historic srs connection status
    public static bool bLotATCStatus = false;           // Historic lotatc connection status
    public static bool bClientConnected = false;        // Is TCP connection alive
    public static int intMysqlErros = 0;                // Error counter
    public static int intGameErros = 0;                 // Error counter
    public static int intGameErrosHistory = 0;                 // Error counter
    public static int intSRSErros = 0;                  // Error counter
    public static int intLotATCErros = 0;               // Error counter
}

