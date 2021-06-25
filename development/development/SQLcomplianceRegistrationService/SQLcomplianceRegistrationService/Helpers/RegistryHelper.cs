using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLcomplianceRegistrationService
{
    public static class RegistryHelper
    {
        public static void SetValueInRegistry(string name, Object value)
        {
            var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, (Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32));
            RegistryKey key = hklm.OpenSubKey(@"Software\Idera\SQLCM", true);
            if (key == null)
                key = hklm.CreateSubKey(@"Software\Idera\SQLCM");
            key.SetValue(name, value);
        }

        public static object GetetValueFromRegistry(string name)
        {
            var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, (Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32));
            RegistryKey Key = hklm.OpenSubKey(@"Software\Idera\SQLCM");
            Object value = Key.GetValue(name);
            if (value == null) return "";
            return value.ToString();
        }
    }
}
