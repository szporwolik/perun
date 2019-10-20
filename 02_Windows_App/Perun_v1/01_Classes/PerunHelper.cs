// This class gathers all helper functions
using System;
using System.Reflection;

internal class PerunHelper
{
    public static void LogHistoryAdd(ref string[] arrLogHistory, string strEntryToAdd)
    {
        // Rotate log history
        for (int i = 0; i < arrLogHistory.Length - 1; i++)
        {
            arrLogHistory[i] = arrLogHistory[i + 1]; // Shift one down
        }

        // Add new entry
        arrLogHistory[arrLogHistory.Length - 1] = DateTime.Now.ToString("yyyy-dd-MM HH:mm:ss") + " > " + strEntryToAdd; // Add entry at the last position
        
        // Add the entry to log file
        LogController.WriteLog(arrLogHistory[arrLogHistory.Length - 1]);

        // Update control at my window
        Globals.bLogHistoryUpdate = true;
    }
    public static string GetAppVersion(string strBeginning)
    {
        // Gets build version
        if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
        {
            // For network deployed
            System.Deployment.Application.ApplicationDeployment cd = System.Deployment.Application.ApplicationDeployment.CurrentDeployment;
            Globals.strPerunVersion = cd.CurrentVersion.ToString();
        }
        else
        {
            // For other cases
            Globals.strPerunVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
        return strBeginning+"v" + Globals.strPerunVersion;
    }
}