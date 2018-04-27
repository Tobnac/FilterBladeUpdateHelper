using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilterBladeUpdateHelper
{
    /// <summary>
    /// mainly used to edit the cachebuster version in FilterBlade.js
    /// </summary>
    public class JS_FileUpdater
    {
        public string[] FB_JS_Lines { get; set; }
        private const string fbJsPath = FilterBladeUpdateHelper.FbDirPath + "\\js\\FilterBlade.js";

        public JS_FileUpdater()
        {
            this.FB_JS_Lines = System.IO.File.ReadAllLines(fbJsPath);
        }

        public void Run()
        {
            this.UpdateFB_JS_CacheBuster();
        }

        private void UpdateFB_JS_CacheBuster()
        {
            var found = false;

            for (int i = 0; i < this.FB_JS_Lines.Length; i++)
            {
                var line = this.FB_JS_Lines[i];
                if (line.Contains("this.aversion") && line.Contains(VersionController.OldVersion))
                {
                    var newLine = line.Replace(VersionController.OldVersion, VersionController.NewVersion);
                    this.FB_JS_Lines[i] = newLine;
                    Logger.Log("Update cachebuster version", 0);
                    found = true;
                    break;
                }
            }

            if (!found) throw new Exception();

            // overwrite file
            System.IO.File.WriteAllLines(fbJsPath, this.FB_JS_Lines);
        }

        public string FindCurrentCacheBusterVersion()
        {           
            foreach (var line in this.FB_JS_Lines)
            {
                if (line.Contains("this.aversion"))
                {
                    var start = line.Substring(line.IndexOf("\"") + 1);
                    var full = start.Substring(0, start.IndexOf("\""));
                    Logger.Log("Found current version: " + full, 0);
                    return full;
                }
            }

            throw new Exception();
        }
    }
}
