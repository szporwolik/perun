// This class gathers all global variable
using MySql.Data.MySqlClient;

internal class Globals
{
    public static string strPerunVersion = "DEBUG";             // Helper for pulling version definition
    public static string[] arrLogHistory = new string[10];      // Log history for GUI
    public static bool bLogHistoryUpdate = true;               // Mark if log control require update
    public static string strPerunTitleText = "";
   
}

