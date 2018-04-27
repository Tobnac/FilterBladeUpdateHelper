using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilterBladeUpdateHelper
{
    public class Logger
    {
        /// <summary>
        /// ======== STATIC CONTENT =================
        /// </summary>

        public static int LogTolerance = 1;
        public static Logger Instance { get; set; }

        static Logger()
        {
            Instance = new Logger();
        }

        public static void Log(string message, int importance)
        {
            Instance.LogMsg(message, importance);
        }

        /// <summary>
        /// ========= NON-STATIC CONTENT ========================
        /// </summary>

        public void LogMsg(string message, int importance)
        {
            if (importance < LogTolerance) return;

            Console.WriteLine("{" + importance + "} " + message);
        }
    }
}
