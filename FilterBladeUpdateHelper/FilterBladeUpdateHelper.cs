using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilterBladeUpdateHelper
{
    public class FilterBladeUpdateHelper
    {
        // CONFIG SETTING FOR USER
        // These strings are the only thing you need to change when you want to use this Tool on your PC.
        // All other configs should work on all systems, as long as we're using the same FB :P
        public const string FbDirPath = @"C:\Users\Tobnac\WebstormProjects\fb";
        public const string NewFilterArchiveFolderPath = @"C:\Users\Tobnac\Downloads\";

        public VersionController VersionController { get; set; } = new VersionController();
        public FilterFileUpdater FilterFileUpdater { get; set; } = new FilterFileUpdater();
        public Js_FileUpdater JsFileUpdater { get; set; } = new Js_FileUpdater();
        public Html_Migrater HtmlMigrater { get; set; }
        public MinimizerHandler MinimizerHandler { get; set; } = new MinimizerHandler();

        public FilterBladeUpdateHelper()
        {
            this.HtmlMigrater = new Html_Migrater("/indexdev.html", "/index.html");
            this.VerifyDirectory();
            this.RunAll();
            FilterFileUpdater.CleanUpFiles();
            Console.WriteLine("\n\nFilterBlade successfully updated!");
            Console.Read();
        }

        private void RunAll()
        {
            // get current version + target version with user input
            this.VersionController.Run(this.JsFileUpdater);

            // check for new filter files
            // insert new filter files
            // update filter date + version in optionFile
            this.FilterFileUpdater.Run();

            // update cacheBuster version in FB.js
            this.JsFileUpdater.Run();

            // update optionFile version
            OptionFileVersioner.UpdateCustomizerVersion();

            // update HTML version + date
            // migrate devHTML into releaseHTML file
            // also update version in releaseHTML file
            // --> keep new startUp page in mind -> that one might need updating too!
            this.HtmlMigrater.Run();

            // start minimizer
            this.MinimizerHandler.Run();
        }

        private void VerifyDirectory()
        {
            // if the given dictionary does not exist -> exit with Exception
            if (!System.IO.Directory.Exists(FbDirPath)) throw new Exception("FilterBlade directory does not exist: " + FbDirPath);
            Logger.Log("FilterBlade directory found", 0);
        }
    }
}
