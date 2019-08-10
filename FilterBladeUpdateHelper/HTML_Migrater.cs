using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilterBladeUpdateHelper
{
    /// <summary>
    /// merges the new dev HTML file into the release file
    /// updates manual cacheBuster versions for file imports
    /// </summary>
    public class Html_Migrater
    {
        private List<string> releaseImports;
        private readonly string devFilePath;
        private readonly string releaseFilePath;
        private const string ImportStartIdent = "<StartImport>";
        private const string ImportEndIdent = "<EndImport>";

        private List<string> releaseLines;
        private List<string> devLines;

        public Html_Migrater(string devPath, string releasePath)
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
            this.UpdateDateInDevHtml();

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

        private void UpdateDateInDevHtml()
        {
            var state = 0;
            var key = "Version " + VersionController.OldVersion + ", ";
            const string endKey = "</label>";

            for (var i = 0; i < this.devLines.Count; i++)
            {
                var line = this.devLines[i];

                switch (state)
                {
                    case 0:
                        if (line.Contains(ImportStartIdent)) state++;
                        break;
                    
                    case 1:
                        if (line.Contains(ImportEndIdent)) state++;
                        break;
                    
                    case 2:
                        if (!line.Contains("<label") || !line.Contains(key) || !line.Contains(endKey)) break;
                        
                        // update version
                        var newLine = line.Replace(VersionController.OldVersion, VersionController.NewVersion);

                        // update date
                        var oldDateStart = line.Substring(line.IndexOf(key, StringComparison.Ordinal) + key.Length);
                        var oldDate = oldDateStart.Substring(0, oldDateStart.IndexOf(endKey, StringComparison.Ordinal));
                        var newDate = Helper.GetCurrentDate();
                        newLine = newLine.Replace(oldDate, newDate);
                        Logger.Log("Updated HTML date from '" + oldDate + "' to '" + newDate + "'.", 1);

                        // apply
                        this.devLines[i] = newLine;
                        return;
                    
                    default:
                        throw new Exception("unexpected state in html date update");
                }
            }

            throw new Exception("unexpected behaviour in html date update");
        }

        private List<string> GetReleaseImports()
        {
            var result = new List<string>();
            var state = 0;

            foreach (var line in this.releaseLines)
            {
                if (state == 0 && line.Contains(ImportStartIdent)) state++;
                else if (state == 1 && line.Contains(ImportEndIdent)) return result;
                else if (state == 1)
                {
                    result.Add(line);
                }
            }

            throw new Exception("error while saving release html imports");
        }

        private void MigrateDevIntoRelease()
        {
            this.releaseLines = new List<string>(this.devLines);
        }

        private void RevertReleaseImports()
        {
            var state = 0;

            // delete devImports
            for (var i = 0; i < this.releaseLines.Count; i++)
            {
                var line = this.releaseLines[i];

                if (line.Contains(ImportStartIdent))
                {
                    state++;
                }

                else if (line.Contains(ImportEndIdent))
                {
                    // wrong imports deleted, now insert the right ones
                    this.releaseLines.InsertRange(i, this.releaseImports);
                    return;
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
            var updatedLineCount = 0;

            for (var i = 0; i < lineList.Count; i++)
            {
                var line = lineList[i];

                switch (state)
                {
                    case 0 when line.Contains(ImportStartIdent):
                        state++;
                        break;
                    
                    case 1 when line.Contains(ImportEndIdent):
                        if (updatedLineCount < 3) throw new Exception("error: did not successfully update version in HTML imports");
                        return;
                    
                    case 1 when line.Contains(key):
                        var newLine = line.Replace(key, newKey);
                        updatedLineCount++;
                        lineList[i] = newLine;
                        break;
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
