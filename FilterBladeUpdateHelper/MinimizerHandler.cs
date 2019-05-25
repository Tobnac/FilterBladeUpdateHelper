using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilterBladeUpdateHelper
{
    /// <summary>
    /// starts the minimizer and checks its outputs
    /// </summary>
    public class MinimizerHandler
    {
        private const string MinimizeStartCommand = "npm run minification";

        public void Run()
        {
            Logger.Log("Minimizer Starting:", 1);
            this.ExecuteCmdCommand(MinimizeStartCommand);
        }

        public void ExecuteCmdCommand(string command)
        {
            var fullCommand = "/c " + command;

            var processInfo = new ProcessStartInfo("cmd.exe", fullCommand)
            {
                WorkingDirectory = FilterBladeUpdateHelper.FbDirPath,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true                
            };

            var process = Process.Start(processInfo);
           
            process.OutputDataReceived += (object sender, DataReceivedEventArgs e) => Logger.Log(e.Data, 1);
            process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) => { if (e.Data != null) throw new Exception(e.Data); };
            
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
            if (process.ExitCode != 0) throw new Exception();
            Logger.Log("Minimizer done!", 0);
            process.Close();
        }
    }
}
