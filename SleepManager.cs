// To be used for monitoring PC sleep setting
// David Piao

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PCKLIB
{
    public class SleepManager
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

        [FlagsAttribute]
        public enum EXECUTION_STATE : uint
        {
            ES_AWAYMODE_REQUIRED = 0x00000040,
            ES_CONTINUOUS = 0x80000000,
            ES_DISPLAY_REQUIRED = 0x00000002,
            ES_SYSTEM_REQUIRED = 0x00000001
            // Legacy flag, should not be used.
            // ES_USER_PRESENT = 0x00000004
        }
        public static void PreventSleep(bool prevent = true)
        {
            EXECUTION_STATE ret;
            if (prevent)
            {
                ret = SetThreadExecutionState(EXECUTION_STATE.ES_DISPLAY_REQUIRED | EXECUTION_STATE.ES_CONTINUOUS | EXECUTION_STATE.ES_SYSTEM_REQUIRED);
                if(ret != null)
                    MainApp.log_error("The PC will not sleep");
                else
                    MainApp.log_error("Setting thread execution state failed.");
            }
            else
            {

                ret = SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
                if (ret != null)
                    MainApp.log_error("The PC sleep setting set to default");
                else
                    MainApp.log_error("Setting thread execution state failed.");
            }
        }
    }
}
