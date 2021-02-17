using System;
using System.Diagnostics;
using System.Management;

// Class to hold all global information

class HardwareMonitorClass
{
    // Class for HW monitoring
    protected PerformanceCounter PerformanceCounterCPU;
    protected PerformanceCounter PerformanceCounterRAM;

    public string LastCurrentCpuUsage = "-1";  // Last measured CPU usage
    public string LastCurrentRamUsage = "-1";  // Last measured RAM usage
    public HardwareMonitorClass()
    {
        // Prepare CPU counter
        PerformanceCounterCPU = new PerformanceCounter();
        PerformanceCounterCPU.CategoryName = "Processor";
        PerformanceCounterCPU.CounterName = "% Processor Time";
        PerformanceCounterCPU.InstanceName = "_Total";

        // Prepare RAM counter
        PerformanceCounterRAM = new PerformanceCounter();
        PerformanceCounterRAM.CategoryName = "Memory";
        PerformanceCounterRAM.CounterName = "% Committed Bytes In Use";
    }
    // Get CPU usage
    public string getCurrentCpuUsage()
    {
        LastCurrentCpuUsage = PerformanceCounterCPU.NextValue().ToString("0.00");
        return LastCurrentCpuUsage;
    }

    // Get available RAM memory
    public string getCurrentRamUsage()
    {
        LastCurrentRamUsage = PerformanceCounterRAM.NextValue().ToString("0.00");
        return LastCurrentRamUsage;
    }
}
class CurrentMissionClass
{
    // Class holding current mission/DCS server status
    public string Theatre = "";         // Mission theathre
    public string Mission = "";         // Mission name

    public string Pause = "";           // Mission pause
    public int PlayerCount = 0;         // Actual player time

    public string ModelTime = "";       // Mission time
    public string RealTime = "";        // Server time
}

internal class Globals
{
    public static string[] AppLogHistory = new string[10];          // Log history for GUI
    public static bool AppUpdateGUI = true;                         // Flag if log control requires update
    public static string AppTitle = "";                             // Helper to update title
    public static int AppInstanceID = 0;                            // Kepp the instance ID
    public static bool AppForceIconReload = true;                   // Force main window icons reload

    public static string[] DatabaseSendBuffer = new string[50];            // MySQL send buffer

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

    public static string LastLogLocation = "";                    // Last used log location

    public static CurrentMissionClass CurrentMission = new CurrentMissionClass();   // Actual mission information single ton

    public static HardwareMonitorClass HardwareMonitor = new HardwareMonitorClass();   // Hardware monitor singleton

    public static DatabaseController DatabaseConnection = new DatabaseController();    // Main MySQL controller
    public static TCPController TCPServer = new TCPController();                       // Main TCP controller
}

