// This class gathers all helper functions
using System;
using System.Reflection;

internal class PerunHelper
{
    public static void GUILogHistoryAdd(ref string[] arrLogHistory, string strEntryToAdd, int intDirection = 0, int intMarker = 0, string strType = " ", bool bSkipGui = false)
    {
        // Declare values
        string LogDirection;
        string LogMarker;
        // Set direction marker
        switch (intDirection)
        {
            case 1:
                LogDirection = ">";
                break;
            case 2:
                LogDirection = "<";
                break;
            case 3:
                LogDirection = "^";
                break;
            default:
                LogDirection = " ";
                break;
        }

        // Set marker for user flags (markers)
        LogMarker = (intMarker>0) ? "X" : " ";

        // Set frame type 
        strType=strType.PadLeft(3, ' ');

        if (!bSkipGui)
        {
            // Rotate log history
            for (int i = 0; i < arrLogHistory.Length - 1; i++)
            {
                arrLogHistory[i] = arrLogHistory[i + 1]; // Shift one down
            }

            // Add new entry
            arrLogHistory[arrLogHistory.Length - 1] = DateTime.Now.ToString("HH:mm:ss") + " " + LogDirection + " " + strEntryToAdd; // Add entry at the last position

            // Update control at my window
            Globals.AppUpdateGUI = true;
        }
        // Add the entry to log file
        LogController.WriteLog(DateTime.Now.ToString("yyyy-dd-MM ") + " " + DateTime.Now.ToString("HH:mm:ss") + " | Instance: "+ Globals.AppInstanceID + " | " + LogMarker + " | "+ LogDirection + " | "+ strType + " | " + strEntryToAdd);
    }

    public static string GetAppVersion(string strBeginning)
    {
        // Gets build version
        Globals.VersionPerun = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        return strBeginning + "v" + Globals.VersionPerun;
    }
}