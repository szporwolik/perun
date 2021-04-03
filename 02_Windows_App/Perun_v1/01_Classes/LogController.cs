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

    public void LogError(string strLog, string content = null)
    {
        this.WriteLog(0, strLog, content);
    }

    public void LogWarning(string strLog, string content = null)
    {
        this.WriteLog(1, strLog, content);
    }

    public void LogInfo(string strLog, string content = null)
    {
        this.WriteLog(2, strLog, content);
    }

    public void LogDebug(string strLog, string content = null)
    {
        this.WriteLog(3, strLog, content);
    }

    // TBD - done via https://stackoverflow.com/questions/20185015/how-to-write-log-file-in-c
    public void WriteLog(int logLevel, string strLog, string content = null)
    {
        // Write log entry to file
        if (logLevel > this.level) return;  // Check if we shall log it with the current log level, if not - exit

        // Declare variables
        StreamWriter LogStreamWriter;
        FileStream LogFileStream = null;
        DirectoryInfo LogDirectoryInfo = null;
        FileInfo LogFileInfo;
        string message = strLog;

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
            if (content != null)
            {
                // write the content to a file and add the name of this file to the message
                string contentFilename = "Perun_LogContent_" + System.DateTime.Today.ToString("o").Replace('T', '-').Replace(':', '-').Replace('+','-') + "." + "txt";
                string contentFilepath = LogFileInfo + contentFilename;
                File.WriteAllText(contentFilepath, content);
                message = strLog + " - content stored in " + contentFilename;
            }
            LogStreamWriter = new StreamWriter(LogFileStream);
            LogStreamWriter.WriteLine(message);
            LogStreamWriter.Close();
            Globals.LastLogLocation = LogFilePath;
        }
        catch
        {
            // Do nothing - TBD error handling
        }

        // Delete old files - log rotation
        if (Globals.RotateLogs)
        {
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
}
