using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilterBladeUpdateHelper
{
    /// <summary>
    /// mainly used to edit the cacheBuster version in FilterBlade.js
    /// </summary>
    public class Js_FileUpdater
    {
        public string[] Fb_Js_Lines { get; set; }
        private const string FbJsPath = FilterBladeUpdateHelper.FbDirPath + "\\js\\FilterBlade.js";

        public Js_FileUpdater()
        {
            if (!System.IO.File.Exists(FbJsPath)) throw new Exception("Error: FilterBlade.js was not found at: " + FbJsPath);
            this.Fb_Js_Lines = System.IO.File.ReadAllLines(FbJsPath);
        }

        public void Run()
        {
            this.UpdateFB_JS_CacheBuster();
        }

        private void UpdateFB_JS_CacheBuster()
        {
            var found = false;

            for (var i = 0; i < this.Fb_Js_Lines.Length; i++)
            {
                var line = this.Fb_Js_Lines[i];
                if (!this.IsVersionLine(line) || !line.Contains(VersionController.OldVersion)) continue;
                
                var newLine = line.Replace(VersionController.OldVersion, VersionController.NewVersion);
                this.Fb_Js_Lines[i] = newLine;
                Logger.Log("Update cacheBuster version", 0);
                found = true;
                break;
            }

            if (!found) throw new Exception("unable to update version in FB.js");

            // overwrite file
            System.IO.File.WriteAllLines(FbJsPath, this.Fb_Js_Lines);
        }

        public string FindCurrentCacheBusterVersion()
        {
            var line = this.Fb_Js_Lines.SingleOrDefault(this.IsVersionLine);
            if (line == null) throw new Exception("Could not find exactly one line with current (cacheBuster) version in FilterBlade.js");

            var startIndex = line.IndexOf("\"", StringComparison.Ordinal);
            var start = line.Substring(startIndex + 1);
            
            if (startIndex - line.IndexOf("=", StringComparison.Ordinal) != 2) throw new Exception("unexpected line structure for FB version: " + line);
                
            var full = start.Substring(0, start.IndexOf("\"", StringComparison.Ordinal));
            Logger.Log("Found current version: " + full, 0);
            
            if (full.Length > 15 || full.Length < 5) throw new Exception("version seems unrealistically short/long: " + full);
            if (full.Split('.').Length != 3) throw new Exception("version has incorrect format. expected 1.2.3b, got: " + full);
            
            return full;
        }

        private bool IsVersionLine(string line)
        {
            const string key = "this.aversion";
            return line.Contains(key) && line.Contains("\"") && line.Contains("=");
        }
    }
}
