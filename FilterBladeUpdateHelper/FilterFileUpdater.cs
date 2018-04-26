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

        public OptionFileVersioner OptionFileVersioner { get; set; } = new OptionFileVersioner();

        public FilterFileUpdater()
        {
        }

        public void Run()
        {
            if (this.FindNewFilterFiles())
            {
                Logger.Log("New filter files found", 0);
                this.InsertNewFilterFiles();
                this.UpdateFilterData();
            }

            Logger.Log("No new filter files found", 0);
        }

        private void UpdateFilterData()
        {
            string v = this.GetFilterVersion();
            this.OptionFileVersioner.UpdateFilterData(v);
        }

        private string GetFilterVersion()
        {
            throw new NotImplementedException();
        }

        private void InsertNewFilterFiles()
        {
            throw new NotImplementedException();

            //var strictnessNames = new string[] { "REGULAR", "SEMI-STRICT", "STRICT", "VERY-STRICT", "UBER-STRICT", "UBER-PLUS-STRICT" };

            //for (int i = 0; i < strictnessNames.Length; i++)
            //{
            //    this.FilterFileNames.Add("NeverSink's filter - " + i + " - " + strictnessNames[i] + ".filter");
            //}

            //todo: check if i can just get all files and directories within a dictionary
        }

        private bool FindNewFilterFiles()
        {
            throw new NotImplementedException();

            // todo: how to provide the info for this?
            // -> provide path -> annoying
            // -> always the same place+name -> dumb because NS sends 4 versions all the time anyway
        }
    }
}
