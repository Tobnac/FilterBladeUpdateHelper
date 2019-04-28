using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilterBladeUpdateHelper
{
    /// <summary>
    /// get info: are there new filter files?
    /// -> get filter files
    /// -> replace old files
    /// -> send signal to update filter version + date in optionFile
    /// </summary>
    public class FilterFileUpdater
    {
        private const string FilterDirPath = FilterBladeUpdateHelper.FbDirPath + "\\datafiles\\filters\\NeverSink";
        private const string NewFileDic = @"C:\Users\Tobnac\Downloads\RESULTS";

        public OptionFileVersioner OptionFileVersioner { get; set; } = new OptionFileVersioner();

        public void Run()
        {
            if (this.FindNewFilterFiles())
            {
                Logger.Log("New filter files found", 1);
                this.UpdateFilterData();
                this.InsertNewFilterFiles();                
            }

            Logger.Log("No new filter files found", 1);
        }

        private void UpdateFilterData()
        {
            var newVersion = this.GetFilterVersion(true);
            var oldVersion = this.GetFilterVersion(false);
            Logger.Log("New filter version: " + newVersion, 0);
            Logger.Log("Old filter version: " + oldVersion, 0);
            if (newVersion == oldVersion) Logger.Log("New and Old filter version is identical", 5);
            this.OptionFileVersioner.UpdateFilterData(newVersion, oldVersion);
        }

        private string GetFilterVersion(bool newFiles)
        {
            string filterPath;
            if (newFiles) filterPath = NewFileDic + "/NeverSink's filter - 1-REGULAR.filter";
            else filterPath = FilterDirPath + "/Normal/NeverSink's filter - 1-REGULAR.filter";
            // # VERSION:		5.73

            var filterLines = System.IO.File.ReadAllLines(filterPath);

            foreach (var line in filterLines)
            {
                int index;
                var ident = "version:";
                string reLine;
                if ((index = (reLine = line.ToLower().Replace('\t', ' ')).IndexOf(ident)) != -1)
                {
                    var version = reLine.Substring(index + ident.Length).Trim();
                    return version;
                }
            }

            throw new Exception();
        }

        private void InsertNewFilterFiles()
        {
            var counter = 0;

            // normal style
            foreach (var file in System.IO.Directory.GetFiles(NewFileDic))
            {                
                this.MoveFile(file, FilterDirPath + "/Normal");
                counter++;
            }

            // all styles
            foreach (var subFolder in System.IO.Directory.GetDirectories(NewFileDic))
            {
                var style = this.GetStyleName(subFolder);

                foreach (var file in System.IO.Directory.GetFiles(subFolder))
                {
                    // skip sound placeholder files
                    if (file.Contains(".mp3")) continue;

                    this.MoveFile(file, FilterDirPath + "/" + style);
                    counter++;
                }
            }

            Logger.Log("Copied " + counter + " filter files", 0);
        }

        private string GetStyleName(string subFolder)
        {
            var capsLockName = subFolder.Substring(subFolder.IndexOf("(STYLE) ") + "(STYLE) ".Length);
            var result = "";
            result += capsLockName[0].ToString().ToUpper();
            result += capsLockName.Substring(1).ToLower();
            return result;
        }

        private void MoveFile(string original, string destination)
        {
            var destPath = destination + "/" + original.Split('\\').Last();
            var a = System.IO.File.Exists(original);
            var b = System.IO.File.Exists(destPath);
            if (!a || !b) throw new Exception();
            
            System.IO.File.Copy(original, destPath, true);
        }

        private bool FindNewFilterFiles()
        {
            var cloneFolders = Enumerable.Range(1, 8).Any(x => System.IO.Directory.Exists(NewFileDic + " (" + x + ")"));
            var cloneRars = Enumerable.Range(1, 8).Any(x => System.IO.File.Exists(NewFileDic + " (" + x + ").rar"));

            if (cloneFolders || cloneRars)
            {
                while (Console.ReadLine() != "exit") Console.WriteLine("Error: There is more than one rar/folder present. Please delete any duplicates and restart.");
                throw new Exception("there are duplicate filter folders");
            }

            return System.IO.Directory.Exists(NewFileDic);
        }
    }
}
