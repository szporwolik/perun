// This class gathers all helper functions
using System;
using System.Reflection;

internal class PerunHelper
{
    public static void GUILogHistoryAdd(ref string[] arrLogHistory, string strEntryToAdd, int intDirection = 0, int intMarker = 0)
    {
        // Declare values
        string strDirection;
        string strMarker;
        // Set direction marker
        switch (intDirection)
        {
            case 1:
                strDirection = ">";
                break;
            case 2:
                strDirection = "<";
                break;
            case 3:
                strDirection = "^";
                break;
            default:
                strDirection = " ";
                break;
        }

        // Set marker for user flags (markers)
        strMarker = (intMarker>0) ? "X" : " ";

        // Rotate log history
        for (int i = 0; i < arrLogHistory.Length - 1; i++)
        {
            arrLogHistory[i] = arrLogHistory[i + 1]; // Shift one down
        }

        // Add new entry
        arrLogHistory[arrLogHistory.Length - 1] = DateTime.Now.ToString("HH:mm:ss") + " " + strDirection  + " " + strEntryToAdd; // Add entry at the last position

        // Add the entry to log file
        LogController.WriteLog(DateTime.Now.ToString("yyyy-dd-MM ") + " " + DateTime.Now.ToString("HH:mm:ss") + " | Instance: "+ Globals.intInstanceId + " | " + strMarker + " | " + strDirection + " | " + strEntryToAdd);

        // Update control at my window
        Globals.bGUILogHistoryUpdate = true;
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
        return strBeginning + "v" + Globals.strPerunVersion;
    }
}