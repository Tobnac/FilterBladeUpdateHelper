using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpCompress.Archives.Rar;
using SharpCompress.Common;
using SharpCompress.Readers;

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
        private static readonly string[] NewFilterArchiveNames = { "RESULTS", "Filter" };
        public static string NewFilterFolderPath { get; private set; } = FilterBladeUpdateHelper.NewFilterArchiveFolderPath + "FB_UpdateHelper_UnpackedFilters";

        public void Run()
        {
            if (!this.DoNewFilterFilesExist())
            {
                Logger.Log("No new filter files found", 1);
                return;                
            }
            
            Logger.Log("New filter files found", 1);
            this.UpdateFilterData();
            this.InsertNewFilterFiles();
        }

        private void UpdateFilterData()
        {
            var newVersion = this.GetFilterVersion(true);
            var oldVersion = this.GetFilterVersion(false);
            Logger.Log("New filter version: " + newVersion, 0);
            Logger.Log("Old filter version: " + oldVersion, 0);
            if (newVersion == oldVersion) Logger.Log("New and Old filter version is identical", 5);
            else Logger.Log("Updating filter version from " + oldVersion + " to " + newVersion, 5);
            OptionFileVersioner.UpdateFilterData(newVersion, oldVersion);
        }

        private string GetFilterVersion(bool newFiles)
        {
            string filterPath;
            if (newFiles) filterPath = NewFilterFolderPath + "/NeverSink's filter - 1-REGULAR.filter";
            else filterPath = FilterDirPath + "/Normal/NeverSink's filter - 1-REGULAR.filter";
            // # VERSION:		5.73
            
            if (!File.Exists(filterPath)) throw new Exception("filter file not found: " + filterPath);

            const string key = "version:";
            var filterLines = File.ReadAllLines(filterPath)
                .Select(x => x.ToLower())
                .FirstOrDefault(x => x.Contains(key));
            
            if (filterLines == null) throw new Exception("unable to find version in filter file");

            var index = filterLines.IndexOf(key, StringComparison.Ordinal);
            var version = filterLines.Substring(index + key.Length);
            version = version.Replace('\t', ' ').Trim();
            return version;
        }

        private void InsertNewFilterFiles()
        {
            var counter = 0;
            
            // todo: prep for changelog file

            // normal style
            foreach (var file in Directory.GetFiles(NewFilterFolderPath))
            {
                // we only care about filter files here. Ignore any other files like the occasional gitIgnore file
                if (!file.EndsWith(".filter") || file.ToLower().Contains("seed")) continue;
                
                this.MoveFile(file, FilterDirPath + "/Normal");
                counter++;
            }

            // all styles
            foreach (var subFolder in Directory.GetDirectories(NewFilterFolderPath))
            {
                var style = this.GetStyleName(subFolder);
                if (style == null) continue;

                foreach (var file in Directory.GetFiles(subFolder))
                {
                    // skip sound placeholder files
                    if (!file.EndsWith(".filter")) continue;

                    this.MoveFile(file, FilterDirPath + "/" + style);
                    counter++;
                }
            }

            Logger.Log("Copied " + counter + " filter files", 2);
        }

        private string GetStyleName(string subFolder)
        {
            var styleIndex = subFolder.IndexOf("(STYLE) ", StringComparison.Ordinal);
            if (styleIndex == -1) return null;
            var capsLockName = subFolder.Substring(styleIndex + "(STYLE) ".Length);
            var result = "";
            result += capsLockName[0].ToString().ToUpper();
            result += capsLockName.Substring(1).ToLower();
            return result;
        }

        private void MoveFile(string original, string destination)
        {
            if (!Directory.Exists(destination))
            {
                Logger.Log("Created new filter style folder: " + Path.GetFileName(destination) + ". Don't forget to add those files to git!", 3);
                Directory.CreateDirectory(destination);
            }
            
            destination += "/" + Path.GetFileName(original);
            File.Copy(original, destination, true);
        }

        private bool DoNewFilterFilesExist()
        {
            var archiveFiles = Directory.EnumerateFiles(FilterBladeUpdateHelper.NewFilterArchiveFolderPath)
                .Where(IsFilterArchive)
                .OrderByDescending(File.GetCreationTime)
                .ToList();

            if (archiveFiles.Count == 0) return false;
            
            Console.WriteLine(archiveFiles.Count + " possible new filter archives found");
            
            foreach (var archiveFile in archiveFiles)
            {
                var archiveName = Path.GetFileName(archiveFile);
                Console.WriteLine("Filter Archive found: " + archiveName + ". Created on: " + File.GetCreationTime(archiveFile));
                Console.WriteLine("Use this one? Press <Enter> to confirm, write anything to select next archive.");
                if (Console.ReadLine() != "") continue;

                while (Directory.Exists(NewFilterFolderPath))
                {
                    NewFilterFolderPath += "_";
                }
                
                if (!Directory.Exists(NewFilterFolderPath)) Directory.CreateDirectory(NewFilterFolderPath);

                if (archiveFile.EndsWith(".rar"))
                {
                    RarArchive.Open(archiveFile).ExtractAllEntries().WriteAllToDirectory(NewFilterFolderPath, new ExtractionOptions(){ExtractFullPath = true});
                }
                else if (archiveFile.EndsWith(".zip"))
                {
                    System.IO.Compression.ZipFile.ExtractToDirectory(archiveFile, NewFilterFolderPath);
                }
                else throw new Exception("unexpected filter archive format/ending");
                
                return true;
            }

            Console.WriteLine("All filter archives iterated. Do you want to NOT insert any new filter files? Press <Enter> to confirm.");
            if (Console.ReadLine() != "") throw new Exception("unexpected filter selection. Cancelling update");
            return false;

            bool IsFilterArchive(string randomFileName)
            {
                randomFileName = Path.GetFileName(randomFileName);
                randomFileName = randomFileName.ToLower();
                if (!randomFileName.EndsWith(".rar") && !randomFileName.EndsWith(".zip")) return false;
                return NewFilterArchiveNames.Any(s => randomFileName.Contains(s.ToLower()));
            }
        }

        public static void CleanUpFiles()
        {
            if (Directory.Exists(NewFilterFolderPath))
            {
                Directory.Delete(NewFilterFolderPath, true);
            }
        }
    }
}
