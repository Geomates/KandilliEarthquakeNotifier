﻿using KandilliEarthquakeBot.Enums;
using System.Text.RegularExpressions;

namespace KandilliEarthquakeBot.Helpers
{
    public static class CommandHelper
    {
        public static bool TryParseCommand(string chatMessage, out Command command)
        {
            string pattern = @"\/(basla|start|siddet|konum|bitir|stop)";
            command = Command.None;
            var result = Regex.Match(chatMessage, pattern);

            if (result.Success)
            {
                var regexGroup = result.Groups[1];
                switch(regexGroup.Value)
                {
                    case "basla":
                    case "start":
                        command = Command.Start;
                        break;
                    case "bitir":
                    case "stop":
                        command = Command.Stop;
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
