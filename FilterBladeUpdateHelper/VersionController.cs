using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilterBladeUpdateHelper
{
    /// <summary>
    /// -> read current FB version
    /// -> get user input for next version:
    /// --> increase letter
    /// --> increase version
    /// --> increase main version
    /// --> manual direct input
    /// </summary>
    public class VersionController
    {
        public static string NewVersion { get; private set; }
        public static string OldVersion { get; private set; }

        private JS_FileUpdater jS_FileUpdater;

        public void Run(JS_FileUpdater jsFu)
        {
            this.jS_FileUpdater = jsFu;
            OldVersion = this.FindCurrentVersion();
            NewVersion = this.EvaluateUserInput();            
        }

        private string EvaluateUserInput()
        {
            Console.WriteLine("Enter version increment:");
            Console.WriteLine("<Enter> -> dont change version");
            Console.WriteLine("+ -> increase letter");
            Console.WriteLine("++ -> increase version");
            Console.WriteLine("+++ -> increase main version");
            Console.WriteLine("d <NV> -> directly set new version to <NV>");
            var input = Console.ReadLine();
            var result = OldVersion;
            var versionSplit = OldVersion.Split('.');
            if (versionSplit.Length != 3) throw new Exception();

            switch (input)
            {
                case "":
                    break;

                case "+":
                    versionSplit[2] = this.IncreaseLetter(versionSplit[2]);
                    result = String.Join(".", versionSplit);
                    break;
                    
                case "++":
                    versionSplit[2] = this.IncreaseVersion(versionSplit[2]).ToString();
                    result = String.Join(".", versionSplit);
                    break;
                    
                case "+++":
                    versionSplit[1] = this.IncreaseMainVersion(versionSplit[1]).ToString();
                    versionSplit[2] = "0";
                    result = String.Join(".", versionSplit);
                    break;

                default:
                    if (input[0] != 'd' || input[1] != ' ') throw new Exception();

                    result = input.Substring(2);
                    break;
            }

            Console.WriteLine("Target version was set to: \"" + result + "\". Press <Enter> to confirm.");
            Console.ReadLine();
            return result;
        }

        private string IncreaseLetter(string oldVersion)
        {
            var index = this.FindFirstLetter(oldVersion);

            if (index == -1) return oldVersion.Substring(0, index) + "a";

            var letter = oldVersion.Substring(index);

            if (letter == "z") throw new Exception();
            if (letter.Length != 1) throw new Exception();
            
            char newLetter = letter[0];
            newLetter++;
            return oldVersion.Substring(0, index) + newLetter;                
        }

        private int IncreaseVersion(string oldVersion)
        {
            var index = this.FindFirstLetter(oldVersion);
            if (index == -1) return int.Parse(oldVersion) + 1;
            return int.Parse(oldVersion.Substring(0, index)) + 1;
        }

        private int IncreaseMainVersion(string oldVersion)
        {
            return int.Parse(oldVersion) + 1;
        }

        private int FindFirstLetter(string version)
        {
            int index = -1;
            for (int i = 0; i < version.Length; i++)
            {
                if (Char.IsDigit(version[i])) continue;
                index = i;
                break;
            }

            return index;
        }

        private string FindCurrentVersion()
        {
            return this.jS_FileUpdater.FindCurrentCacheBusterVersion();
        }
    }
}
