using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilterBladeUpdateHelper
{
    /// <summary>
    /// -> read current FB version
    /// -> get user input for next version:
    /// --> increase letter
    /// --> increase version
    /// --> increase main version
    /// --> manual direct input
    /// </summary>
    public class VersionController
    {
        public static string NewVersion { get; private set; }
        public static string OldVersion { get; private set; }

        public void Run()
        {
            OldVersion = this.FindCurrentVersion();
            NewVersion = this.EvaluateUserInput(Console.ReadLine());
        }

        private string EvaluateUserInput(string v)
        {
            throw new NotImplementedException();
        }

        private string FindCurrentVersion()
        {
            throw new NotImplementedException();
        }
    }
}
