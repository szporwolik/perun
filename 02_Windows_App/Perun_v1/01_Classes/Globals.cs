// This class gathers all global variable
using MySql.Data.MySqlClient;

internal class Globals
{
    public static string strPerunVersion = "DEBUG";                 // Helper for pulling version definition
    public static string[] arrLogHistory = new string[10];          // Log history for GUI
    public static bool bLogHistoryUpdate = true;                    // Flag if log control requires update
    public static string strPerunTitleText = "";                    // Helper to update title
    public static int intInstanceId = 0;                            // Kepp the instance ID
    public static bool bStatusIconsForce = true;                    // Force main window icons reload
    public static bool bdcConnection = false;                       // Historic db connection status
    public static bool bTCPServer = false;                          // Historic tcp connection status
    public static bool bSRSStatus = false;                          // Historic srs connection status
    public static bool bLotATCStatus = false;                       // Historic lotatc connection status
    public static bool bClientConnected = false;                    // Flag if is TCP connectionstill alive
    public static int intMysqlErros = 0;                            // MySQL - Error counter
    public static int intGameErros = 0;                             // TCP connection - Error counter
    public static int intGameErrosHistory = 0;                      // TCP connection - historic value of Error counter
    public static int intSRSErros = 0;                              // DCS SRS - error counter
    public static int intLotATCErros = 0;                           // LotATC - error counter
}

