using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilterBladeUpdateHelper
{
    public class FilterBladeUpdateHelper
    {
        public const string FbDirPath = "C:\\Users\\Tobnac\\WebstormProjects\fb";

        public VersionController VersionController { get; set; } = new VersionController();
        public FilterFileUpdater FilterFileUpdater { get; set; } = new FilterFileUpdater();
        public JS_FileUpdater JS_FileUpdater { get; set; } = new JS_FileUpdater();
        public HTML_Migrater HTML_Migrater { get; set; }
        public MinimizerHandler MinimizerHandler { get; set; } = new MinimizerHandler();

        public FilterBladeUpdateHelper()
        {
            this.VerifyDirectory();
            this.RunAll();
        }

        private void RunAll()
        {
            // get current version + target version with user input
            this.VersionController.Run();

            // check for new filter files
            // insert new filter files
            // update filter date + version in optionFile
            this.FilterFileUpdater.Run();

            // update cacheBuster version in FB.js
            this.JS_FileUpdater.Run();

            // update optionFile version
            this.FilterFileUpdater.OptionFileVersioner.UpdateCustomizerVersion();

            // update HTML version + date
            // migrate devHTML into releaseHTML file
            // also update version in releaseHTML file
            // --> keep new startUp page in mind -> that one might need updating too!
            this.HTML_Migrater.Run();

            // start minimizer
            this.MinimizerHandler.Run();
        }

        private void VerifyDirectory()
        {
            // if the given dictionary does not exist -> exit with Exception
            throw new NotImplementedException();
        }
    }
}
