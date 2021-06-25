using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CWFInstallerService;

// used for account validation
using System.DirectoryServices.AccountManagement;

// for reading registry values
using Microsoft.Win32;
using System.Threading;
using System.IO;


namespace Installer_form_application
{
    public partial class Introduction : Form
    {

        public Introduction()
        {
            InitializeComponent();
            MinimizeBox = false;
            MaximizeBox = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            EULA eula = new EULA(this);
            eula.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                Validator.ValidateIfDotNet40FullInstalled();
                Validator.ValidateIfOperatingSystemCompatible();
            }
            catch (CWFBaseException ex)
            {
                MessageBox.Show(ex.ErrorCode + " - " + ex.ErrorMessage, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("Do you really want to exit?", "Exit", MessageBoxButtons.YesNo))
            {
                Application.Exit();
            }
        }

        private void linkLabelHere_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(CWFInstallerService.Constants.ideraVideoLink);
        }

        private void linkLabelGuide_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(CWFInstallerService.Constants.ideraGuideLink);
        }

        private void Introduction_Load(object sender, EventArgs e)
        {
            CheckXEventDllExists();
            //Check64BitOS();
        }

        public void CheckXEventDllExists()
        {
            if (!(loadAssembly("Microsoft.SqlServer.XEvent.Linq.dll") || loadAssemblyFromGac32("Microsoft.SqlServer.XEvent.Linq.dll") || loadAssemblyFromGac64("Microsoft.SqlServer.XEvent.Linq.dll")))
            {
                new LinqError().ShowDialog();
            }
        }

        public static bool loadAssemblyFromGac32(string dll)
        {
            string start_search_path;

            start_search_path = @"C:\Windows\Microsoft.NET\assembly\GAC_32";
            if (loadXEventLinqAssemblyFromGac(start_search_path, dll))
                return true;
            return false;
        }

        public static bool loadAssemblyFromGac64(string dll)
        {
            string start_search_path;

            start_search_path = @"C:\Windows\Microsoft.NET\assembly\GAC_64";
            if (loadXEventLinqAssemblyFromGac(start_search_path, dll))
                return true;
            return false;
        }

        private static bool loadXEventLinqAssemblyFromGac(string start_search_path, string dll)
        {
            if (!Directory.Exists(start_search_path))
            {
                return false;
            }
            DirectoryInfo start_dir = new DirectoryInfo(start_search_path);
            DirectoryInfo[] SQLDirs = start_dir.GetDirectories();
            foreach (DirectoryInfo d in SQLDirs)
            {
                DirectoryInfo[] InDirs = d.GetDirectories();
                foreach (DirectoryInfo di in InDirs)
                {
                    string dll_path = di.FullName + "\\" + dll;

                    if (File.Exists(dll_path))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool loadAssembly(string dll)
        {
            string start_search_path;
            string end_search_path;
            string sqlPath = String.Empty;
            try
            {
                int version = 0;
                using (RegistryKey sqlServerKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server"))
                {
                    foreach (string subKeyName in sqlServerKey.GetSubKeyNames())
                    {
                        if (subKeyName.Equals("130") || subKeyName.Equals("120")
                            || subKeyName.Equals("110") || subKeyName.Equals("100") || subKeyName.Equals("90"))
                        {
                            int temp = 0;
                            int.TryParse(subKeyName, out temp);
                            if (version < temp)
                            {
                                version = temp;
                            }
                        }
                    }
                    if (version != 0)
                        sqlPath = sqlServerKey.OpenSubKey(version.ToString()).GetValue("VerSpecificRootDir").ToString();
                }
                sqlPath = sqlPath.Substring(0, sqlPath.IndexOf(version.ToString()));
            }
            catch (Exception e)
            {
            }
            start_search_path = @"C:\Program Files\Microsoft SQL Server\";
            if (sqlPath != String.Empty)
                start_search_path = sqlPath;
            end_search_path = @"Shared\";
            if (loadXEventLinqAssembly(start_search_path, end_search_path, dll))
                return true;
            return false;
        }

        private static bool loadXEventLinqAssembly(string start_search_path, string end_search_path, string dll)
        {
            if (!Directory.Exists(start_search_path))
            {
                return false;
            }
            DirectoryInfo start_dir = new DirectoryInfo(start_search_path);
            DirectoryInfo[] SQLDirs = start_dir.GetDirectories();
            foreach (DirectoryInfo d in SQLDirs)
            {
                string dll_path = d.FullName + "\\" + end_search_path + dll;

                if (File.Exists(dll_path))
                {
                    return true;
                }
            }
            return false;
        }

        private string CheckLinqAssemblyPath(string start_search_path, string end_search_path, string dll)
        {
            if (!Directory.Exists(start_search_path))
            {
                return " ";
            }
            DirectoryInfo start_dir = new DirectoryInfo(start_search_path);
            DirectoryInfo[] SQLDirs = start_dir.GetDirectories();
            foreach (DirectoryInfo d in SQLDirs)
            {
                string dll_path = d.FullName + "\\" + end_search_path + dll;

                if (File.Exists(dll_path))
                {
                    return dll_path;
                }
            }
            return " ";
        }

        public void Check64BitOS()
        {
            string LinqFileName = "Microsoft.SqlServer.XEvent.Linq.dll";
            string LinqPath = String.Empty;
            string LinqPathX86 = String.Empty;
            string start_search_path = String.Empty;
            string end_search_path = String.Empty;
            string start_search_path_X86 = String.Empty;
            string end_search_path_X86 = String.Empty;
            start_search_path = @"C:\Program Files\Microsoft SQL Server\";
            end_search_path = @"Shared\";
            LinqPath = CheckLinqAssemblyPath(start_search_path, end_search_path, LinqFileName);
            start_search_path_X86 = @"C:\Program Files (x86)\Microsoft SQL Server\";
            end_search_path_X86 = @"Tools\Binn\ManagementStudio\Extensions\Application\";
            LinqPathX86 = CheckLinqAssemblyPath(start_search_path_X86, end_search_path_X86, LinqFileName);

            //This is a special case that has been added in SQLCM
            if (Environment.Is64BitOperatingSystem)
            {
                if (LinqPath.Equals(" "))
                {
                    MessageBox.Show("The file Microsoft.SqlServer.XEvent.Linq.dll is missing.You can install the file by installing Client Tools SDK from the SQL Server setup. If you don't have SQL Server installed then you can install the file from the most recent version of the Full SQL Server Feature Pack.Do not use the SP version of the SQL Server Feature Pack.", "SQL Server Version Incompatible", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
            }
            else
            {
                if (LinqPathX86.Equals(" "))
                {
                    MessageBox.Show("The file Microsoft.SqlServer.XEvent.Linq.dll is missing.You can install the file by installing Client Tools SDK from the SQL Server setup. If you don't have SQL Server installed then you can install the file from the most recent version of the Full SQL Server Feature Pack.Do not use the SP version of the SQL Server Feature Pack.", "SQL Server Version Incompatible", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
            }
        }
    }
}
