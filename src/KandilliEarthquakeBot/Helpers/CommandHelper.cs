using KandilliEarthquakeBot.Enums;
using System.Text.RegularExpressions;

namespace KandilliEarthquakeBot.Helpers
{
    public static class CommandHelper
    {
        public static bool TryParseCommand(string chatMessage, out Command command)
        {
            string pattern = @"\/(basla|siddet|konum)";
            command = Command.None;
            var result = Regex.Match(chatMessage, pattern);

            if (result.Success)
            {
                var regexGroup = result.Groups[1];
                switch(regexGroup.Value)
                {
                    case "basla":
                        command = Command.Start;
                        break;
                    case "siddet":
                        command = Command.Magnitude;
                        break;
                    case "konum":
                        command = Command.Location;
                        break;
                }
                return true;
            }

            return false;
        }
    }
}
