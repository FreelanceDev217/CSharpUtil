// Windows API functions to be used for C# applications
// David Piao

#pragma warning disable 0649
#pragma warning disable 0168
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace OS_Util
{
    internal static class OS_Win
    {
        private static readonly IntPtr WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero;
        public static IntPtr NullHandle = IntPtr.Zero;
        public static IntPtr InvalidHandleValue = new IntPtr(-1);
        public static readonly Guid GUID_PRINTER_INSTALL_CLASS = new Guid(1295444345U, (ushort)58149, (ushort)4558, (byte)191, (byte)193, (byte)8, (byte)0, (byte)43, (byte)225, (byte)3, (byte)24);
        public static readonly Guid GUID_DEVINTERFACE_USBPRINT = new Guid(685215661, (short)23058, (short)4561, (byte)174, (byte)91, (byte)0, (byte)0, (byte)248, (byte)3, (byte)168, (byte)194);
        public static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
        private const uint TOKEN_QUERY = 8;
        private const uint TOKEN_DUPLICATE = 2;
        private const uint TOKEN_IMPERSONATE = 4;
        private const uint TOKEN_ADJUST_PRIVILEGES = 32;
        private const uint SE_PRIVILEGE_ENABLED = 2;
        private const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";
        private const int WTS_CURRENT_SESSION = -1;
        public const uint QS_ALLINPUT = 1279;
        public const uint INPUT_MOUSE = 0;
        public const uint INPUT_KEYBOARD = 1;
        public const uint INPUT_HARDWARE = 2;
        public const uint KEYEVENTF_KEYUP = 2;
        public const uint KEYEVENTF_UNICODE = 4;
        public const uint MOUSEEVENTF_MOVE = 1;
        public const uint MOUSEEVENTF_ABSOLUTE = 32768;
        public const uint SPIF_SENDWININICHANGE = 2;
        public const uint SPIF_UPDATEINIFILE = 1;
        public const uint SPI_SETWORKAREA = 47;
        public const uint SPI_GETWORKAREA = 48;
        public const uint SPI_GETMINIMIZEDMETRICS = 43;
        public const uint SPI_SETMINIMIZEDMETRICS = 44;
        public const int ARW_HIDE = 8;
        public const int MONITOR_DEFAULTTONEAREST = 2;
        public const int SW_SHOW = 5;
        public const int SW_HIDE = 0;
        public const int SW_SHOWNOACTIVATE = 4;
        public const int SW_MAXIMIZE = 3;
        public const int SW_MINIMIZE = 6;
        public const int SW_RESTORE = 9;
        public const int HWND_TOPMOST = -1;
        public const int HWND_NOTOPMOST = -2;
        public const int HWND_BOTTOM = 1;
        public const uint WS_EX_TOOLWINDOW = 128;
        public const uint WS_EX_NOACTIVATE = 134217728;
        public const uint WS_CHILD = 1073741824;
        public const uint WS_CLIPCHILDREN = 33554432;
        public const uint WS_CLIPSIBLINGS = 67108864;
        public const uint WS_THICKFRAME = 262144;
        public const uint WS_EX_TRANSPARENT = 32;
        public const uint WS_EX_COMPOSITED = 33554432;
        public const uint WS_EX_LAYERED = 524288;
        public const uint CS_NOCLOSE_BUTTON = 512;
        public const uint SWP_NOSIZE = 1;
        public const uint SWP_NOMOVE = 2;
        public const uint SWP_NOZORDER = 4;
        public const uint SWP_NOREDRAW = 8;
        public const uint SWP_NOACTIVATE = 16;
        public const uint SWP_FRAMECHANGED = 32;
        public const uint SWP_SHOWWINDOW = 64;
        public const uint SWP_HIDEWINDOW = 128;
        public const uint SWP_NOCOPYBITS = 256;
        public const uint SWP_NOOWNERZORDER = 512;
        public const uint SWP_NOSENDCHANGING = 1024;
        public const int GWL_WNDPROC = -4;
        public const int GWL_HINSTANCE = -6;
        public const int GWL_HWNDPARENT = -8;
        public const int GWL_STYLE = -16;
        public const int GWL_EXSTYLE = -20;
        public const int GWL_USERDATA = -21;
        public const int GWL_ID = -12;
        public const uint PM_NOREMOVE = 0;
        public const uint PM_REMOVE = 1;
        public const uint GA_PARENT = 1;
        public const uint GA_ROOT = 2;
        public const uint GA_ROOTOWNER = 3;
        public const uint WM_CLOSE = 16;
        public const uint WM_MOVE = 3;
        public const uint WM_MOUSEMOVE = 512;
        public const uint WM_QUIT = 18;
        public const uint WM_COMMAND = 273;
        public const uint WM_ACTIVATE = 6;
        public const uint WM_ACTIVATEAPP = 28;
        public const uint WM_LBUTTONDOWN = 513;
        public const uint WM_LBUTTONUP = 514;
        public const uint WM_USER = 1024;
        public const uint WM_APP = 32768;
        public const uint WM_MOUSEACTIVATE = 33;
        public const uint WM_PAINT = 15;
        public const uint WM_DISPLAYCHANGE = 126;
        public const uint WM_DEVICECHANGE = 537;
        public const uint WM_MOUSEWHEEL = 522;
        public const uint WM_KEYDOWN = 256;
        public const uint WM_KEYUP = 257;
        public const uint WM_SYSKEYDOWN = 260;
        public const uint WM_ERASEBKGND = 20;
        public const uint WM_NCPAINT = 133;
        public const uint WM_GETICON = 127;
        public const uint WM_SHOWWINDOW = 24;
        public const uint WM_SETFOCUS = 7;
        public const uint WM_KILLFOCUS = 8;
        public const uint WM_IME_KEYDOWN = 656;
        public const uint WM_IME_KEYUP = 657;
        public const uint WM_IME_CHAR = 646;
        public const uint MA_NOACTIVATEANDEAT = 4;
        public const int ICON_SMALL = 0;
        public const int ICON_BIG = 1;
        public const int GCL_HICON = -14;
        public const int GCL_HICONSM = -34;
        public const uint SMTO_ABORTIFHUNG = 2;
        public const uint VK_F5 = 116;
        public const uint VK_LEFT = 37;
        public const uint VK_RIGHT = 39;
        public const uint VK_LCONTROL = 162;
        public const uint VK_RCONTROL = 163;
        public const uint VK_LMENU = 164;
        public const uint VK_RMENU = 165;
        public const uint VK_LWIN = 91;
        public const uint VK_RWIN = 92;
        public const uint VK_LSHIFT = 160;
        public const uint VK_RSHIFT = 161;
        public const uint VK_BACK = 8;
        public const uint VK_BROWSER_BACK = 166;
        public const uint VK_BROWSER_FORWARD = 167;
        public const uint VK_DOWN = 40;
        public const uint VK_UP = 38;
        public const uint VK_NEXT = 34;
        public const uint VK_PRIOR = 33;
        public const uint GW_HWNDFIRST = 0;
        public const uint GW_HWNDLAST = 1;
        public const uint GW_HWNDNEXT = 2;
        public const uint GW_HWNDPREV = 3;
        public const uint GW_OWNER = 4;
        public const uint GW_CHILD = 5;
        public static uint WM_SHELLHOOKMESSAGE;
        public const uint HSHELL_WINDOWCREATED = 1;
        public const uint HSHELL_WINDOWDESTROYED = 2;
        public const uint HSHELL_ACTIVATESHELLWINDOW = 3;
        public const uint HSHELL_WINDOWACTIVATED = 4;
        public const uint HSHELL_GETMINRECT = 5;
        public const uint HSHELL_REDRAW = 6;
        public const uint HSHELL_TASKMAN = 7;
        public const uint HSHELL_LANGUAGE = 8;
        public const uint HSHELL_SYSMENU = 9;
        public const uint HSHELL_ENDTASK = 10;
        public const uint HSHELL_ACCESSIBILITYSTATE = 11;
        public const uint HSHELL_APPCOMMAND = 12;
        public const uint HSHELL_WINDOWREPLACED = 13;
        public const uint HSHELL_WINDOWREPLACING = 14;
        public const uint HSHELL_HIGHBIT = 32768;
        public const uint HSHELL_FLASH = 32774;
        public const uint HSHELL_RUDEAPPACTIVATED = 32772;
        public const int HCBT_CREATEWND = 3;
        public const int HCBT_DESTROYWND = 4;
        public const int HCBT_ACTIVATE = 5;
        public const int ERROR_SUCCESS = 0;
        public const int ERROR_NO_MORE_ITEMS = 259;
        public const uint SYSTEM_DEFAULT = 0;
        public const uint SEM_FAILCRITICALERRORS = 1;
        public const uint SEM_NOALIGNMENTFAULTEXCEPT = 4;
        public const uint SEM_NOGPFAULTERRORBOX = 2;
        public const uint SEM_NOOPENFILEERRORBOX = 32768;
        public const int DBT_DEVTYP_DEVICEINTERFACE = 5;
        public const int DEVICE_NOTIFY_WINDOW_HANDLE = 0;
        public const int DEVICE_NOTIFY_ALL_INTERFACE_CLASSES = 4;
        public const int DBT_DEVICEARRIVAL = 32768;
        public const int DBT_DEVICEREMOVECOMPLETE = 32772;
        public const int DIGCF_PRESENT = 2;
        public const int DIGCF_DEVICEINTERFACE = 16;
        public const int HIDP_STATUS_SUCCESS = 1114112;
        public const int HIDP_STATUS_NULL = -2146369535;
        public const int HIDP_STATUS_INVALID_PREPARSED_DATA = -1072627711;
        public const int HIDP_STATUS_INVALID_REPORT_TYPE = -1072627710;
        public const int HIDP_STATUS_INVALID_REPORT_LENGTH = -1072627709;
        public const int HIDP_STATUS_USAGE_NOT_FOUND = -1072627708;
        public const int HIDP_STATUS_VALUE_OUT_OF_RANGE = -1072627707;
        public const int HIDP_STATUS_BAD_LOG_PHY_VALUES = -1072627706;
        public const int HIDP_STATUS_BUFFER_TOO_SMALL = -1072627705;
        public const int HIDP_STATUS_INTERNAL_ERROR = -1072627704;
        public const int HIDP_STATUS_I8042_TRANS_UNKNOWN = -1072627703;
        public const int HIDP_STATUS_INCOMPATIBLE_REPORT_ID = -1072627702;
        public const int HIDP_STATUS_NOT_VALUE_ARRAY = -1072627701;
        public const int HIDP_STATUS_IS_VALUE_ARRAY = -1072627700;
        public const int HIDP_STATUS_DATA_INDEX_NOT_FOUND = -1072627699;
        public const int HIDP_STATUS_DATA_INDEX_OUT_OF_RANGE = -1072627698;
        public const int HIDP_STATUS_BUTTON_NOT_PRESSED = -1072627697;
        public const int HIDP_STATUS_REPORT_DOES_NOT_EXIST = -1072627696;
        public const int HIDP_STATUS_NOT_IMPLEMENTED = -1072627680;
        public const uint GENERIC_READ = 2147483648;
        public const uint GENERIC_WRITE = 1073741824;
        public const uint FILE_SHARE_WRITE = 2;
        public const uint FILE_SHARE_READ = 1;
        public const uint FILE_FLAG_OVERLAPPED = 1073741824;
        public const uint OPEN_EXISTING = 3;
        public const uint OPEN_ALWAYS = 4;
        public const int ERROR_INSUFFICIENT_BUFFER = 122;
        public const uint CR_SUCCESS = 0;
        public const int STANDARD_RIGHTS_REQUIRED = 983040;
        public const int PRINTER_ACCESS_ADMINISTER = 4;
        public const int PRINTER_ACCESS_USE = 8;
        public const int PRINTER_ALL_ACCESS = 983052;
        public const int PRINTER_STATUS_ERROR = 2;
        public const int PRINTER_STATUS_PAPER_JAM = 8;
        public const int PRINTER_STATUS_DOOR_OPEN = 4194304;
        public const int PRINTER_STATUS_PAPER_OUT = 16;
        public const int PRINTER_STATUS_MANUAL_FEED = 32;
        public const int PRINTER_STATUS_PAPER_PROBLEM = 64;
        public const int PRINTER_STATUS_OFFLINE = 128;
        public const int PRINTER_STATUS_BUSY = 512;
        public const int PRINTER_STATUS_PRINTING = 1024;
        public const int PRINTER_STATUS_OUTPUT_BIN_FULL = 2048;
        public const int PRINTER_STATUS_NOT_AVAILABLE = 4096;
        public const int PRINTER_STATUS_WAITING = 8192;
        public const int PRINTER_STATUS_PROCESSING = 16384;
        public const int PRINTER_STATUS_INITIALIZING = 32768;
        public const int PRINTER_STATUS_WARMING_UP = 65536;
        public const int PRINTER_STATUS_TONER_LOW = 131072;
        public const int PRINTER_STATUS_NO_TONER = 262144;
        public const int PRINTER_STATUS_PAGE_PUNT = 524288;
        public const int PRINTER_STATUS_USER_INTERVENTION = 1048576;
        public const int PRINTER_STATUS_OUT_OF_MEMORY = 2097152;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetCaretPos(ref OS_Win.POINT point);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetCursorInfo(ref OS_Win.CursorInfo pci);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int ShowCursor(bool bShow);

        public static bool CursorVisible
        {
            get
            {
                OS_Win.CursorInfo pci = new OS_Win.CursorInfo();
                pci.cbSize = Marshal.SizeOf(typeof(OS_Win.CursorInfo));
                if (!OS_Win.GetCursorInfo(ref pci))
                    throw new Win32Exception();
                return pci.flags == OS_Win.CursorFlags.Showing;
            }
            set
            {
                int num1 = OS_Win.ShowCursor(value);
                int num2 = value ? 0 : -1;
                while (num1 != num2)
                    num1 = OS_Win.ShowCursor(num1 < num2);
            }
        }

        private static void _ExitWindows(OS_Win.ExitWindows exAction, bool forceIfHung = false)
        {
            OS_Win.AcquireTokenPriv("SeShutdownPrivilege", 40U);
            OS_Win.ExitWindows uFlags = exAction;
            if (forceIfHung)
                uFlags |= OS_Win.ExitWindows.ForceIfHung;
            if (!OS_Win.ExitWindowsEx(uFlags, OS_Win.ShutdownReason.MajorApplication | OS_Win.ShutdownReason.MinorInstallation | OS_Win.ShutdownReason.FlagPlanned))
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to exit Windows");
        }

        public static void RebootWindows(bool forceIfHung = false)
        {
            OS_Win._ExitWindows(OS_Win.ExitWindows.Reboot, forceIfHung);
        }

        public static void ShutDownWindows(bool forceIfHung = false)
        {
            OS_Win._ExitWindows(OS_Win.ExitWindows.ShutDown, forceIfHung);
        }

        public static string GetOSArchitecture()
        {
            bool is64 = System.Environment.Is64BitOperatingSystem;
            if (is64)
                return "x64";
            else
                return "x86";
        }
        public static void LogOffWindows(bool forceIfHung = false)
        {
            OS_Win._ExitWindows(OS_Win.ExitWindows.LogOff, forceIfHung);
        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ExitWindowsEx(OS_Win.ExitWindows uFlags, OS_Win.ShutdownReason dwReason);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool OpenProcessToken(IntPtr ProcessHandle, uint DesiredAccess, out IntPtr TokenHandle);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool LookupPrivilegeValue(string lpSystemName, string lpName, out OS_Win.LUID lpLuid);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AdjustTokenPrivileges(IntPtr TokenHandle, [MarshalAs(UnmanagedType.Bool)] bool DisableAllPrivileges, ref OS_Win.TOKEN_PRIVILEGES NewState, uint Zero, IntPtr Null1, IntPtr Null2);

        public static void AcquireTokenPriv(string name, uint accessFlags = 40)
        {
            IntPtr TokenHandle = IntPtr.Zero;
            try
            {
                if (!OS_Win.OpenProcessToken(Process.GetCurrentProcess().Handle, accessFlags, out TokenHandle))
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to open process token handle");
                OS_Win.TOKEN_PRIVILEGES NewState = new OS_Win.TOKEN_PRIVILEGES();
                NewState.PrivilegeCount = 1U;
                NewState.Privileges = new OS_Win.LUID_AND_ATTRIBUTES[1];
                NewState.Privileges[0].Attributes = 2U;
                if (!OS_Win.LookupPrivilegeValue((string)null, name, out NewState.Privileges[0].Luid))
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to look up privilege");
                if (!OS_Win.AdjustTokenPrivileges(TokenHandle, false, ref NewState, 0U, IntPtr.Zero, IntPtr.Zero))
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to adjust process token privileges");
            }
            finally
            {
                if (TokenHandle != IntPtr.Zero)
                    OS_Win.CloseHandle(TokenHandle);
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool AttachConsole(int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool FreeConsole();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern OS_Win.ExecutionState SetThreadExecutionState(OS_Win.ExecutionState esFlags);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool OpenProcessToken(IntPtr ProcessHandle, TokenAccessLevels DesiredAccess, out IntPtr TokenHandle);

        public static IntPtr OpenProcessToken(Process process, TokenAccessLevels DesiredAccess = TokenAccessLevels.AssignPrimary | TokenAccessLevels.Duplicate | TokenAccessLevels.Query)
        {
            IntPtr TokenHandle;
            if (!OS_Win.OpenProcessToken(process.Handle, DesiredAccess, out TokenHandle))
                throw new Win32Exception();
            return TokenHandle;
        }

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetVolumeInformation(string RootPathName, StringBuilder VolumeNameBuffer, int VolumeNameSize, out uint VolumeSerialNumber, out uint MaximumComponentLength, out OS_Win.FileSystemFeature FileSystemFlags, StringBuilder FileSystemNameBuffer, int nFileSystemNameSize);

        public static int GetVolumeSerialNumber(char driveLetter)
        {
            uint VolumeSerialNumber;
            uint MaximumComponentLength;
            OS_Win.FileSystemFeature FileSystemFlags;
            if (!OS_Win.GetVolumeInformation(driveLetter.ToString() + ":\\", (StringBuilder)null, 0, out VolumeSerialNumber, out MaximumComponentLength, out FileSystemFlags, (StringBuilder)null, 0))
                throw new Win32Exception();
            return (int)VolumeSerialNumber;
        }

        [DllImport("kernel32.dll", EntryPoint = "GetSystemTimes", SetLastError = true)]
        private static extern bool _GetSystemTimes(out System.Runtime.InteropServices.ComTypes.FILETIME lpIdleTime, out System.Runtime.InteropServices.ComTypes.FILETIME lpKernelTime, out System.Runtime.InteropServices.ComTypes.FILETIME lpUserTime);

        public static void GetSystemTimes(out TimeSpan IdleTime, out TimeSpan KernelTime, out TimeSpan UserTime)
        {
            System.Runtime.InteropServices.ComTypes.FILETIME lpIdleTime;
            System.Runtime.InteropServices.ComTypes.FILETIME lpKernelTime;
            System.Runtime.InteropServices.ComTypes.FILETIME lpUserTime;
            if (!OS_Win._GetSystemTimes(out lpIdleTime, out lpKernelTime, out lpUserTime))
                throw new Win32Exception();
            IdleTime = OS_Win.FiletimeToTimeSpan(lpIdleTime);
            KernelTime = OS_Win.FiletimeToTimeSpan(lpKernelTime);
            UserTime = OS_Win.FiletimeToTimeSpan(lpUserTime);
        }

        internal static TimeSpan FiletimeToTimeSpan(System.Runtime.InteropServices.ComTypes.FILETIME fileTime)
        {
            return TimeSpan.FromMilliseconds((double)((ulong)fileTime.dwHighDateTime << 32 | (ulong)(uint)fileTime.dwLowDateTime) / 10000.0);
        }

        [DllImport("kernel32.dll", EntryPoint = "GetSystemRegistryQuota", SetLastError = true)]
        private static extern bool _GetSystemRegistryQuota(out uint pdwQuotaAllowed, out uint pdwQuotaUsed);

        public static void GetSystemRegistryQuota(out long QuotaAllowed, out long QuotaUsed)
        {
            uint pdwQuotaAllowed;
            uint pdwQuotaUsed;
            if (!OS_Win._GetSystemRegistryQuota(out pdwQuotaAllowed, out pdwQuotaUsed))
                throw new Win32Exception();
            QuotaAllowed = (long)pdwQuotaAllowed;
            QuotaUsed = (long)pdwQuotaUsed;
        }

        [DllImport("User32.dll", EntryPoint = "GetGuiResources", SetLastError = true)]
        private static extern int _GetGuiResources(IntPtr hProcess, uint uiFlags);

        public static int GetGDIObjectCount(this Process hProcess)
        {
            int guiResources = OS_Win._GetGuiResources(hProcess.Handle, 0U);
            if (guiResources != 0)
                return guiResources;
            int lastWin32Error = Marshal.GetLastWin32Error();
            if (lastWin32Error == 0)
                return guiResources;
            throw new Win32Exception(lastWin32Error);
        }

        public static int GetUserObjectCount(this Process hProcess)
        {
            int guiResources = OS_Win._GetGuiResources(hProcess.Handle, 1U);
            if (guiResources != 0)
                return guiResources;
            int lastWin32Error = Marshal.GetLastWin32Error();
            if (lastWin32Error == 0)
                return guiResources;
            throw new Win32Exception(lastWin32Error);
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GlobalMemoryStatusEx([In, Out] OS_Win.MEMORYSTATUSEX lpBuffer);

        public static OS_Win.MEMORYSTATUSEX GlobalMemoryStatus()
        {
            OS_Win.MEMORYSTATUSEX lpBuffer = new OS_Win.MEMORYSTATUSEX();
            if (OS_Win.GlobalMemoryStatusEx(lpBuffer))
                return lpBuffer;
            throw new Win32Exception();
        }

        internal static OS_Win.IO_COUNTERS GetProcessIoCounters(this Process hProcess)
        {
            OS_Win.IO_COUNTERS lpIoCounters = new OS_Win.IO_COUNTERS();
            if (!OS_Win.GetProcessIoCounters(hProcess.Handle, out lpIoCounters))
                throw new Win32Exception();
            return lpIoCounters;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetProcessIoCounters(IntPtr hProcess, out OS_Win.IO_COUNTERS lpIoCounters);

        [DllImport("wininet.dll", SetLastError = true)]
        private static extern bool InternetGetConnectedState(out OS_Win.InternetGetConnectedStateFlags lpdwFlags, int dwReserved);

        public static string GetHeartbeatInternetConnValue()
        {
            OS_Win.InternetGetConnectedStateFlags lpdwFlags;
            if (!OS_Win.InternetGetConnectedState(out lpdwFlags, 0))
                return "Internet is not connected (or no default gateway).";
            if ((lpdwFlags & OS_Win.InternetGetConnectedStateFlags.INTERNET_CONNECTION_OFFLINE) != (OS_Win.InternetGetConnectedStateFlags)0)
                return "Internet connection is offline.";
            if ((lpdwFlags & OS_Win.InternetGetConnectedStateFlags.INTERNET_CONNECTION_LAN) != (OS_Win.InternetGetConnectedStateFlags)0)
                return "Internet connection is via LAN/WAN.";
            return (lpdwFlags & OS_Win.InternetGetConnectedStateFlags.INTERNET_CONNECTION_MODEM) != (OS_Win.InternetGetConnectedStateFlags)0 ? "Internet connection is via modem." : "Internet connection is unknown.";
        }

        [DllImport("kernel32.dll", EntryPoint = "TerminateProcess", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool _TerminateProcess(IntPtr hProcess, int uExitCode);

        public static void TerminateProcess(this Process proc, int ExitCode)
        {
            if (!OS_Win._TerminateProcess(proc.Handle, ExitCode))
                throw new Win32Exception();
        }

        public static void TerminateProcessTree(this Process proc)
        {
            proc.TerminateProcessTree(-1);
        }

        public static void TerminateProcessTree(this Process proc, int ExitCode)
        {
            List<Process> retList = new List<Process>();
            OS_Win.GetProcAndChildren(Process.GetProcesses(), proc, retList);
            foreach (Process proc1 in retList)
            {
                try
                {
                    proc1.TerminateProcess(ExitCode);
                }
                catch (Exception ex)
                {
                    if (proc1.Id == proc.Id)
                        throw;
                }
            }
        }

        [DllImport("ntdll.dll")]
        private static extern int NtQueryInformationProcess(IntPtr hProc, int procInfoClass, ref OS_Win.PROCESS_BASIC_INFORMATION procInfo, int procInfoSize, out int retSize);

        public static void GetProcAndChildren(Process[] runningProcs, Process parProc, List<Process> retList)
        {
            foreach (Process runningProc in runningProcs)
            {
                if (OS_Win.GetParentProcId(runningProc) == parProc.Id)
                    OS_Win.GetProcAndChildren(runningProcs, runningProc, retList);
            }
            retList.Add(parProc);
        }

        public static int GetParentProcId(Process p)
        {
            try
            {
                OS_Win.PROCESS_BASIC_INFORMATION procInfo = new OS_Win.PROCESS_BASIC_INFORMATION();
                int retSize;
                if (OS_Win.NtQueryInformationProcess(p.Handle, 0, ref procInfo, Marshal.SizeOf((object)procInfo), out retSize) != 0)
                    return 0;
                return procInfo.InheritedFromUniqueProcessId.ToInt32();
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        [DllImport("wtsapi32.dll", SetLastError = true)]
        private static extern bool WTSQueryUserToken(int sessionId, out IntPtr Token);

        public static IntPtr WTSQueryUserToken(int sessionid)
        {
            IntPtr Token;
            if (!OS_Win.WTSQueryUserToken(sessionid, out Token))
                throw new Win32Exception();
            return Token;
        }

        [DllImport("Wtsapi32.dll", CharSet = CharSet.Unicode)]
        private static extern bool WTSQuerySessionInformation(IntPtr hServer, int sessionId, OS_Win.WTS_INFO_CLASS wtsInfoClass, out IntPtr ppBuffer, out int pBytesReturned);

        /// <summary>
        /// The WTSFreeMemory function frees memory allocated by a Terminal
        /// Services function.
        /// </summary>
        /// <param name="memory">Pointer to the memory to free.</param>
        [DllImport("wtsapi32.dll")]
        private static extern void WTSFreeMemory(IntPtr memory);

        [DllImport("Kernel32.dll")]
        private static extern int WTSGetActiveConsoleSessionId();

        public static bool GetIsCurrentSessionConsoleSession()
        {
            return OS_Win.WTSGetActiveConsoleSessionId() == OS_Win.GetCurrentSessionId();
        }

        public static int GetCurrentSessionId()
        {
            IntPtr ppBuffer = IntPtr.Zero;
            int pBytesReturned;
            int num;
            if (OS_Win.WTSQuerySessionInformation(IntPtr.Zero, -1, OS_Win.WTS_INFO_CLASS.WTSSessionId, out ppBuffer, out pBytesReturned))
            {
                num = Marshal.ReadInt32(ppBuffer);
                OS_Win.WTSFreeMemory(ppBuffer);
            }
            else
                num = Process.GetCurrentProcess().SessionId;
            return num;
        }

        public static string GetSessionClientName(int SessionId = -1)
        {
            IntPtr ppBuffer = IntPtr.Zero;
            string str = (string)null;
            int pBytesReturned;
            if (OS_Win.WTSQuerySessionInformation(OS_Win.WTS_CURRENT_SERVER_HANDLE, SessionId, OS_Win.WTS_INFO_CLASS.WTSClientName, out ppBuffer, out pBytesReturned))
            {
                str = Marshal.PtrToStringUni(ppBuffer, pBytesReturned / 2).TrimEnd(new char[1]);
                OS_Win.WTSFreeMemory(ppBuffer);
            }
            return str;
        }

        [DllImport("kernel32.dll", EntryPoint = "CloseHandle", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool _CloseHandle(IntPtr hObject);

        public static void CloseHandle(IntPtr hObject)
        {
            if (!OS_Win._CloseHandle(hObject))
                throw new Win32Exception();
        }

        [DllImport("userenv.dll", SetLastError = true)]
        private static extern bool CreateEnvironmentBlock(out IntPtr lpEnvironment, IntPtr hToken, bool bInherit);

        public static IntPtr CreateEnvironmentBlock(IntPtr hToken, bool bInherit)
        {
            IntPtr lpEnvironment;
            if (!OS_Win.CreateEnvironmentBlock(out lpEnvironment, hToken, bInherit))
                throw new Win32Exception();
            return lpEnvironment;
        }

        [DllImport("userenv.dll", EntryPoint = "DestroyEnvironmentBlock", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool _DestroyEnvironmentBlock(IntPtr lpEnvironment);

        public static void DestroyEnvironmentBlock(IntPtr lpEnvironment)
        {
            if (!OS_Win._DestroyEnvironmentBlock(lpEnvironment))
                throw new Win32Exception();
        }

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool CreateProcessAsUser(IntPtr hToken, string lpApplicationName, string lpCommandLine, IntPtr lpProcessAttributes, IntPtr lpThreadAttributes, bool bInheritHandles, uint dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, ref OS_Win.STARTUPINFO lpStartupInfo, out OS_Win.PROCESS_INFORMATION lpProcessInformation);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool CreateProcess(string lpApplicationName, string lpCommandLine, IntPtr lpProcessAttributes, IntPtr lpThreadAttributes, bool bInheritHandles, uint dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, ref OS_Win.STARTUPINFO lpStartupInfo, out OS_Win.PROCESS_INFORMATION lpProcessInformation);

        [DllImport("user32.dll")]
        public static extern IntPtr CreateWindowEx(uint exStyle, string className, string title, uint style, int x, int y, int w, int h, IntPtr hPar, IntPtr hMenu, IntPtr hInst, IntPtr pParam);

        [DllImport("user32.dll")]
        public static extern bool DestroyWindow(IntPtr h);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PostMessage(IntPtr h, uint wm, IntPtr wp, IntPtr lp);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr h, uint wm, IntPtr wp, IntPtr lp);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SendMessageTimeout(IntPtr h, uint wm, IntPtr wp, IntPtr lp, uint flags, int timeoutMs, out IntPtr retVal);

        [DllImport("user32.dll")]
        public static extern bool WaitMessage();

        [DllImport("user32.dll")]
        public static extern int MsgWaitForMultipleObjects(int cnt, IntPtr[] hndls, bool waitAll, int ms, uint wakeMask);

        [DllImport("winmm.dll")]
        public static extern int timeBeginPeriod(int ms);

        [DllImport("winmm.dll")]
        public static extern int timeEndPeriod(int ms);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PostThreadMessage(int tid, uint wm, IntPtr wp, IntPtr lp);

        [DllImport("user32.dll")]
        public static extern bool GetMessage(out OS_Win.MSG pMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

        [DllImport("user32.dll")]
        public static extern bool PeekMessage(out OS_Win.MSG pMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax, uint wRemove);

        [DllImport("user32.dll")]
        public static extern bool TranslateMessage(ref OS_Win.MSG pMsg);

        [DllImport("user32.dll")]
        public static extern IntPtr DispatchMessage(ref OS_Win.MSG pMsg);

        [DllImport("kernel32")]
        public static extern long GetTickCount64();

        [DllImport("user32.dll")]
        private static extern bool GetLastInputInfo(ref OS_Win.LASTINPUTINFO v);

        public static uint GetLastInputTimeVal()
        {
            OS_Win.LASTINPUTINFO v = new OS_Win.LASTINPUTINFO();
            v.cbSize = (uint)Marshal.SizeOf(typeof(OS_Win.LASTINPUTINFO));
            if (!OS_Win.GetLastInputInfo(ref v))
                throw new Exception("GetLastInputInfo failed");
            return v.dwTime;
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetParent(IntPtr h, IntPtr newPar);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowRect(IntPtr hWnd, ref OS_Win.RECT r);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hInsertAfter, int x, int y, int w, int h, uint swpFlags);

        [DllImport("user32.dll", EntryPoint = "GetWindowText", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int GetWindowTextAPI(IntPtr h, StringBuilder buf, int bufLen);

        [DllImport("user32.dll", EntryPoint = "GetClassName")]
        private static extern int GetClassNameAPI(IntPtr h, StringBuilder buf, int bufLen);

        [DllImport("user32.dll", EntryPoint = "RealGetClassName")]
        private static extern int RealGetClassNameAPI(IntPtr h, StringBuilder buf, int bufLen);

        public static string GetWindowText(IntPtr h, int bufLen)
        {
            OS_Win.SetLastError(0);
            StringBuilder buf = new StringBuilder(bufLen);
            int windowTextApi = OS_Win.GetWindowTextAPI(h, buf, bufLen);
            if (windowTextApi == 0 && Marshal.GetLastWin32Error() != 0)
                return (string)null;
            return buf.ToString(0, windowTextApi > bufLen ? bufLen : windowTextApi);
        }

        public static string GetClassName(IntPtr h, int bufLen)
        {
            OS_Win.SetLastError(0);
            StringBuilder buf = new StringBuilder(bufLen);
            int classNameApi = OS_Win.GetClassNameAPI(h, buf, bufLen);
            return buf.ToString(0, classNameApi > bufLen ? bufLen : classNameApi);
        }

        public static string RealGetClassName(IntPtr h, int bufLen)
        {
            OS_Win.SetLastError(0);
            StringBuilder buf = new StringBuilder(bufLen);
            int classNameApi = OS_Win.RealGetClassNameAPI(h, buf, bufLen);
            return buf.ToString(0, classNameApi > bufLen ? bufLen : classNameApi);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string modName = null);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool ShowWindow(IntPtr h, int sw);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindow(IntPtr h, uint op);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetTopWindow(IntPtr h);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetParent(IntPtr h);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetAncestor(IntPtr h, uint ga_flags);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumWindows(OS_Win.EnumWindowsDelegate proc, IntPtr lp);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindowLong(IntPtr h, int gwl);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetWindowLong(IntPtr h, int gwl, IntPtr v);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr CallWindowProc(IntPtr hPrev, IntPtr h, uint m, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool IsWindow(IntPtr h);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool IsWindowVisible(IntPtr h);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool IsIconic(IntPtr h);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetFocus(IntPtr h);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetForegroundWindow(IntPtr h);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool LockSetForegroundWindow(uint v);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern uint MapVirtualKey(uint uCode, uint uMapType);
        public static int GetLastError()
        {
            return Marshal.GetLastWin32Error();
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetProp(IntPtr hWnd, string name, IntPtr hData);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetProp(IntPtr hWnd, string name);

        [DllImport("kernel32.dll")]
        public static extern void SetLastError(int v);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDlgItem(IntPtr h, int childId);

        [DllImport("user32.dll")]
        public static extern int SetDlgItemText(IntPtr hwnd, int id, string title);

        [DllImport("user32.dll")]
        public static extern int GetDlgItemText(IntPtr hwnd, int id, StringBuilder title, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern bool RegisterShellHookWindow(IntPtr h);

        [DllImport("user32.dll")]
        public static extern bool DeregisterShellHookWindow(IntPtr h);

        [DllImport("user32.dll")]
        public static extern bool SetTaskmanWindow(IntPtr h);

        [DllImport("user32.dll")]
        public static extern bool SetShellWindow(IntPtr h);

        [DllImport("user32.dll")]
        public static extern uint RegisterWindowMessage(string m);

        [DllImport("user32.dll")]
        public static extern int GetWindowThreadProcessId(IntPtr h, out uint procId);

        [DllImport("user32.dll")]
        public static extern bool SystemParametersInfo(uint action, uint param, IntPtr pData, uint fWinIni);

        [DllImport("user32.dll")]
        public static extern bool SystemParametersInfo(uint uAction, uint uParam, ref bool lpvParam, uint flags);

        [DllImport("user32.dll")]
        public static extern bool SystemParametersInfo(uint uAction, uint uParam, ref OS_Win.MINIMIZEDMETRICS lpvParam, uint flags);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string className, string title);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr par, IntPtr childAfter, string className, string title);

        [DllImport("user32.dll")]
        public static extern short GetKeyState(uint vk);

        [DllImport("user32.dll")]
        public static extern ushort VkKeyScan(char c);

        [DllImport("user32.dll")]
        public static extern uint SendInput(int len, OS_Win.INPUT[] inputs, int size);

        [DllImport("user32.dll")]
        public static extern bool InvalidateRect(IntPtr h, ref OS_Win.RECT rect, bool erase);

        [DllImport("user32.dll")]
        public static extern bool UpdateWindow(IntPtr h);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr h);

        [DllImport("user32.dll")]
        public static extern bool ReleaseDC(IntPtr h, IntPtr hDC);

        [DllImport("user32.dll")]
        public static extern bool GetUpdateRect(IntPtr h, ref OS_Win.RECT pRect, bool sendErase);

        [DllImport("user32.dll")]
        public static extern bool ValidateRect(IntPtr h, ref OS_Win.RECT pRect);

        [DllImport("user32.dll")]
        public static extern bool ValidateRect(IntPtr h, IntPtr pRect);

        public static IntPtr GetClassLongPtr(IntPtr hWnd, int nInd)
        {
            if (IntPtr.Size > 4)
                return OS_Win.GetClassLongPtr64(hWnd, nInd);
            return (IntPtr)((long)OS_Win.GetClassLongPtr32(hWnd, nInd));
        }

        [DllImport("user32.dll", EntryPoint = "GetClassLong")]
        public static extern uint GetClassLongPtr32(IntPtr hWnd, int nInd);

        [DllImport("user32.dll", EntryPoint = "GetClassLongPtr")]
        public static extern IntPtr GetClassLongPtr64(IntPtr hWnd, int nInd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetFocus();

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool ConvertStringSidToSid([MarshalAs(UnmanagedType.LPTStr), In] string pStringSid, ref IntPtr sid);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool LookupAccountSid([MarshalAs(UnmanagedType.LPTStr), In] string systemName, IntPtr sid, [MarshalAs(UnmanagedType.LPTStr), Out] StringBuilder name, ref int cbName, StringBuilder referencedDomainName, ref int cbReferencedDomainName, out OS_Win.SID_NAME_USE use);

        public static string GetAccountNameForSID(string sid)
        {
            IntPtr zero = IntPtr.Zero;
            int cbName = 0;
            int cbReferencedDomainName = 0;
            StringBuilder referencedDomainName1 = new StringBuilder();
            StringBuilder name1 = new StringBuilder();
            if (!OS_Win.ConvertStringSidToSid(sid, ref zero))
            {
                int lastWin32Error = Marshal.GetLastWin32Error();
                Marshal.FreeHGlobal(zero);
                throw new Win32Exception(lastWin32Error);
            }
            OS_Win.SID_NAME_USE use;
            OS_Win.LookupAccountSid((string)null, zero, name1, ref cbName, referencedDomainName1, ref cbReferencedDomainName, out use);
            StringBuilder referencedDomainName2 = new StringBuilder(cbReferencedDomainName);
            StringBuilder name2 = new StringBuilder(cbName);
            if (!OS_Win.LookupAccountSid((string)null, zero, name2, ref cbName, referencedDomainName2, ref cbReferencedDomainName, out use))
            {
                int lastWin32Error = Marshal.GetLastWin32Error();
                Marshal.FreeHGlobal(zero);
                if (lastWin32Error == 1332)
                    return (string)null;
                throw new Win32Exception(lastWin32Error);
            }
            Marshal.FreeHGlobal(zero);
            return referencedDomainName2.ToString() + "\\" + name2.ToString();
        }

        internal static List<string> GetWindowsUserList()
        {
            List<string> user_list = new List<string>();
            try
            {
                RegistryKey registryKey;
                using (registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\ProfileList"))
                {
                    if (registryKey == null)
                        return user_list;
                    string[] subKeyNames = registryKey.GetSubKeyNames();
                    for (int index = 0; index < subKeyNames.Length; ++index)
                    {
                        try
                        {
                            if (subKeyNames[index] != null)
                            {
                                if (subKeyNames[index].StartsWith("S-1-5-21-"))
                                {
                                    string accountNameForSid = OS_Win.GetAccountNameForSID(subKeyNames[index]);
                                    if (accountNameForSid != null)
                                        user_list.Add(accountNameForSid);
                                }
                            }
                        }
                        catch
                        {

                        }
                    }
                }
            }
            catch
            {
            }
            return user_list;

        }

        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags = 2);

        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromPoint(OS_Win.POINT pt, uint dwFlags = 2);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetMonitorInfo(IntPtr hMonitor, ref OS_Win.MONITORINFO lpmi);

        internal enum KeyMod : uint
        {
            None = 0,
            CtrlL = 1,
            CtrlR = 2,
            AltL = 4,
            AltR = 8,
            WinL = 16, // 0x00000010
            WinR = 32, // 0x00000020
            ShiftL = 64, // 0x00000040
            ShiftR = 128, // 0x00000080
            CtrlBoth = 256, // 0x00000100
            AltBoth = 512, // 0x00000200
            ShiftBoth = 1024, // 0x00000400
        }
        public static KeyMod GetKeyMod()
        {
            KeyMod keyMod = KeyMod.None;
            if ((int)OS_Win.GetKeyState(162U) < 0)
                keyMod |= KeyMod.CtrlL;
            if ((int)OS_Win.GetKeyState(163U) < 0)
                keyMod |= KeyMod.CtrlR;
            if ((int)OS_Win.GetKeyState(164U) < 0)
                keyMod |= KeyMod.AltL;
            if ((int)OS_Win.GetKeyState(165U) < 0)
                keyMod |= KeyMod.AltR;
            if ((int)OS_Win.GetKeyState(91U) < 0)
                keyMod |= KeyMod.WinL;
            if ((int)OS_Win.GetKeyState(92U) < 0)
                keyMod |= KeyMod.WinR;
            if ((int)OS_Win.GetKeyState(160U) < 0)
                keyMod |= KeyMod.ShiftL;
            if ((int)OS_Win.GetKeyState(161U) < 0)
                keyMod |= KeyMod.ShiftR;
            return keyMod;
        }

        [DllImport("kernel32.dll")]
        public static extern uint SetErrorMode(uint mode);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr RegisterDeviceNotification(IntPtr hWnd, ref OS_Win.DEV_BROADCAST_HDR filter, uint flags);

        [DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr lpDeviceInfoSet, ref OS_Win.DeviceInterfaceData oInterfaceData, ref OS_Win.DeviceInterfaceDetailData pDeviceInterfaceDetailData, uint nDeviceInterfaceDetailDataSize, IntPtr oRequiredSize, IntPtr pDeviceInfoData);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiEnumDeviceInterfaces(IntPtr pDeviceInfo, IntPtr pDeviceInfoData, ref Guid gClass, uint nIndex, ref OS_Win.DeviceInterfaceData oInterfaceData);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiDestroyDeviceInfoList(IntPtr lpInfoSet);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern IntPtr SetupDiGetClassDevs(ref Guid gClass, [MarshalAs(UnmanagedType.LPStr)] string strEnumerator, IntPtr hParent, uint nFlags);

        [DllImport("hid.dll", SetLastError = true)]
        public static extern void HidD_GetHidGuid(out Guid gHid);

        [DllImport("hid.dll", SetLastError = true)]
        public static extern bool HidD_GetPreparsedData(IntPtr hFile, out IntPtr lpData);

        [DllImport("hid.dll", SetLastError = true)]
        public static extern int HidP_GetCaps(IntPtr lpData, out OS_Win.HidCaps oCaps);

        [DllImport("hid.dll", SetLastError = true)]
        public static extern bool HidD_FreePreparsedData(ref IntPtr pData);

        [DllImport("hid.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool HidD_GetManufacturerString(IntPtr hFile, StringBuilder buffer, int bufferLength);

        [DllImport("hid.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool HidD_GetProductString(IntPtr hFile, StringBuilder buffer, int bufferLength);

        [DllImport("hid.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool HidD_GetSerialNumberString(IntPtr hDevice, StringBuilder buffer, int bufferLength);

        [DllImport("hid.dll", SetLastError = true)]
        public static extern bool HidD_GetAttributes(IntPtr hFile, ref OS_Win.HIDD_ATTRIBUTES attributes);

        [DllImport("hid.dll", SetLastError = true)]
        public static extern bool HidD_SetOutputReport(IntPtr hFile, byte[] lpReportBuffer, int reportBufferLength);

        [DllImport("hid.dll", SetLastError = true)]
        public static extern int HidP_GetUsageValue(OS_Win.HIDP_REPORT_TYPE reportType, ushort page, ushort linkCollection, short usage, out int usageRet, IntPtr preparsedData, byte[] report, int reportLen);

        [DllImport("hid.dll", SetLastError = true)]
        public static extern int HidP_GetUsageValueArray(OS_Win.HIDP_REPORT_TYPE reportType, ushort page, ushort linkCollection, short usage, byte[] usageRet, short usageRetLen, IntPtr preparsedData, byte[] report, int reportLen);

        [DllImport("hid.dll", SetLastError = true)]
        public static extern int HidP_GetButtonCaps(OS_Win.HIDP_REPORT_TYPE reportType, [In, Out] OS_Win.HidP_Button_Caps[] buttonCaps, ref short buttonCapsLength, IntPtr preparsedData);

        [DllImport("hid.dll", SetLastError = true)]
        public static extern int HidP_GetValueCaps(OS_Win.HIDP_REPORT_TYPE reportType, [In, Out] OS_Win.HIDP_VALUE_CAPS[] valueCaps, ref short valueCapsLength, IntPtr preparsedData);

        [DllImport("hid.dll", SetLastError = true)]
        public static extern int HidP_GetSpecificValueCaps(OS_Win.HIDP_REPORT_TYPE reportType, ushort page, ushort linkCollection, ushort usage, [In, Out] OS_Win.HIDP_VALUE_CAPS[] valueCaps, ref int valueCapsLength, IntPtr preparsedData);

        [DllImport("hid.dll", SetLastError = true)]
        public static extern int HidP_InitializeReportForID(OS_Win.HIDP_REPORT_TYPE reportType, byte reportID, IntPtr preparsedData, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] byte[] report, int reportLength);

        [DllImport("hid.dll", SetLastError = true)]
        public static extern int HidP_SetUsages(OS_Win.HIDP_REPORT_TYPE reportType, short usagePage, short linkCollection, [In, Out] OS_Win.HIDP_DATA[] usageList, ref int usageLength, IntPtr preparsedData, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] byte[] report, int reportLength);

        [DllImport("hid.dll", SetLastError = true)]
        public static extern int HidP_SetData(OS_Win.HIDP_REPORT_TYPE reportType, [In, Out] OS_Win.HIDP_DATA[] dataList, ref int dataLength, IntPtr preparsedData, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 5)] byte[] report, int reportLength);

        [DllImport("hid.dll", SetLastError = true)]
        public static extern int HidP_GetData(OS_Win.HIDP_REPORT_TYPE reportType, [In, Out] OS_Win.HIDP_DATA[] dataList, ref int dataLength, IntPtr preparsedData, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 5)] byte[] report, int reportLength);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateFile([MarshalAs(UnmanagedType.LPStr)] string strName, uint nAccess, uint nShareMode, IntPtr lpSecurity, uint nCreationFlags, uint nAttributes, IntPtr lpTemplate);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool DeleteFile(string path);

        [DllImport("cfgmgr32.dll", CharSet = CharSet.Auto)]
        public static extern uint CM_Get_Parent(out uint pdnDevInst, uint dnDevInst, uint ulFlags);

        [DllImport("cfgmgr32.dll", CharSet = CharSet.Auto)]
        public static extern uint CM_Get_Device_ID(uint dnDevInst, string Buffer, uint BufferLen, uint ulFlags);

        [DllImport("cfgmgr32.dll", CharSet = CharSet.Auto)]
        public static extern uint CM_Get_Device_ID_Size(out uint pulLen, uint dnDevInst, uint ulFlags);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetupDiGetClassDevs([MarshalAs(UnmanagedType.LPStruct), In] Guid ClassGuid, string Enumerator, IntPtr hwndParent, uint Flags);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int SetupDiEnumDeviceInfo(IntPtr DeviceInfoSet, uint MemberIndex, ref OS_Win.SP_DEVINFO_DATA DeviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int SetupDiEnumDeviceInterfaces(IntPtr DeviceInfoSet, [In] ref OS_Win.SP_DEVINFO_DATA DeviceInfoData, [MarshalAs(UnmanagedType.LPStruct), In] Guid InterfaceClassGuid, uint MemberIndex, ref OS_Win.SP_DEVICE_INTERFACE_DATA DeviceInterfaceData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int SetupDiGetDeviceInterfaceDetail(IntPtr DeviceInfoSet, [In] ref OS_Win.SP_DEVICE_INTERFACE_DATA DeviceInterfaceData, IntPtr DeviceInterfaceDetailData, uint DeviceInterfaceDetailDataSize, out uint RequiredSize, IntPtr DeviceInfoData);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, int dwShareMode, IntPtr lpSecurityAttributes, int dwCreationDisposition, int dwFlagsAndAttributes, IntPtr hTemplateFile);

        private static string GetPrinterRegistryInstanceID(string PrinterName)
        {
            if (string.IsNullOrEmpty(PrinterName))
                throw new ArgumentNullException(nameof(PrinterName));
            string format = "SYSTEM\\CurrentControlSet\\Control\\Print\\Printers\\{0}\\PNPData";
            using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(string.Format(format, (object)PrinterName), RegistryKeyPermissionCheck.Default, RegistryRights.QueryValues))
            {
                if (registryKey == null)
                    throw new ArgumentOutOfRangeException(nameof(PrinterName), "This printer does not have PnP data.");
                return (string)registryKey.GetValue("DeviceInstanceId");
            }
        }

        private static string GetPrinterParentDeviceId(string RegistryInstanceID)
        {
            if (string.IsNullOrEmpty(RegistryInstanceID))
                throw new ArgumentNullException(nameof(RegistryInstanceID));
            IntPtr classDevs = OS_Win.SetupDiGetClassDevs(OS_Win.GUID_PRINTER_INSTALL_CLASS, RegistryInstanceID, IntPtr.Zero, 2U);
            if (classDevs.Equals((object)OS_Win.INVALID_HANDLE_VALUE))
                throw new Win32Exception();
            try
            {
                OS_Win.SP_DEVINFO_DATA DeviceInfoData = new OS_Win.SP_DEVINFO_DATA();
                DeviceInfoData.cbSize = (uint)Marshal.SizeOf(typeof(OS_Win.SP_DEVINFO_DATA));
                if (OS_Win.SetupDiEnumDeviceInfo(classDevs, 0U, ref DeviceInfoData) == 0)
                    throw new Win32Exception();
                uint pdnDevInst = 0;
                uint parent = OS_Win.CM_Get_Parent(out pdnDevInst, DeviceInfoData.DevInst, 0U);
                if ((int)parent != 0)
                    throw new Exception(string.Format("Failed to get parent of the device '{0}'. Error code: 0x{1:X8}", (object)RegistryInstanceID, (object)parent));
                uint pulLen = 0;
                uint deviceIdSize = OS_Win.CM_Get_Device_ID_Size(out pulLen, pdnDevInst, 0U);
                if ((int)deviceIdSize != 0)
                    throw new Exception(string.Format("Failed to get size of the device ID of the parent of the device '{0}'. Error code: 0x{1:X8}", (object)RegistryInstanceID, (object)deviceIdSize));
                ++pulLen;
                string Buffer = new string(char.MinValue, (int)pulLen);
                uint deviceId = OS_Win.CM_Get_Device_ID(pdnDevInst, Buffer, pulLen, 0U);
                if ((int)deviceId != 0)
                    throw new Exception(string.Format("Failed to get device ID of the parent of the device '{0}'. Error code: 0x{1:X8}", (object)RegistryInstanceID, (object)deviceId));
                return Buffer;
            }
            finally
            {
                OS_Win.SetupDiDestroyDeviceInfoList(classDevs);
            }
        }

        private static string GetUSBInterfacePath(string SystemDeviceInstanceID = null)
        {
            IntPtr classDevs = OS_Win.SetupDiGetClassDevs(OS_Win.GUID_DEVINTERFACE_USBPRINT, SystemDeviceInstanceID, IntPtr.Zero, 18U);
            if (classDevs.Equals((object)OS_Win.INVALID_HANDLE_VALUE))
                throw new Win32Exception();
            try
            {
                OS_Win.SP_DEVINFO_DATA DeviceInfoData = new OS_Win.SP_DEVINFO_DATA();
                DeviceInfoData.cbSize = (uint)Marshal.SizeOf(typeof(OS_Win.SP_DEVINFO_DATA));
                if (OS_Win.SetupDiEnumDeviceInfo(classDevs, 0U, ref DeviceInfoData) == 0)
                    throw new Win32Exception();
                OS_Win.SP_DEVICE_INTERFACE_DATA DeviceInterfaceData = new OS_Win.SP_DEVICE_INTERFACE_DATA();
                DeviceInterfaceData.cbSize = (uint)Marshal.SizeOf(typeof(OS_Win.SP_DEVICE_INTERFACE_DATA));
                if (OS_Win.SetupDiEnumDeviceInterfaces(classDevs, ref DeviceInfoData, OS_Win.GUID_DEVINTERFACE_USBPRINT, 0U, ref DeviceInterfaceData) == 0)
                    throw new Win32Exception();
                uint RequiredSize = 0;
                OS_Win.SetupDiGetDeviceInterfaceDetail(classDevs, ref DeviceInterfaceData, IntPtr.Zero, 0U, out RequiredSize, IntPtr.Zero);
                int lastWin32Error = Marshal.GetLastWin32Error();
                if (lastWin32Error != 122)
                    throw new Win32Exception(lastWin32Error);
                IntPtr num = Marshal.AllocCoTaskMem((int)RequiredSize);
                try
                {
                    switch (IntPtr.Size)
                    {
                        case 4:
                            Marshal.WriteInt32(num, 4 + Marshal.SystemDefaultCharSize);
                            break;
                        case 8:
                            Marshal.WriteInt32(num, 8);
                            break;
                        default:
                            throw new NotSupportedException("Architecture not supported.");
                    }
                    if (OS_Win.SetupDiGetDeviceInterfaceDetail(classDevs, ref DeviceInterfaceData, num, RequiredSize, out RequiredSize, IntPtr.Zero) == 0)
                        throw new Win32Exception();
                    return Marshal.PtrToStringAuto(new IntPtr(num.ToInt64() + Marshal.OffsetOf(typeof(OS_Win.SP_DEVICE_INTERFACE_DETAIL_DATA), "DevicePath").ToInt64()));
                }
                finally
                {
                    Marshal.FreeCoTaskMem(num);
                }
            }
            finally
            {
                OS_Win.SetupDiDestroyDeviceInfoList(classDevs);
            }
        }

        public static string GetUsbPrinterDevicePath(string name)
        {
            return OS_Win.GetUSBInterfacePath(OS_Win.GetPrinterParentDeviceId(OS_Win.GetPrinterRegistryInstanceID(name)));
        }

        public static SafeFileHandle OpenUsbPrinter(string name)
        {
            IntPtr file = OS_Win.CreateFile(OS_Win.GetUsbPrinterDevicePath(name), 3221225472U, 3U, IntPtr.Zero, 3U, 0U, IntPtr.Zero);
            if (file != OS_Win.INVALID_HANDLE_VALUE)
                return new SafeFileHandle(file, true);
            return (SafeFileHandle)null;
        }

        public static SafeFileHandle OpenCustomEngineeringUsbPrinter(string name)
        {
            IntPtr file = OS_Win.CreateFile(OS_Win.GetUSBInterfacePath((string)null), 3221225472U, 3U, IntPtr.Zero, 3U, 0U, (IntPtr)0);
            if (file != OS_Win.INVALID_HANDLE_VALUE)
                return new SafeFileHandle(file, true);
            return (SafeFileHandle)null;
        }

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool OpenPrinter(string pPrinterName, ref IntPtr phPrinter, IntPtr pDefault);

        [DllImport("winspool.drv", SetLastError = true)]
        public static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int StartDocPrinter(IntPtr hPrinter, int Level, ref OS_Win.DOC_INFO_1 pDocInfo);

        [DllImport("winspool.drv", SetLastError = true)]
        public static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", SetLastError = true)]
        public static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", SetLastError = true)]
        public static extern bool EndPagePrinter(IntPtr hPrinter);

        //[DllImport("winspool.drv", SetLastError = true)]
        //public static extern unsafe bool WritePrinter(IntPtr hPrinter, byte* data, int dataLen, ref int pWritten);

        [DllImport("winspool.drv", SetLastError = true)]
        public static extern bool GetPrinter(IntPtr hPrinter, int Level, ref OS_Win.PRINTER_INFO_6 pPrinter, int cbBuf, ref int pcbNeeded);

        //[DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        //public static extern unsafe int GetPrinterData(IntPtr hPrinter, string valueName, ref int pType, byte* pData, int nSize, ref int pcbNeeded);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetPrinterData(IntPtr hPrinter, string valueName, ref int pType, ref int pData, int nSize, ref int pcbNeeded);

        public static bool GetPrinterDataDword(IntPtr hPrinter, string valueName, ref int dw)
        {
            int pType = 0;
            int pcbNeeded = 0;
            if (OS_Win.GetPrinterData(hPrinter, valueName, ref pType, ref dw, 4, ref pcbNeeded) == 0)
                return pcbNeeded == 4;
            return false;
        }

        [DllImport("kernel32.dll")]
        public static extern bool ProcessIdToSessionId(int pid, ref int sid);

        public static int GetSessionId()
        {
            int sid = -1;
            OS_Win.ProcessIdToSessionId(Process.GetCurrentProcess().Id, ref sid);
            return sid;
        }

        public struct Point
        {
            public int x;
            public int y;
        }

        public enum CursorFlags
        {
            Hidden,
            Showing,
            Suppressed,
        }

        public struct CursorInfo
        {
            /// <summary>
            /// Specifies the size, in bytes, of the structure.
            /// The caller must set this to Marshal.SizeOf(typeof(CURSORINFO)).
            /// </summary>
            public int cbSize;
            /// <summary>Specifies the cursor state.</summary>
            public OS_Win.CursorFlags flags;
            /// <summary>Handle to the cursor.</summary>
            public IntPtr hCursor;
            /// <summary>
            /// A POINT structure that receives the screen coordinates of the cursor.
            /// </summary>
            public OS_Win.Point ptScreenPos;
        }

        [Flags]
        private enum ExitWindows : uint
        {
            LogOff = 0,
            ShutDown = 1,
            Reboot = 2,
            PowerOff = 8,
            RestartApps = 64, // 0x00000040
            Force = 4,
            ForceIfHung = 16, // 0x00000010
        }

        [Flags]
        private enum ShutdownReason : uint
        {
            MajorApplication = 262144, // 0x00040000
            MajorHardware = 65536, // 0x00010000
            MajorLegacyApi = 458752, // 0x00070000
            MajorOperatingSystem = 131072, // 0x00020000
            MajorOther = 0,
            MajorPower = MajorOperatingSystem | MajorApplication, // 0x00060000
            MajorSoftware = MajorOperatingSystem | MajorHardware, // 0x00030000
            MajorSystem = MajorHardware | MajorApplication, // 0x00050000
            MinorBlueScreen = 15, // 0x0000000F
            MinorCordUnplugged = 11, // 0x0000000B
            MinorDisk = 7,
            MinorEnvironment = 12, // 0x0000000C
            MinorHardwareDriver = 13, // 0x0000000D
            MinorHotfix = 17, // 0x00000011
            MinorHung = 5,
            MinorInstallation = 2,
            MinorMaintenance = 1,
            MinorMMC = 25, // 0x00000019
            MinorNetworkConnectivity = 20, // 0x00000014
            MinorNetworkCard = 9,
            MinorOther = 0,
            MinorOtherDriver = MinorInstallation | MinorEnvironment, // 0x0000000E
            MinorPowerSupply = 10, // 0x0000000A
            MinorProcessor = 8,
            MinorReconfig = 4,
            MinorSecurity = 19, // 0x00000013
            MinorSecurityFix = 18, // 0x00000012
            MinorSecurityFixUninstall = 24, // 0x00000018
            MinorServicePack = 16, // 0x00000010
            MinorServicePackUninstall = MinorServicePack | MinorReconfig | MinorInstallation, // 0x00000016
            MinorTermSrv = 32, // 0x00000020
            MinorUnstable = MinorReconfig | MinorInstallation, // 0x00000006
            MinorUpgrade = MinorMaintenance | MinorInstallation, // 0x00000003
            MinorWMI = MinorServicePack | MinorReconfig | MinorMaintenance, // 0x00000015
            FlagUserDefined = 1073741824, // 0x40000000
            FlagPlanned = 2147483648, // 0x80000000
        }

        private struct LUID
        {
            public uint LowPart;
            public int HighPart;
        }

        private struct LUID_AND_ATTRIBUTES
        {
            public OS_Win.LUID Luid;
            public uint Attributes;
        }

        private struct TOKEN_PRIVILEGES
        {
            public uint PrivilegeCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            public OS_Win.LUID_AND_ATTRIBUTES[] Privileges;
        }

        [Flags]
        public enum ExecutionState : uint
        {
            AwayModeRequired = 64, // 0x00000040
            Continuous = 2147483648, // 0x80000000
            DisplayRequired = 2,
            SystemRequired = 1,
        }

        [Flags]
        private enum FileSystemFeature : uint
        {
            CaseSensitiveSearch = 1,
            CasePreservedNames = 2,
            UnicodeOnDisk = 4,
            PersistentACLS = 8,
            FileCompression = 16, // 0x00000010
            VolumeQuotas = 32, // 0x00000020
            SupportsSparseFiles = 64, // 0x00000040
            SupportsReparsePoints = 128, // 0x00000080
            VolumeIsCompressed = 32768, // 0x00008000
            SupportsObjectIDs = 65536, // 0x00010000
            SupportsEncryption = 131072, // 0x00020000
            NamedStreams = 262144, // 0x00040000
            ReadOnlyVolume = 524288, // 0x00080000
            SequentialWriteOnce = 1048576, // 0x00100000
            SupportsTransactions = 2097152, // 0x00200000
        }

        /// <summary>
        /// contains information about the current state of both physical and virtual memory, including extended memory
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal class MEMORYSTATUSEX
        {
            /// <summary>
            /// Size of the structure, in bytes. You must set this member before calling GlobalMemoryStatusEx.
            /// </summary>
            public uint dwLength;
            /// <summary>
            /// Number between 0 and 100 that specifies the approximate percentage of physical memory that is in use (0 indicates no memory use and 100 indicates full memory use).
            /// </summary>
            public uint dwMemoryLoad;
            /// <summary>Total size of physical memory, in bytes.</summary>
            public ulong ullTotalPhys;
            /// <summary>Size of physical memory available, in bytes.</summary>
            public ulong ullAvailPhys;
            /// <summary>
            /// Size of the committed memory limit, in bytes. This is physical memory plus the size of the page file, minus a small overhead.
            /// </summary>
            public ulong ullTotalPageFile;
            /// <summary>
            /// Size of available memory to commit, in bytes. The limit is ullTotalPageFile.
            /// </summary>
            public ulong ullAvailPageFile;
            /// <summary>
            /// Total size of the user mode portion of the virtual address space of the calling process, in bytes.
            /// </summary>
            public ulong ullTotalVirtual;
            /// <summary>
            /// Size of unreserved and uncommitted memory in the user mode portion of the virtual address space of the calling process, in bytes.
            /// </summary>
            public ulong ullAvailVirtual;
            /// <summary>
            /// Size of unreserved and uncommitted memory in the extended portion of the virtual address space of the calling process, in bytes.
            /// </summary>
            public ulong ullAvailExtendedVirtual;

            /// <summary>
            /// Initializes a new instance of the <see cref="T:MEMORYSTATUSEX" /> class.
            /// </summary>
            public MEMORYSTATUSEX()
            {
                this.dwLength = (uint)Marshal.SizeOf(typeof(OS_Win.MEMORYSTATUSEX));
            }
        }

        internal struct IO_COUNTERS
        {
            public ulong ReadOperationCount;
            public ulong WriteOperationCount;
            public ulong OtherOperationCount;
            public ulong ReadTransferCount;
            public ulong WriteTransferCount;
            public ulong OtherTransferCount;
        }

        [Flags]
        private enum InternetGetConnectedStateFlags
        {
            INTERNET_CONNECTION_MODEM = 1,
            INTERNET_CONNECTION_LAN = 2,
            INTERNET_CONNECTION_PROXY = 4,
            INTERNET_CONNECTION_RAS_INSTALLED = 16, // 0x00000010
            INTERNET_CONNECTION_OFFLINE = 32, // 0x00000020
            INTERNET_CONNECTION_CONFIGURED = 64, // 0x00000040
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct PROCESS_BASIC_INFORMATION
        {
            public IntPtr ExitStatus;
            public IntPtr PebBaseAddress;
            public IntPtr AffinityMask;
            public IntPtr BasePriority;
            public UIntPtr UniqueProcessId;
            public IntPtr InheritedFromUniqueProcessId;

            public int Size
            {
                get
                {
                    return Marshal.SizeOf(typeof(OS_Win.PROCESS_BASIC_INFORMATION));
                }
            }
        }

        private enum WTS_INFO_CLASS
        {
            WTSSessionId = 4,
            WTSClientName = 10, // 0x0000000A
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct STARTUPINFO
        {
            public int cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public int dwX;
            public int dwY;
            public int dwXSize;
            public int dwYSize;
            public int dwXCountChars;
            public int dwYCountChars;
            public int dwFillAttribute;
            public int dwFlags;
            public short wShowWindow;
            public short cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        public struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public int dwProcessId;
            public int dwThreadId;
        }

        private struct LASTINPUTINFO
        {
            public uint cbSize;
            public uint dwTime;
        }

        public delegate bool EnumWindowsDelegate(IntPtr h, IntPtr lp);

        public delegate IntPtr WndProcDelegate(IntPtr h, uint m, IntPtr wParam, IntPtr lParam);

        public struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        public struct KEYBDINPUT
        {
            public short wVk;
            public short wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        public struct HARDWAREINPUT
        {
            public uint uMsg;
            public short wParamL;
            public short wParamH;
        }

        public struct INPUT
        {
            public uint type;
            public OS_Win._INPUTUnion U;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct _INPUTUnion
        {
            [FieldOffset(0)]
            public OS_Win.MOUSEINPUT mi;
            [FieldOffset(0)]
            public OS_Win.KEYBDINPUT ki;
            [FieldOffset(0)]
            public OS_Win.HARDWAREINPUT hi;
        }

        private enum SID_NAME_USE
        {
            SidTypeUser = 1,
            SidTypeGroup = 2,
            SidTypeDomain = 3,
            SidTypeAlias = 4,
            SidTypeWellKnownGroup = 5,
            SidTypeDeletedAccount = 6,
            SidTypeInvalid = 7,
            SidTypeUnknown = 8,
            SidTypeComputer = 9,
        }

        public struct MINIMIZEDMETRICS
        {
            public uint cbSize;
            public int iWidth;
            public int iHorzGap;
            public int iVertGap;
            public int iArrange;
        }

        public struct MSG
        {
            public IntPtr hwnd;
            public uint message;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public OS_Win.POINT pt;
        }

        public struct MINMAXINFO
        {
            public OS_Win.POINT ptReserved;
            public OS_Win.POINT ptMaxSize;
            public OS_Win.POINT ptMaxPosition;
            public OS_Win.POINT ptMinTrackSize;
            public OS_Win.POINT ptMaxTrackSize;
        }

        public struct MONITORINFO
        {
            public int cbSize;
            public OS_Win.RECT rcMonitor;
            public OS_Win.RECT rcWork;
            public int dwFlags;
        }

        public struct POINT
        {
            public int x;
            public int y;
        }

        public struct RECT
        {
            public int l;
            public int t;
            public int r;
            public int b;
        }

        public struct SHELLHOOKINFO
        {
            public IntPtr hwnd;
            public OS_Win.RECT rect;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct DEV_BROADCAST_HDR
        {
            public int dbch_size;
            public int dbch_devicetype;
            public int dbch_reserved;

            public DEV_BROADCAST_HDR(int dbt_deviceType)
            {
                this.dbch_size = Marshal.SizeOf(typeof(OS_Win.DEV_BROADCAST_HDR));
                this.dbch_devicetype = dbt_deviceType;
                this.dbch_reserved = 0;
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct DEV_BROADCAST_DEVICEINTERFACE
        {
            public int dbcc_size;
            public int dbcc_devicetype;
            public int dbcc_reserved;
            public Guid dbcc_classguid;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
            public string dbcc_name;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct DeviceInterfaceData
        {
            public int Size;
            public Guid InterfaceClassGuid;
            public int Flags;
            public IntPtr Reserved;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
        public struct DeviceInterfaceDetailData
        {
            public int Size;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
            public string DevicePath;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct HidCaps
        {
            public short Usage;
            public short UsagePage;
            public short InputReportByteLength;
            public short OutputReportByteLength;
            public short FeatureReportByteLength;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
            public short[] Reserved;
            public short NumberLinkCollectionNodes;
            public short NumberInputButtonCaps;
            public short NumberInputValueCaps;
            public short NumberInputDataIndices;
            public short NumberOutputButtonCaps;
            public short NumberOutputValueCaps;
            public short NumberOutputDataIndices;
            public short NumberFeatureButtonCaps;
            public short NumberFeatureValueCaps;
            public short NumberFeatureDataIndices;
        }

        public struct HIDD_ATTRIBUTES
        {
            public int Size;
            public short VendorID;
            public short ProductID;
            public short VersionNumber;
        }

        public struct HidP_Range
        {
            public short UsageMin;
            public short UsageMax;
            public short StringMin;
            public short StringMax;
            public short DesignatorMin;
            public short DesignatorMax;
            public short DataIndexMin;
            public short DataIndexMax;
        }

        public struct HidP_NotRange
        {
            public short Usage;
            public short Reserved1;
            public short StringIndex;
            public short Reserved2;
            public short DesignatorIndex;
            public short Reserved3;
            public short DataIndex;
            public short Reserved4;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct HidP_Button_Caps
        {
            [FieldOffset(0)]
            public short UsagePage;
            [FieldOffset(2)]
            public byte ReportID;
            [MarshalAs(UnmanagedType.U1)]
            [FieldOffset(3)]
            public bool IsAlias;
            [FieldOffset(4)]
            public short BitField;
            [FieldOffset(6)]
            public short LinkCollection;
            [FieldOffset(8)]
            public short LinkUsage;
            [FieldOffset(10)]
            public short LinkUsagePage;
            [MarshalAs(UnmanagedType.U1)]
            [FieldOffset(12)]
            public bool IsRange;
            [MarshalAs(UnmanagedType.U1)]
            [FieldOffset(13)]
            public bool IsStringRange;
            [MarshalAs(UnmanagedType.U1)]
            [FieldOffset(14)]
            public bool IsDesignatorRange;
            [MarshalAs(UnmanagedType.U1)]
            [FieldOffset(15)]
            public bool IsAbsolute;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            [FieldOffset(16)]
            public int[] Reserved;
            [FieldOffset(56)]
            public OS_Win.HidP_Range Range;
            [FieldOffset(56)]
            public OS_Win.HidP_NotRange NotRange;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct HIDP_VALUE_CAPS
        {
            [FieldOffset(0)]
            public short UsagePage;
            [FieldOffset(2)]
            public byte ReportID;
            [MarshalAs(UnmanagedType.I1)]
            [FieldOffset(3)]
            public bool IsAlias;
            [FieldOffset(4)]
            public short BitField;
            [FieldOffset(6)]
            public short LinkCollection;
            [FieldOffset(8)]
            public short LinkUsage;
            [FieldOffset(10)]
            public short LinkUsagePage;
            [MarshalAs(UnmanagedType.I1)]
            [FieldOffset(12)]
            public bool IsRange;
            [MarshalAs(UnmanagedType.I1)]
            [FieldOffset(13)]
            public bool IsStringRange;
            [MarshalAs(UnmanagedType.I1)]
            [FieldOffset(14)]
            public bool IsDesignatorRange;
            [MarshalAs(UnmanagedType.I1)]
            [FieldOffset(15)]
            public bool IsAbsolute;
            [MarshalAs(UnmanagedType.I1)]
            [FieldOffset(16)]
            public bool HasNull;
            [FieldOffset(17)]
            public char Reserved;
            [FieldOffset(18)]
            public short BitSize;
            [FieldOffset(20)]
            public short ReportCount;
            [FieldOffset(22)]
            public short Reserved2a;
            [FieldOffset(24)]
            public short Reserved2b;
            [FieldOffset(26)]
            public short Reserved2c;
            [FieldOffset(28)]
            public short Reserved2d;
            [FieldOffset(30)]
            public short Reserved2e;
            [FieldOffset(32)]
            public int UnitsExp;
            [FieldOffset(36)]
            public int Units;
            [FieldOffset(40)]
            public int LogicalMin;
            [FieldOffset(44)]
            public int LogicalMax;
            [FieldOffset(48)]
            public int PhysicalMin;
            [FieldOffset(52)]
            public int PhysicalMax;
            [FieldOffset(56)]
            public OS_Win.HidP_Range Range;
            [FieldOffset(56)]
            public OS_Win.HidP_NotRange NotRange;
        }

        public enum HIDP_REPORT_TYPE
        {
            HidP_Input,
            HidP_Output,
            HidP_Feature,
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct HIDP_DATA
        {
            [FieldOffset(0)]
            public short DataIndex;
            [FieldOffset(2)]
            public short Reserved;
            [FieldOffset(4)]
            public int RawValue;
            [MarshalAs(UnmanagedType.U1)]
            [FieldOffset(4)]
            public bool On;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SP_DEVINFO_DATA
        {
            public uint cbSize;
            public Guid ClassGuid;
            public uint DevInst;
            public IntPtr Reserved;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SP_DEVICE_INTERFACE_DATA
        {
            public uint cbSize;
            public Guid InterfaceClassGuid;
            public uint Flags;
            public IntPtr Reserved;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Auto)]
        public struct SP_DEVICE_INTERFACE_DETAIL_DATA
        {
            public uint cbSize;
            public char DevicePath;
        }

        public struct DOC_INFO_1
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pDocName;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pOutputFile;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pDatatype;
        }

        public struct PRINTER_DEFAULTS
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pDatatype;
            public IntPtr pDevMode;
            public int DesiredAccess;
        }

        public struct PRINTER_INFO_6
        {
            public int dwStatus;
        }
    }

    public class ClickOnPointTool
    {

        [DllImport("user32.dll")]
        static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);

        [DllImport("user32.dll")]
        internal static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs, int cbSize);

#pragma warning disable 649
        internal struct INPUT
        {
            public UInt32 Type;
            public MOUSEKEYBDHARDWAREINPUT Data;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct MOUSEKEYBDHARDWAREINPUT
        {
            [FieldOffset(0)]
            public MOUSEINPUT Mouse;
        }

        internal struct MOUSEINPUT
        {
            public Int32 X;
            public Int32 Y;
            public UInt32 MouseData;
            public UInt32 Flags;
            public UInt32 Time;
            public IntPtr ExtraInfo;
        }

#pragma warning restore 649


        public static void ClickOnPoint(IntPtr wndHandle, Point clientPoint)
        {
            var oldPos = Cursor.Position;

            /// get screen coordinates
            ClientToScreen(wndHandle, ref clientPoint);

            /// set cursor on coords, and press mouse
            Cursor.Position = new Point(clientPoint.X, clientPoint.Y);

            var inputMouseDown = new INPUT();
            inputMouseDown.Type = 0; /// input type mouse
            inputMouseDown.Data.Mouse.Flags = 0x0002; /// left button down

            var inputMouseUp = new INPUT();
            inputMouseUp.Type = 0; /// input type mouse
            inputMouseUp.Data.Mouse.Flags = 0x0004; /// left button up

            var inputs = new INPUT[] { inputMouseDown, inputMouseUp };
            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));

            /// return mouse 
            Cursor.Position = oldPos;
        }

    }
}

#pragma warning restore 0649
#pragma warning restore 0168