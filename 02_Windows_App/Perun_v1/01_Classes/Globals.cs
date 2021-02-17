using System;
using System.Diagnostics;
using System.Management;

// Class to hold all global information

class HardwareMonitorClass
{

    protected PerformanceCounter cpuCounter;
    protected PerformanceCounter ramCounter;
    protected PerformanceCounter cputempCounter;

    public HardwareMonitorClass()
    {
        // Prepare CPU counter
        cpuCounter = new PerformanceCounter();
        cpuCounter.CategoryName = "Processor";
        cpuCounter.CounterName = "% Processor Time";
        cpuCounter.InstanceName = "_Total";

        // Prepare RAM counter
        ramCounter = new PerformanceCounter();
        ramCounter.CategoryName = "Memory";
        ramCounter.CounterName = "% Committed Bytes In Use";

    }
    // Get CPU usage
    public string getCurrentCpuUsage()
    {
        return cpuCounter.NextValue().ToString("0.00");
    }

    // Get available RAM memory
    public string getAvailableRAM()
    {
        return ramCounter.NextValue().ToString("0.00");
    }

}
class CurrentMissionClass
{

    public string Theatre = "";         // Mission theathre
    public string Mission = "";         // Mission name

    public string Pause = "";           // Mission pause
    public int PlayerCount = 0;         // Actual player time

    public string ModelTime = "";
    public string RealTime = "";

    public string ToInfoString()
    {
        // Return mission information as string
        if (this.Mission != "")
        {
            // Return mission information as string
            return "Mission: " + this.Mission + "(" + this.Theatre + ") Pause:" + this.Pause + " Players: " + this.PlayerCount;
        } else
        {
            // Mission information is not available
            return "Mission: Unknown";
        }
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

    public static CurrentMissionClass CurrentMission = new CurrentMissionClass();   // Actual mission information

    public static HardwareMonitorClass HardwareMonitor = new HardwareMonitorClass();
}

