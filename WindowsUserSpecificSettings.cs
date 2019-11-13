// C# class with Windows API related to user settings
// David Piao

using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;

namespace OS_Setting
{
    internal class WindowsUserSpecificSettings : IDisposable
    {
        private const uint TOKEN_QUERY = 8;
        private const uint TOKEN_DUPLICATE = 2;
        private const uint TOKEN_IMPERSONATE = 4;
        private const uint TOKEN_ADJUST_PRIVILEGES = 32;
        private const uint SE_PRIVILEGE_ENABLED = 2;
        private const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";
        private IntPtr m_UserToken;
        private SafeRegistryHandle m_RegHandle;
        private RegistryKey m_RegHive;
        private static string _InstallPath;

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, WindowsUserSpecificSettings.LogonType dwLogonType, WindowsUserSpecificSettings.LogonProvider dwLogonProvider, out IntPtr phToken);

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr handle);

        [DllImport("userenv.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool LoadUserProfile(IntPtr hToken, ref WindowsUserSpecificSettings.PROFILEINFO lpProfileInfo);

        [DllImport("userenv.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool UnloadUserProfile(IntPtr hToken, IntPtr hProfile);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool OpenProcessToken(IntPtr ProcessHandle, uint DesiredAccess, out IntPtr TokenHandle);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool LookupPrivilegeValue(string lpSystemName, string lpName, out WindowsUserSpecificSettings.LUID lpLuid);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AdjustTokenPrivileges(IntPtr TokenHandle, [MarshalAs(UnmanagedType.Bool)] bool DisableAllPrivileges, ref WindowsUserSpecificSettings.TOKEN_PRIVILEGES NewState, uint Zero, IntPtr Null1, IntPtr Null2);

        private static void AcquireTokenPriv(string name, uint accessFlags = 40)
        {
            IntPtr TokenHandle = IntPtr.Zero;
            try
            {
                if (!WindowsUserSpecificSettings.OpenProcessToken(Process.GetCurrentProcess().Handle, accessFlags, out TokenHandle))
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to open process token handle");
                WindowsUserSpecificSettings.TOKEN_PRIVILEGES NewState = new WindowsUserSpecificSettings.TOKEN_PRIVILEGES();
                NewState.PrivilegeCount = 1U;
                NewState.Privileges = new WindowsUserSpecificSettings.LUID_AND_ATTRIBUTES[1];
                NewState.Privileges[0].Attributes = 2U;
                if (!WindowsUserSpecificSettings.LookupPrivilegeValue((string)null, name, out NewState.Privileges[0].Luid))
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to look up privilege");
                if (!WindowsUserSpecificSettings.AdjustTokenPrivileges(TokenHandle, false, ref NewState, 0U, IntPtr.Zero, IntPtr.Zero))
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to adjust process token privileges");
            }
            finally
            {
                if (TokenHandle != IntPtr.Zero)
                    WindowsUserSpecificSettings.CloseHandle(TokenHandle);
            }
        }
        public void ToggleTaskManager(bool enabled = false)
        {
            RegistryKey registryKey = this.m_RegHive.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", true);

            if (enabled == false)
                registryKey.SetValue("DisableTaskMgr", "1");
            else
                registryKey.DeleteValue("DisableTaskMgr");
            registryKey.Close();
        }
        public bool IsLocalAdministrator
        {
            get
            {
                using (WindowsIdentity ntIdentity = new WindowsIdentity(this.m_UserToken))
                    return new WindowsPrincipal(ntIdentity).IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        public string Shell
        {
            get
            {
                using (RegistryKey registryKey = this.m_RegHive.OpenSubKey("Software\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon", false))
                    return registryKey.GetValue("shell", (object)null) as string;
            }
            set
            {
                using (RegistryKey registryKey = this.m_RegHive.OpenSubKey("Software\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon", true))
                {
                    if (value == null)
                        registryKey.DeleteValue("shell", false);
                    else
                        registryKey.SetValue("shell", (object)value, RegistryValueKind.String);
                }
            }
        }

        public bool RunAtLogin
        {
            get
            {
                using (RegistryKey registryKey = this.m_RegHive.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", false))
                {
                    if (registryKey == null)
                        return false;
                    string[] valueNames = registryKey.GetValueNames();
                    if (valueNames == null)
                        return false;
                    string name = ((IEnumerable<string>)valueNames).FirstOrDefault<string>((Func<string, bool>)(s => string.Compare(s ?? "", "DeeKiosk Client Platform", true) == 0));
                    if (string.IsNullOrEmpty(name))
                        return false;
                    string strA = registryKey.GetValue(name, (object)null) as string;
                    if (string.IsNullOrEmpty(strA))
                        return false;
                    string strB = "\"" + WindowsUserSpecificSettings.InstallPath + "DeeKioskClient.exe\"";
                    return string.Compare(strA, strB, true) == 0;
                }
            }
            set
            {
                using (RegistryKey subKey = this.m_RegHive.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run"))
                {
                    if (value)
                        subKey.SetValue("DeeKiosk Client Platform", (object)("\"" + WindowsUserSpecificSettings.InstallPath + "DeeKiosk Client.exe\""), RegistryValueKind.String);
                    else
                        subKey.DeleteValue("DeeKiosk Client Platform", false);
                }
            }
        }

        public WindowsUserSpecificSettings(string domain, string user, string pass)
        {
            WindowsUserSpecificSettings.AcquireTokenPriv("SeImpersonatePrivilege", 40U);
            WindowsUserSpecificSettings.AcquireTokenPriv("SeBackupPrivilege", 40U);
            WindowsUserSpecificSettings.AcquireTokenPriv("SeRestorePrivilege", 40U);
            if (!WindowsUserSpecificSettings.LogonUser(user, domain, pass, WindowsUserSpecificSettings.LogonType.LOGON32_LOGON_NETWORK, WindowsUserSpecificSettings.LogonProvider.LOGON32_PROVIDER_DEFAULT, out this.m_UserToken))
                throw new Win32Exception();
            WindowsUserSpecificSettings.PROFILEINFO lpProfileInfo = new WindowsUserSpecificSettings.PROFILEINFO();
            lpProfileInfo.dwFlags = 1;
            lpProfileInfo.lpServerName = string.Empty;
            lpProfileInfo.lpUserName = user;
            lpProfileInfo.dwSize = Marshal.SizeOf((object)lpProfileInfo);
            if (!WindowsUserSpecificSettings.LoadUserProfile(this.m_UserToken, ref lpProfileInfo))
                throw new Win32Exception();
            this.m_RegHandle = new SafeRegistryHandle(lpProfileInfo.hProfile, false);
            this.m_RegHive = RegistryKey.FromHandle(this.m_RegHandle, RegistryView.Default);
        }

        ~WindowsUserSpecificSettings()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            if (this.m_RegHive != null)
            {
                this.m_RegHive.Dispose();
                this.m_RegHive = (RegistryKey)null;
            }
            if (this.m_RegHandle != null)
            {
                WindowsUserSpecificSettings.UnloadUserProfile(this.m_UserToken, this.m_RegHandle.DangerousGetHandle());
                this.m_RegHandle.Dispose();
                this.m_RegHandle = (SafeRegistryHandle)null;
            }
            if (!(this.m_UserToken != IntPtr.Zero))
                return;
            WindowsUserSpecificSettings.CloseHandle(this.m_UserToken);
            this.m_UserToken = IntPtr.Zero;
        }

        internal static string InstallPath
        {
            get
            {
                return WindowsUserSpecificSettings._InstallPath;
            }
            set
            {
                WindowsUserSpecificSettings._InstallPath = value;
                if (string.IsNullOrEmpty(WindowsUserSpecificSettings._InstallPath))
                    return;
                WindowsUserSpecificSettings._InstallPath = WindowsUserSpecificSettings._InstallPath.TrimEnd('\\') + "\\";
            }
        }

        static WindowsUserSpecificSettings()
        {
            WindowsUserSpecificSettings.InstallPath = Assembly.GetExecutingAssembly().Location;
        }

        private struct PROFILEINFO
        {
            public int dwSize;
            public int dwFlags;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpUserName;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpProfilePath;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpDefaultPath;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpServerName;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpPolicyPath;
            public IntPtr hProfile;
        }

        private enum LogonType
        {
            LOGON32_LOGON_INTERACTIVE = 2,
            LOGON32_LOGON_NETWORK = 3,
            LOGON32_LOGON_BATCH = 4,
            LOGON32_LOGON_SERVICE = 5,
            LOGON32_LOGON_UNLOCK = 7,
            LOGON32_LOGON_NETWORK_CLEARTEXT = 8,
            LOGON32_LOGON_NEW_CREDENTIALS = 9,
        }

        private enum LogonProvider
        {
            LOGON32_PROVIDER_DEFAULT,
            LOGON32_PROVIDER_WINNT35,
            LOGON32_PROVIDER_WINNT40,
            LOGON32_PROVIDER_WINNT50,
        }

        private struct LUID
        {
            public uint LowPart;
            public int HighPart;
        }

        private struct LUID_AND_ATTRIBUTES
        {
            public WindowsUserSpecificSettings.LUID Luid;
            public uint Attributes;
        }

        private struct TOKEN_PRIVILEGES
        {
            public uint PrivilegeCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            public WindowsUserSpecificSettings.LUID_AND_ATTRIBUTES[] Privileges;
        }
    }
}
