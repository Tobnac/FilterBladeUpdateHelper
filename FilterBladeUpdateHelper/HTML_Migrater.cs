using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilterBladeUpdateHelper
{
    /// <summary>
    /// merges the new dev HTML file into the release file
    /// updates manual cachebuster versions for file imports
    /// </summary>
    public class HTML_Migrater
    {
        private List<string> releaseImports;
        private readonly string devFilePath;
        private readonly string releaseFilePath;
        private const string importStartIdent = "<StartImport>";
        private const string importEndIdent = "<EndImport>";

        private List<string> releaseLines;
        private List<string> devLines;

        public HTML_Migrater(string devPath, string releasePath)
        {
            this.devFilePath = FilterBladeUpdateHelper.FbDirPath + devPath;
            this.releaseFilePath = FilterBladeUpdateHelper.FbDirPath + releasePath;
        }

        public void Run()
        {
            // read both files and save their lines
            // -> we will edit those lineLists and later insert them back into their files
            this.ReadFiles();


            // update date and version in devHTML
            this.UpdateDateInDevHTML();

            // find script-part in releaseHTML an save it
            this.releaseImports = this.GetReleaseImports();


            // copy devHTML into releaseHTML
            this.MigrateDevIntoRelease();


            // replace fullImports with releaseImports in releaseHTML
            this.RevertReleaseImports();

            // update cacheBuster version in imports in both files
            this.UpdateImportCachbusting(this.devLines);
            this.UpdateImportCachbusting(this.releaseLines);


            // insert edited lines back into original files
            this.Apply();
        }

        private void ReadFiles()
        {
            this.devLines = System.IO.File.ReadAllLines(this.devFilePath).ToList();
            this.releaseLines = System.IO.File.ReadAllLines(this.releaseFilePath).ToList();
        }

        private void UpdateDateInDevHTML()
        {
            int state = 0;
            var key = "Version " + VersionController.OldVersion + ",<br>";
            var endKey = "</label>";

            for (int i = 0; i < this.devLines.Count; i++)
            {
                var line = this.devLines[i];
                if (state == 0 && line.Contains(importStartIdent)) state++;
                else if (state == 1 && line.Contains(importEndIdent)) state++;
                else if (state == 2 && line.Contains("<label") && line.Contains(key) && line.Contains(endKey))
                {                
                    // update version
                    var newLine = line.Replace(VersionController.OldVersion, VersionController.NewVersion);

                    // update date
                    var oldDateStart = line.Substring(line.IndexOf(key) + key.Length);
                    var oldDate = oldDateStart.Substring(0, oldDateStart.IndexOf(endKey));
                    var newDate = Helper.GetCurrentDate();
                    newLine = newLine.Replace(oldDate, newDate);
                    Logger.Log("Updated HTML date from '" + oldDate + "' to '" + newDate + "'.", 1);

                    // apply
                    this.devLines[i] = newLine;
                    return;
                }
            }

            throw new Exception();
        }

        private List<string> GetReleaseImports()
        {
            var result = new List<string>();
            var state = 0;

            foreach (var line in this.releaseLines)
            {
                if (state == 0 && line.Contains(importStartIdent)) state++;
                else if (state == 1 && line.Contains(importEndIdent)) return result;
                else if (state == 1)
                {
                    result.Add(line);
                }
            }

            throw new Exception();
        }

        private void MigrateDevIntoRelease()
        {
            this.releaseLines = new List<string>(this.devLines);
        }

        private void RevertReleaseImports()
        {
            var state = 0;

            // delete devImports
            for (int i = 0; i < this.releaseLines.Count; i++)
            {
                var line = this.releaseLines[i];

                if (line.Contains(importStartIdent))
                {
                    state++;
                }

                else if (line.Contains(importEndIdent))
                {
                    // wrong imports deleted, now insert the right ones
                    this.releaseLines.InsertRange(i, this.releaseImports);
                    state++;
                    break;
                }

                else if (state == 1)
                {
                    // delete wrong imports
                    this.releaseLines.RemoveAt(i);
                    i--;
                }
            }
        }

        private void UpdateImportCachbusting(List<string> lineList)
        {
            var state = 0;
            var key = "?v=" + VersionController.OldVersion.Replace(".", "");
            var newKey = "?v=" + VersionController.NewVersion.Replace(".", "");

            for (int i = 0; i < lineList.Count; i++)
            {
                var line = lineList[i];

                if (state == 0 && line.Contains(importStartIdent)) state++;
                else if (state == 1 && line.Contains(importEndIdent)) return;
                else if (state == 1 && line.Contains(key))
                {
                    var newLine = line.Replace(key, newKey);
                    lineList[i] = newLine;
                }
            }
        }

        private void Apply()
        {
            System.IO.File.WriteAllLines(this.devFilePath, this.devLines);
            System.IO.File.WriteAllLines(this.releaseFilePath, this.releaseLines);
            Logger.Log("HTML file changes applied", 0);
        }
    }
}
