using SQLCMInstall_32bit.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SQLCMInstall_32bit
{
    public static class Constants
    {
        public static string Dashboard = "IderaDashboard.msi";
        public static string SQLCM = "SQLcompliance.exe";
        public static string Agent = "SQLcomplianceAgent.msi";
        public static string Cluster = "SQLcomplianceClusterSetup.exe";
        public static string Installer = "SQLCMInstall.exe";
        public static string Addin = "SqlCompliancemanager.zip";
        public static string InstallerDLL = "CWFInstallerService.dll";
        public static string FontRegular = "SourceSansPro-Regular.ttf";
        public static string FontSemiBold = "SourceSansPro-SemiBold.ttf";
        public static string FontBold = "SourceSansPro-Bold.ttf";
        public static string SQLNativeClient = "sqlncli.msi";

        #region Fonts
        public static PrivateFontCollection fontCollection;
        public static FontFamily SourceSansProRegular;
        public static FontFamily SourceSansProBold;
        public static FontFamily SourceSansProSemiBold;

        public static void InitializePrivateFonts()
        {
            fontCollection = new PrivateFontCollection();
            int fontLength = Resources.SourceSansPro_Regular.Length;
            byte[] fontData = Resources.SourceSansPro_Regular;
            IntPtr data = Marshal.AllocCoTaskMem(fontLength);
            try
            {
                Marshal.Copy(fontData, 0, data, fontLength);
                fontCollection.AddMemoryFont(data, fontLength);
            }
            catch (Exception ex)
            {
                // TODO - Log failure
            }
            finally
            {
                Marshal.FreeCoTaskMem(data);
            }
            SourceSansProRegular = fontCollection.Families[0];

            fontCollection = null;
            fontCollection = new PrivateFontCollection();
            int fontLengthSemiBold = Resources.SourceSansPro_SemiBold.Length;
            byte[] fontDataSemiBold = Resources.SourceSansPro_SemiBold;
            IntPtr dataSemiBold = Marshal.AllocCoTaskMem(fontLengthSemiBold);
            try
            {
                Marshal.Copy(fontDataSemiBold, 0, dataSemiBold, fontLengthSemiBold);
                fontCollection.AddMemoryFont(dataSemiBold, fontLengthSemiBold);
            }
            catch (Exception ex)
            {
                // TODO - Log failure
            }
            finally
            {
                Marshal.FreeCoTaskMem(dataSemiBold);
            }
            SourceSansProSemiBold = fontCollection.Families[0];

            fontCollection = null;
            fontCollection = new PrivateFontCollection();
            int fontLengthBold = Resources.SourceSansPro_Bold.Length;
            byte[] fontDataBold = Resources.SourceSansPro_Bold;
            IntPtr dataBold = Marshal.AllocCoTaskMem(fontLengthBold);
            try
            {
                Marshal.Copy(fontDataBold, 0, dataBold, fontLengthBold);
                fontCollection.AddMemoryFont(dataBold, fontLengthBold);
            }
            catch (Exception ex)
            {
                // TODO - Log failure
            }
            finally
            {
                Marshal.FreeCoTaskMem(dataBold);
            }
            SourceSansProBold = fontCollection.Families[0];
        }
        #endregion
    }
}
