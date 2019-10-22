using System;
using System.IO;

class LogController
{
    // TBD - done via https://stackoverflow.com/questions/20185015/how-to-write-log-file-in-c
    public static void WriteLog(string strLog)
    {
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
