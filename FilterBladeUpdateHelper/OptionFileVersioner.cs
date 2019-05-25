using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilterBladeUpdateHelper
{
    /// <summary>
    /// edit optionFile to update version + last update date
    /// </summary>
    public static class OptionFileVersioner
    {
        private const string OptionFilePath = FilterBladeUpdateHelper.FbDirPath + @"/datafiles/optionfiles/optionFile085d.options";

        public static void UpdateCustomizerVersion()
        {
            var optLines = System.IO.File.ReadAllLines(OptionFilePath);
            var found = false;

            for (var i = 0; i < optLines.Length; i++)
            {
                var line = optLines[i];
                if (!line.Contains("Info_OptionVersion") || !line.Contains(VersionController.OldVersion)) continue;
                
                // update version
                var newLine = line.Replace(VersionController.OldVersion, VersionController.NewVersion);
                Logger.Log("Updated optionFile version", 0);

                // overwrite line
                optLines[i] = newLine;
                found = true;
                break;
            }

            if (!found) throw new Exception("unable to update version in optionFile");

            // overwrite file
            System.IO.File.WriteAllLines(OptionFilePath, optLines);
        }

        public static void UpdateFilterData(string newVersion, string oldVersion)
        {
            var optLines = System.IO.File.ReadAllLines(OptionFilePath);
            var found = false;
            const string key = "updated on ";

            for (var i = 0; i < optLines.Length; i++)
            {
                var line = optLines[i];
                if (!line.Contains("Info_FilterDescription") || !line.Contains(oldVersion) || !line.Contains(key)) continue;
                
                // update version
                var newLine = line.Replace(oldVersion, newVersion);

                // update date
                var dateStart = line.Substring(line.IndexOf(key, StringComparison.Ordinal) + key.Length);
                var date = dateStart.Substring(0, dateStart.IndexOf('"'));
                var newDate = Helper.GetCurrentDate();
                newLine = newLine.Replace(date, newDate);
                Logger.Log("Updated date from '" + date + "' to '" + newDate + "'.", 1);

                // overwrite line
                optLines[i] = newLine;
                found = true;
                break;
            }

            if (!found) throw new Exception("unable to update optFile filter data");

            // overwrite file
            System.IO.File.WriteAllLines(OptionFilePath, optLines);
        }
    }
}
