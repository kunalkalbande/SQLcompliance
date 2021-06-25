
#region log4netway
//using log4net;

//namespace SQLCM_Installer
//{
//    public class LogHelper
//    {
//        public static log4net.ILog GetLogger(string filePath)
//        {
//            log4net.Repository.Hierarchy.Hierarchy root =LogManager.GetRepository() as log4net.Repository.Hierarchy.Hierarchy;

//            if (root != null)
//            {
//                var rfa = (log4net.Appender.RollingFileAppender)root.Root.GetAppender("InstallAppender");
//                if (rfa != null)
//                {
//                    rfa.File = filePath;
//                    rfa.ActivateOptions();// Apply changes to the appender
//                }
//            }

//            return LogManager.GetLogger("InstallAppender");
//        }
//    }
//}
#endregion
using SQLCM_Installer.Custom_Controls;
using System;
using System.IO;

namespace SQLCM_Installer
{
    public class LogHelper
    {
        private static string filePath = string.Empty;

        public static void SetLogger(string path)
        {
            filePath = path;
        }

        public static void Log(string message)
        {
            try
            {
                if (!string.IsNullOrEmpty(filePath))
                {
                    using (StreamWriter streamWriter = new StreamWriter(filePath,true))
                    {
                        streamWriter.WriteLine(message);
                        streamWriter.Close();
                    }
                }
            }
            catch(Exception ex)
            {
                //log the message
            }
        }
    }
}