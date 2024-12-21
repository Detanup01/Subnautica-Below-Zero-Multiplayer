namespace Subnautica.Events.Patches.Events.World
{
    using HarmonyLib;
    using Subnautica.API.Features;
    using Subnautica.Events.EventArgs;
    using System;

    [HarmonyPatch]
    public static class WeatherProfileChanged
    {
        private static StopwatchItem Timing = new StopwatchItem(1000f);

        private static string LastProfileId { get; set; }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(global::WeatherManager), nameof(global::WeatherManager.Update))]
        private static void WeatherManager_Update()
        {
            if (Network.IsMultiplayerActive && World.IsLoaded && Timing.IsFinished() && global::WeatherManager.main && global::WeatherManager.main.currentWeatherProfile)
            {
                Timing.Restart();

                if (WeatherProfileChanged.LastProfileId != global::WeatherManager.main.currentWeatherProfile.name)
                {
                    WeatherProfileChanged.LastProfileId = global::WeatherManager.main.currentWeatherProfile.name;

                    if (!string.IsNullOrEmpty(WeatherProfileChanged.LastProfileId))
                    {
                        try
                        {
                            WeatherProfileChangedEventArgs args = new WeatherProfileChangedEventArgs(WeatherProfileChanged.LastProfileId, true);

                            Handlers.World.OnWeatherProfileChanged(args);
                        }
                        catch (Exception e)
                        {
                            Log.Error($"WeatherProfileChanged.WeatherManager_Update: {e}\n{e.StackTrace}");
                        }
                    }
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(global::WeatherManager), nameof(global::WeatherManager.ActivateScriptedWeatherEvent), new Type[] { typeof(WeatherEventData), typeof(bool) })]
        private static bool WeatherManager_ActivateScriptedWeatherEvent(WeatherEventData eventData)
        {
            if (Network.IsMultiplayerActive)
            {
                try
                {
                    WeatherProfileChangedEventArgs args = new WeatherProfileChangedEventArgs(eventData.weatherId, false);

                    Handlers.World.OnWeatherProfileChanged(args);

                    return args.IsAllowed;
                }
                catch (Exception e)
                {
                    Log.Error($"WeatherProfileChanged.WeatherManager_ActivateScriptedWeatherEvent: {e}\n{e.StackTrace}");
                }
            }

            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(global::WeatherManager), nameof(global::WeatherManager.Start))]
        private static void WeatherManager_OnDestroy()
        {
            WeatherProfileChanged.LastProfileId = null;
        }
    }
}
