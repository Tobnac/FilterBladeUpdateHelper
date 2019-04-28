using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilterBladeUpdateHelper
{
    public static class Helper
    {
        public static string GetCurrentDate()
        {
            var dt = DateTime.Today;

            string suffix;

            if (new[] { 11, 12, 13 }.Contains(dt.Day))
            {
                suffix = "th";
            }
            else if (dt.Day % 10 == 1)
            {
                suffix = "st";
            }
            else if (dt.Day % 10 == 2)
            {
                suffix = "nd";
            }
            else if (dt.Day % 10 == 3)
            {
                suffix = "rd";
            }
            else
            {
                suffix = "th";
            }

            return string.Format("{0:MMMM} {1}{2}", dt, dt.Day, suffix);
        }

        public static string GetFileNameFromPath(string path)
        {
            var index = path.LastIndexOf("/");
            var otherIndex = path.LastIndexOf("\\");
            if (otherIndex > index) index = otherIndex;
            index++;

            return path.Substring(index);
        }
    }
}
