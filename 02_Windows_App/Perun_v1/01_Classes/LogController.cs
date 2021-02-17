using System;
using System.IO;

class LogController
{
    // Singleton class - log controller
    private static LogController _instance = new LogController();   // Singleton instance
    public int level;   // Level of logging

    public static LogController instance
    {
        get
        {
            return _instance;   // Return current instance
        }
    }

    public void WriteLog(int logLevel, string strLog)
    {
        // Write log entry to file
        if (logLevel > this.level) return;  // Check if we shall log it with the current log level, if not - exit

        // Declare variables
        StreamWriter LogStreamWriter;
        FileStream LogFileStream = null;
        DirectoryInfo LogDirectoryInfo = null;
        FileInfo LogFileInfo;

        string LogFileDir = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents") + "\\Perun\\";
        string LogFilePath = LogFileDir + "Perun_Log_" + Globals.AppInstanceID + "_" + System.DateTime.Today.ToString("yyyyMMdd") + "." + "log";
        LogFileInfo = new FileInfo(LogFilePath);
        LogDirectoryInfo = new DirectoryInfo(LogFileInfo.DirectoryName);
        if (!LogDirectoryInfo.Exists) LogDirectoryInfo.Create();
        if (!LogFileInfo.Exists)
        {
            LogFileStream = LogFileInfo.Create();
        }
        else
        {
            try
            {
                LogFileStream = new FileStream(LogFilePath, FileMode.Append);
            }
            catch
            {
                // Do nothing - TBD error handling
            }
        }
        try
        {
            LogStreamWriter = new StreamWriter(LogFileStream);
            LogStreamWriter.WriteLine(strLog);
            LogStreamWriter.Close();
            Globals.LastLogLocation = LogFilePath;
        }
        catch
        {
            // Do nothing - TBD error handling
        }

        // Delete old files - log rotation
        DirectoryInfo di = new DirectoryInfo(LogFileDir);
        FileInfo[] files = di.GetFiles("*.log");

        foreach (FileInfo fi in files)
        {
            if (fi.LastAccessTime < DateTime.Now.AddDays(-7))
            {
                fi.Delete();
            }
        }
    }
}
