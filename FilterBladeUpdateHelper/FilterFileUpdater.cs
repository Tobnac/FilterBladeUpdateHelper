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
        public OptionFileVersioner OptionFileVersioner { get; set; } = new OptionFileVersioner();

        public void Run()
        {
            if (this.FindNewFilterFiles())
            {
                Logger.Log("New filter files found", 0);
                this.InsertNewFilterFiles();
                this.OptionFileVersioner.UpdateFilterData();
            }

            Logger.Log("No new filter files found", 0);
        }

        private void InsertNewFilterFiles()
        {
            throw new NotImplementedException();
        }

        private bool FindNewFilterFiles()
        {
            throw new NotImplementedException();
        }
    }
}
