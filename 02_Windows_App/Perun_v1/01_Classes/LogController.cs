using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class LogController
{
    // TBD - via https://stackoverflow.com/questions/20185015/how-to-write-log-file-in-c
    public static void WriteLog(string strLog)
    {
        StreamWriter log;
        FileStream fileStream = null;
        DirectoryInfo logDirInfo = null;
        FileInfo logFileInfo;

        string logFilePath = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents") + "\\Perun\\";
        logFilePath = logFilePath + "Perun_Log_" + System.DateTime.Today.ToString("yyyyddMM") + "." + "txt";
        logFileInfo = new FileInfo(logFilePath);
        logDirInfo = new DirectoryInfo(logFileInfo.DirectoryName);
        if (!logDirInfo.Exists) logDirInfo.Create();
        if (!logFileInfo.Exists)
        {
            fileStream = logFileInfo.Create();
        }
        else
        {
            try
            {
                fileStream = new FileStream(logFilePath, FileMode.Append);
            }
            catch
            {

            }
        }
        try
        {
            log = new StreamWriter(fileStream);
            log.WriteLine(strLog);
            log.Close();
        }
        catch
        {

        }
    }
}
