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
    public class OptionFileVersioner
    {
        private const string optionFilePath = FilterBladeUpdateHelper.FbDirPath + @"/datafiles/optionfiles/optionFile085d.options";

        public void UpdateCustomizerVersion()
        {
            var optLines = System.IO.File.ReadAllLines(optionFilePath);
            var found = false;

            for (int i = 0; i < optLines.Length; i++)
            {
                var line = optLines[i];
                if (line.Contains("Info_OptionVersion") && line.Contains(VersionController.OldVersion))
                {
                    // update version
                    var newLine = line.Replace(VersionController.OldVersion, VersionController.NewVersion);
                    Logger.Log("Updated optionFile version", 0);

                    // overwrite line
                    optLines[i] = newLine;
                    found = true;
                    break;
                }
            }

            if (!found) throw new Exception();

            // overwrite file
            System.IO.File.WriteAllLines(optionFilePath, optLines);
        }

        public void UpdateFilterData(string newVersion, string oldVersion)
        {
            var optLines = System.IO.File.ReadAllLines(optionFilePath);
            var found = false;
            var dateKey = "updated on ";

            for (int i = 0; i < optLines.Length; i++)
            {
                var line = optLines[i];
                if (line.Contains("Info_FilterDescription") && line.Contains(oldVersion) && line.Contains(dateKey))
                {
                    // update version
                    var newLine = line.Replace(oldVersion, newVersion);

                    // update date
                    var dateStart = line.Substring(line.IndexOf(dateKey) + dateKey.Length);
                    var date = dateStart.Substring(0, dateStart.IndexOf('"'));
                    var newDate = Helper.GetCurrentDate();
                    newLine = newLine.Replace(date, newDate);
                    Logger.Log("Updated date from '" + date + "' to '" + newDate + "'.", 1);

                    // overwrite line
                    optLines[i] = newLine;
                    found = true;
                    break;
                }
            }

            if (!found) throw new Exception();

            // overwrite file
            System.IO.File.WriteAllLines(optionFilePath, optLines);
        }
    }
}
