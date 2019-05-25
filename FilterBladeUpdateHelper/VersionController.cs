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

        private Js_FileUpdater jsFileUpdater;

        public void Run(Js_FileUpdater jsFu)
        {
            this.jsFileUpdater = jsFu;
            OldVersion = this.FindCurrentVersion();
            NewVersion = this.EvaluateUserInput();            
        }

        private string EvaluateUserInput()
        {
            Console.WriteLine("Enter version increment:");
            Console.WriteLine("<Enter> -> dont change version");
            Console.WriteLine("+ -> increase letter: 1.0.0a -> 1.0.0b /// 1.0.0 -> 1.0.0a /// for patches/hotfixes");
            Console.WriteLine("++ -> increase version: 1.0.0 -> 1.0.1 /// for new leagues");
            Console.WriteLine("+++ -> increase main version: 1.0.0 -> 1.1.0 /// for big, rare content updates");
            Console.WriteLine("d <NewVal> -> directly set new version to <NewVal>");
            
            var input = Console.ReadLine();
            var result = OldVersion;
            var versionSplit = OldVersion.Split('.');

            switch (input)
            {
                case "":
                    // dont change -> return current version
                    break;

                case "+": // letter
                    versionSplit[2] = this.IncreaseLetter(versionSplit[2]);
                    result = string.Join(".", versionSplit);
                    break;
                    
                case "++": // index 2
                    versionSplit[2] = this.IncreaseVersion(versionSplit[2]).ToString();
                    result = string.Join(".", versionSplit);
                    break;
                    
                case "+++": // index 1
                    versionSplit[1] = this.IncreaseMainVersion(versionSplit[1]).ToString();
                    versionSplit[2] = "0";
                    result = string.Join(".", versionSplit);
                    break;

                default:
                    if (input == null || input.Length < 7 || input[0] != 'd' || input[1] != ' ') throw new Exception("invalid custom version input: " + input);

                    result = input.Substring(2);
                    break;
            }
            
            Console.WriteLine("Target version was set to: \"" + result + "\". Press <Enter> to confirm.");
            if (Console.ReadLine() != "") throw new Exception("You did not just press enter, you wrote something: Cancelling updating.");
            return result;
        }

        private string IncreaseLetter(string oldVersion)
        {
            // example inputs:     0    1    1b    123x
            // example outputs:    0a   1a   1c    123y
            
            var index = this.GetIndexOfFirstLetter(oldVersion);
            
            if (index == -1) return oldVersion + "a";

            var letter = oldVersion.Substring(index);

            if (letter == "z") throw new Exception("unhandled case: version letter is higher than z");
            if (letter.Length != 1) throw new Exception("unexpected value for version: " + oldVersion);
            
            var newLetter = letter[0];
            newLetter++;
            return oldVersion.Substring(0, index) + newLetter;                
        }

        private int IncreaseVersion(string oldVersion)
        {
            var index = this.GetIndexOfFirstLetter(oldVersion);
            if (index == -1) return int.Parse(oldVersion) + 1;
            return int.Parse(oldVersion.Substring(0, index)) + 1;
        }

        private int IncreaseMainVersion(string oldVersion)
        {
            return int.Parse(oldVersion) + 1;
        }

        private int GetIndexOfFirstLetter(string version)
        {
            for (var i = 0; i < version.Length; i++)
            {
                if (char.IsLetter(version[i])) return i;
            }

            return -1;
        }

        private string FindCurrentVersion()
        {
            return this.jsFileUpdater.FindCurrentCacheBusterVersion();
        }
    }
}
