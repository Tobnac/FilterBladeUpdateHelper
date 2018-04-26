using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilterBladeUpdateHelper
{
    public class FilterBladeUpdateHelper
    {
        const string FbDirPath = "C:\\Users\\Tobnac\\WebstormProjects\fb";

        public VersionController VersionController { get; set; } = new VersionController();
        public FilterFileUpdater FilterFileUpdater { get; set; } = new FilterFileUpdater();
        public JS_FileUpdater JS_FileUpdater { get; set; } = new JS_FileUpdater();
        public MinimizerHandler MinimizerHandler { get; set; } = new MinimizerHandler();

        public FilterBladeUpdateHelper()
        {

        }

        private void RunAll()
        {
            // get new version

            // check for new filter files
            // insert new filter files
            // update filter date + version in optionFile

            // update cacheBuster version in FB.js

            // update optionFile version

            // update HTML version + date
            // migrate devHTML into releaseHTML file + also update version there
            // --> keep new startUp page in mind -> that one might need updating too!

            // start minimizer
        }
    }
}
