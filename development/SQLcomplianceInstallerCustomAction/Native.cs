using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace SQLcomplianceInstallerCustomAction
{
    internal class Native
    {
        public const int LOGON32_LOGON_INTERACTIVE = 2;
        public const int LOGON32_LOGON_NETWORK = 3;
        public const int LOGON32_LOGON_BATCH = 4;
        public const int LOGON32_LOGON_SERVICE = 5;
        public const int LOGON32_LOGON_UNLOCK = 7;
        public const int LOGON32_LOGON_NETWORK_CLEARTEXT = 8;
        public const int LOGON32_LOGON_NEW_CREDENTIALS = 9;

        public const int LOGON32_PROVIDER_DEFAULT = 0;
        public const int LOGON32_PROVIDER_WINNT50 = 3;
        public const int LOGON32_PROVIDER_WINNT40 = 2;

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool LogonUser(String lpszUserName, String lpszDomain, String lpszPassword, int dwLogonType, int dwLogonProvider, out IntPtr phToken);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal extern static bool DuplicateToken(IntPtr ExistingTokenHandle, int SECURITY_IMPERSONATION_LEVEL, out IntPtr DuplicateTokenHandle);

        [DllImport("Kernel32.dll", SetLastError = true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        internal static extern bool CloseHandle(IntPtr handle);

        [DllImport("user32.dll")]
        internal static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll")]
        internal static extern bool EmptyClipboard();

        [DllImport("user32.dll")]
        internal static extern bool CloseClipboard();

        [DllImport("user32.dll")]
        internal static extern bool SetClipboardData(uint uFormat, IntPtr data);
    }
}
