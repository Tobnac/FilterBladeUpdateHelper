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
            throw new NotImplementedException();
        }

        public void UpdateFilterData(string newVersion, string oldVersion)
        {
            var optLines = System.IO.File.ReadAllLines(optionFilePath);
            var found = false;

            for (int i = 0; i < optLines.Length; i++)
            {
                var line = optLines[i];
                if (line.Contains("Info_FilterDescription") && line.Contains(oldVersion))
                {
                    // update version
                    var newLine = line.Replace(oldVersion, newVersion);

                    //todo update date

                    optLines[i] = newLine;
                    found = true;
                    break;
                }
            }

            if (!found) throw new Exception();

            System.IO.File.WriteAllLines(optionFilePath, optLines);

            throw new NotImplementedException();
        }
    }
}
