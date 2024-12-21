namespace Subnautica.Events.Patches.Fixes.Game
{
    using HarmonyLib;
    using System;

    [HarmonyPatch(typeof(PlatformServicesEpic), nameof(PlatformServicesEpic.GetUserName))]
    public class Platform
    {
        private static bool Prefix(ref string __result)
        {
            __result = Platform.GetUserName();
            return false;
        }

        private static string GetUserName()
        {
            foreach (var command in Environment.GetCommandLineArgs())
            {
                if (command.Contains("username"))
                {
                    var username = command.Split('=');
                    if (username.Length < 1)
                    {
                        return null;
                    }

                    return username[1].Replace("\"", "").Trim();
                }
            }

            return null;
        }
    }
}
