// Windows related action utility to be used for Web automation
// David Piao

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OS_Util;

namespace PCKLIB
{
    class WinUtil
    {
        static int timeout = 10000;
        static object locker = new object();

        private static bool remove_javascript_alert(IntPtr h, IntPtr lp)
        {
            string title = OS_Win.GetWindowText(h, 256);
            if (title == null)
                return true;
            if (title.ToLower().Contains("javascript confirm"))
            {
                OS_Win.SetForegroundWindow(h);
                System.Windows.Forms.SendKeys.SendWait("{TAB}");
                System.Windows.Forms.SendKeys.SendWait("{ENTER}");
            }
            return true;
        }
        public static void check_javascript_alert()
        {
            OS_Win.EnumWindows(new OS_Win.EnumWindowsDelegate(remove_javascript_alert), IntPtr.Zero);
        }
        public async static Task<bool> find_open_dlg(int worker_ID)
        {
            lock(locker)
            {
                MainApp.log_info($"#{worker_ID}: Checking open file dialog"); ;

                Stopwatch w = new Stopwatch();
                w.Start();
                IntPtr hnd = IntPtr.Zero;
                IntPtr par_hnd = IntPtr.Zero;
                string[] title = { "Open", "Open File" };
                string sel_t = "N";
                w.Start();
                while (w.ElapsedMilliseconds < timeout)
                {
                    foreach (var t in title)
                    {
                        hnd = OS_Win.FindWindow("#32770", t);
                        if (hnd != IntPtr.Zero)
                        {
                            par_hnd = OS_Win.GetParent(hnd);
                            if (par_hnd != null)
                            {
                                sel_t = t;
                                break;
                            }
                        }
                    }
                    if (sel_t != "N")
                        break;
                    Thread.Sleep(100);
                }
                w.Stop();
                if (w.ElapsedMilliseconds >= timeout)
                {
                    MainApp.log_info($"#{worker_ID}: Can not find the open file dialog. (timeout)");
                    return false;
                }
                return true;
            }
        }
        public async static Task<bool> set_upload_file(string path, int worker_ID)
        {
            lock(locker)
            {
                MainApp.log_info($"#{worker_ID}: Upload file dialog is opened. If it stays open, click 'Open' manually"); ;

                IntPtr hnd = IntPtr.Zero;
                IntPtr par_hnd = IntPtr.Zero;
                Stopwatch w = new Stopwatch();
                string[] title = { "Open", "Open File" };
                string sel_t = "N";
                w.Start();
                while (w.ElapsedMilliseconds < timeout)
                {
                    foreach (var t in title)
                    {
                        hnd = OS_Win.FindWindow("#32770", t);
                        if (hnd != IntPtr.Zero)
                        {
                            par_hnd = OS_Win.GetParent(hnd);
                            if (par_hnd != null)
                            {
                                sel_t = t;
                                break;
                            }
                        }
                    }
                    if (sel_t != "N")
                        break;
                    Thread.Sleep(100);
                }
                w.Stop();
                Thread.Sleep(500);
                if (w.ElapsedMilliseconds >= timeout)
                {
                    MainApp.log_info($"#{worker_ID}: Can not find the open file dialog. (timeout)");
                    return false;
                }

                MainApp.log_info($"#{worker_ID}: Upload file: find window by class and name return {hnd.ToString("X4")} and the parent is {par_hnd.ToString("X4")}");

                w.Start();
                while (w.ElapsedMilliseconds < 5000)
                {
                    OS_Win.SendMessage(hnd, OS_Win.WM_ACTIVATE, (IntPtr)0, (IntPtr)0);
                    OS_Win.SetForegroundWindow(hnd);
                    string keys = "";
                    foreach (char key in path)
                        keys += "{" + key + "}";
                    System.Windows.Forms.SendKeys.SendWait(path);
                    break;
                    //StringBuilder written = new StringBuilder();
                    //OS_Win.GetDlgItemText(hnd, 0x47C, written, 512);
                    //Thread.Sleep(500);
                    //if (written.ToString() == path)
                    //    break;
                    //MainApp.log_info($"#{worker_ID}: Send key result differs. {written}-{path}");
                    //System.Windows.Forms.SendKeys.SendWait("^+{A}");
                }
                w.Stop();

                OS_Win.SetForegroundWindow(hnd);
                System.Windows.Forms.SendKeys.SendWait("%+{O}");
                Thread.Sleep(100);
                MainApp.log_info($"#{worker_ID}: Alt+O clicked. ({hnd.ToString("X4")})");
                return true;
            }

        }
    }
}
