using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Installer_form_application
{
    public static class Constants
    {
        public static string dashboardMsiLocation = @"Full\x64\IderaDashboard.msi";
        public static string dashboardx86MsiLocation = @"Full\x86\IderaDashboard.msi";
        public static string SQLCMMsiLocation = @"Full\x64\Idera SQL compliance manager (x64).msi";
        public static string SQLCMx86MsiLocation = @"Full\x86\Idera SQL compliance manager.msi";
        public static string installationFailureErrorMessage = "Idera SQL Compliance Manager Wizard ended prematurely because of an error. To install this program at a later time run Setup again.";
    }
}
