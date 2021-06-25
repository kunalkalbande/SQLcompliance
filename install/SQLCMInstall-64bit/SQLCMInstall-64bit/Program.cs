using SQLCMInstall_64bit.Properties;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SQLCMInstall_64bit
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Constants.InitializePrivateFonts();

            if (!Environment.Is64BitOperatingSystem)
            {
                IderaMessageBox messageBox = new IderaMessageBox();
                messageBox.Show("This is the 64-bit version of IDERA SQL Compliance Manager and not designed to work on this operating system. 32-bit version of software are available for download from IDERA website", "IDERA SQL Compliance Manager Installer");
                return;
            }

            try
            {
                Process[] processArray = Process.GetProcessesByName("SQLcompliance");
                if (processArray != null && processArray.Length > 0)
                {
                    IderaMessageBox messageBox = new IderaMessageBox();
                    string messageText = "The following application is using the files that needs to be updated by this setup. Close this application first and try running the setup again.\r\n\r\n - ";

                    if (!string.IsNullOrEmpty(processArray[0].MainWindowTitle))
                    {
                        messageText += processArray[0].MainWindowTitle;
                    }
                    else if (!string.IsNullOrEmpty(processArray[0].ProcessName))
                    {
                        messageText += processArray[0].ProcessName;
                    }
                    else
                    {
                        messageText += "SQLcompliance.exe";
                    }

                    messageBox.Show(messageText, Application.ProductName);
                    return;
                }
            }
            catch (Exception ex)
            {
                IderaMessageBox messageBox = new IderaMessageBox();
                messageBox.Show(ex.Message);
            }

            try
            {
                Process[] processArray = Process.GetProcessesByName("SQLCMInstall");
                if (processArray != null && processArray.Length > 0)
                {
                    IderaMessageBox messageBox = new IderaMessageBox();
                    string messageText = "Another instance of IDERA SQL Compliance Manager Installer is already running on this machine. Please close that before proceeding with the installation.";

                    messageBox.Show(messageText, Application.ProductName);
                    return;
                }
            }
            catch (Exception ex)
            {
                IderaMessageBox messageBox = new IderaMessageBox();
                messageBox.Show(ex.Message);
            }

            SplashScreen splash = new SplashScreen();
            splash.Show();
            System.Windows.Forms.Application.DoEvents();

            string msiPackagePath = Application.UserAppDataPath + "\\MSI Packages";
            try
            {
                if (Directory.Exists(msiPackagePath))
                {
                    DirectoryInfo msiPackageLocation = new DirectoryInfo(msiPackagePath);
                    foreach (FileInfo file in msiPackageLocation.GetFiles())
                    {
                        file.Delete();
                    }
                }
                else
                {
                    Directory.CreateDirectory(msiPackagePath);
                }
            }
            catch (Exception ex)
            {
                IderaMessageBox messageBox = new IderaMessageBox();
                messageBox.Show("Unable to create temporary installation directory.\r\nError: " + ex.Message, "IDERA SQL Compliance Manager Installer");
                return;
            }

            try
            {
                using (FileStream fileStream = new FileStream(msiPackagePath + "\\" + Constants.SQLCM, FileMode.Create, FileAccess.Write))
                {
                    MemoryStream memoryStream = new MemoryStream(Resources.SQLcompliance_x64);
                    memoryStream.CopyTo(fileStream);
                    memoryStream.Flush();
                    memoryStream.Dispose();
                    fileStream.Flush();
                    fileStream.Dispose();
                    memoryStream = null;
                }

                using (FileStream fileStream = new FileStream(msiPackagePath + "\\" + Constants.Dashboard, FileMode.Create, FileAccess.Write))
                {
                    MemoryStream memoryStream = new MemoryStream(Resources.IderaDashboard);
                    memoryStream.CopyTo(fileStream);
                    memoryStream.Flush();
                    memoryStream.Dispose();
                    fileStream.Flush();
                    fileStream.Dispose();
                    memoryStream = null;
                }

                using (FileStream fileStream = new FileStream(msiPackagePath + "\\" + Constants.Agent, FileMode.Create, FileAccess.Write))
                {
                    MemoryStream memoryStream = new MemoryStream(Resources.SQLcomplianceAgent_x64);
                    memoryStream.CopyTo(fileStream);
                    memoryStream.Flush();
                    memoryStream.Dispose();
                    fileStream.Flush();
                    fileStream.Dispose();
                    memoryStream = null;
                }

                using (FileStream fileStream = new FileStream(msiPackagePath + "\\" + Constants.Cluster, FileMode.Create, FileAccess.Write))
                {
                    MemoryStream memoryStream = new MemoryStream(Resources.SQLcomplianceClusterSetup_x64);
                    memoryStream.CopyTo(fileStream);
                    memoryStream.Flush();
                    memoryStream.Dispose();
                    fileStream.Flush();
                    fileStream.Dispose();
                    memoryStream = null;
                }

                using (FileStream fileStream = new FileStream(msiPackagePath + "\\" + Constants.Addin, FileMode.Create, FileAccess.Write))
                {
                    MemoryStream memoryStream = new MemoryStream(Resources.SqlCompliancemanager);
                    memoryStream.CopyTo(fileStream);
                    memoryStream.Flush();
                    memoryStream.Dispose();
                    fileStream.Flush();
                    fileStream.Dispose();
                    memoryStream = null;
                }

                using (FileStream fileStream = new FileStream(msiPackagePath + "\\" + Constants.Installer, FileMode.Create, FileAccess.Write))
                {
                    MemoryStream memoryStream = new MemoryStream(Resources.SQLCMInstall);
                    memoryStream.CopyTo(fileStream);
                    memoryStream.Flush();
                    memoryStream.Dispose();
                    fileStream.Flush();
                    fileStream.Dispose();
                    memoryStream = null;
                }

                using (FileStream fileStream = new FileStream(msiPackagePath + "\\" + Constants.InstallerDLL, FileMode.Create, FileAccess.Write))
                {
                    MemoryStream memoryStream = new MemoryStream(Resources.CWFInstallerService);
                    memoryStream.CopyTo(fileStream);
                    memoryStream.Flush();
                    memoryStream.Dispose();
                    fileStream.Flush();
                    fileStream.Dispose();
                    memoryStream = null;
                }

                using (FileStream fileStream = new FileStream(msiPackagePath + "\\" + Constants.FontRegular, FileMode.Create, FileAccess.Write))
                {
                    MemoryStream memoryStream = new MemoryStream(Resources.SourceSansPro_Regular);
                    memoryStream.CopyTo(fileStream);
                    memoryStream.Flush();
                    memoryStream.Dispose();
                    fileStream.Flush();
                    fileStream.Dispose();
                    memoryStream = null;
                }

                using (FileStream fileStream = new FileStream(msiPackagePath + "\\" + Constants.FontSemiBold, FileMode.Create, FileAccess.Write))
                {
                    MemoryStream memoryStream = new MemoryStream(Resources.SourceSansPro_SemiBold);
                    memoryStream.CopyTo(fileStream);
                    memoryStream.Flush();
                    memoryStream.Dispose();
                    fileStream.Flush();
                    fileStream.Dispose();
                    memoryStream = null;
                }

                using (FileStream fileStream = new FileStream(msiPackagePath + "\\" + Constants.FontBold, FileMode.Create, FileAccess.Write))
                {
                    MemoryStream memoryStream = new MemoryStream(Resources.SourceSansPro_Bold);
                    memoryStream.CopyTo(fileStream);
                    memoryStream.Flush();
                    memoryStream.Dispose();
                    fileStream.Flush();
                    fileStream.Dispose();
                    memoryStream = null;
                }

                using (FileStream fileStream = new FileStream(msiPackagePath + "\\" + Constants.SQLNativeClient, FileMode.Create, FileAccess.Write))
                {
                    MemoryStream memoryStream = new MemoryStream(Resources.SQLNCLI_x64);
                    memoryStream.CopyTo(fileStream);
                    memoryStream.Flush();
                    memoryStream.Dispose();
                    fileStream.Flush();
                    fileStream.Dispose();
                    memoryStream = null;
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
            catch (Exception ex)
            {
                IderaMessageBox messageBox = new IderaMessageBox();
                messageBox.Show("Unable to extract installation files.\r\nError: " + ex.Message, "IDERA SQL Compliance Manager Installer");
            }

            try
            {
                RunInstaller(msiPackagePath, "SQLCMInstall.exe");                
                splash.Hide();
            }
            catch (Exception ex)
            {
                IderaMessageBox messageBox = new IderaMessageBox();
                messageBox.Show("Unable to run installer package.\r\nError: " + ex.Message, "IDERA SQL Compliance Manager Installer");
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        private static void RunInstaller(string directoryName, string filename)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.WorkingDirectory = directoryName;
                process.StartInfo.FileName = filename;
                process.Start();

                SetForegroundWindow(process.MainWindowHandle);
            }
            catch
            {
                throw;
            }
        }
    }
}
