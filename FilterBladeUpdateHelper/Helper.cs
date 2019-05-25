﻿using System;
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
            else switch (dt.Day % 10)
            {
                case 1:
                    suffix = "st";
                    break;
                case 2:
                    suffix = "nd";
                    break;
                case 3:
                    suffix = "rd";
                    break;
                default:
                    suffix = "th";
                    break;
            }

            return $"{dt:MMMM} {dt.Day}{suffix}";
        }

        public static string GetFileNameFromPath(string path)
        {
            var index = path.LastIndexOf("/", StringComparison.Ordinal);
            var otherIndex = path.LastIndexOf("\\", StringComparison.Ordinal);
            if (otherIndex > index) index = otherIndex;
            index++;

            return path.Substring(index);
        }
    }
}
