﻿// This class gathers all helper functions
using System;

internal class PerunHelper
{
    public static void LogHistoryAdd(ref string[] arrLogHistory, string strEntryToAdd)
    {
        // Add entry to log history and rotate
        for (int i = 0; i < arrLogHistory.Length - 1; i++)
        {
            arrLogHistory[i] = arrLogHistory[i + 1]; // Shift one down
        }

        arrLogHistory[arrLogHistory.Length - 1] = DateTime.Now.ToString("HH:mm:ss") + " > " + strEntryToAdd; // Add entry at the last position
    }
    public static string GetAppVersion(string strBeginning)
    {
        // Gets build version
        if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
        {
            System.Deployment.Application.ApplicationDeployment cd = System.Deployment.Application.ApplicationDeployment.CurrentDeployment;
            Globals.strPerunVersion = cd.CurrentVersion.ToString();
        }
        return strBeginning+"v" + Globals.strPerunVersion;
    }
}