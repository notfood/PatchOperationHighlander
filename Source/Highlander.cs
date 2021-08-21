using System.Collections.Generic;
using System.Linq;
using Verse;

namespace PatchOperation
{
    public static class Highlander
    {
        public static bool IsAnyActiveByID(IEnumerable<string> mods)
        {
            return mods.Any(id => ModsConfig.ActiveModsInLoadOrder.Any(mod => mod.SamePackageId(id, true)));
        }

        public static bool IsAllActiveByID(IEnumerable<string> mods)
        {
            return mods.All(id => ModsConfig.ActiveModsInLoadOrder.Any(mod => mod.SamePackageId(id, true)));
        }

        public static bool IsActiveByID(List<string> mods, bool all = false)
		{
            return all ? Highlander.IsAllActiveByID(mods) : Highlander.IsAnyActiveByID(mods);
		}

        public static void Log(string message, LogLevel level = LogLevel.Warning)
        {
            switch(level)
            {
                case LogLevel.Message:
                    Verse.Log.Message(message);
                    break;
                
                default:
                case LogLevel.Warning:
                    Verse.Log.Warning(message);
                    break;
                
                case LogLevel.Error:
                    Verse.Log.Error(message);
                    break;
            }
        }

        public enum LogLevel
        {
            Message, Warning, Error
        }
    }
}