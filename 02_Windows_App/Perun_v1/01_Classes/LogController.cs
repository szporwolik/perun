using System;
using System.IO;

class LogController
{
    private static LogController _instance = new LogController();

    public static LogController instance
    {
        get
        {
            return _instance;
        }
    }

    public int level
    {
        get;
        set;
    }

    public void LogError(string strLog)
    {
        this.WriteLog(0, strLog);
    }

    public void LogWarning(string strLog)
    {
        this.WriteLog(1, strLog);
    }

    public void LogInfo(string strLog)
    {
        this.WriteLog(2, strLog);
    }

    public void LogDebug(string strLog)
    {
        this.WriteLog(3, strLog);
    }

    // TBD - done via https://stackoverflow.com/questions/20185015/how-to-write-log-file-in-c
    public void WriteLog(int logLevel, string strLog)
    {
        if (logLevel > this.level) return;

        StreamWriter LogStreamWriter;
        FileStream LogFileStream = null;
        DirectoryInfo LogDirectoryInfo = null;
        FileInfo LogFileInfo;

        string LogFilePath = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents") + "\\Perun\\";
        LogFilePath = LogFilePath + "Perun_Log_" + System.DateTime.Today.ToString("yyyyddMM") + "." + "txt";
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
                // Do nothing 
            }
        }
        try
        {
            LogStreamWriter = new StreamWriter(LogFileStream);
            LogStreamWriter.WriteLine(strLog);
            LogStreamWriter.Close();
        }
        catch
        {
            // Do nothing
        }
    }
}
