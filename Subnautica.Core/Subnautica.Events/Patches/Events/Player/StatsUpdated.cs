namespace Subnautica.Events.Patches.Events.Player
{
    using HarmonyLib;
    using Subnautica.API.Features;
    using Subnautica.Events.EventArgs;
    using System;

    [HarmonyPatch(typeof(global::Player), nameof(global::Player.FixedUpdate))]
    public static class StatsUpdated
    {
        private static readonly StopwatchItem Timing = new StopwatchItem(BroadcastInterval.PlayerStatsUpdated);

        private static void Postfix()
        {
            if (Network.IsMultiplayerActive && Timing.IsFinished())
            {
                Timing.Restart();

                try
                {
                    Survival survival = global::Player.main.GetComponent<Survival>();

                    PlayerStatsUpdatedEventArgs args = new PlayerStatsUpdatedEventArgs(global::Player.main.liveMixin.health, survival.food, survival.water);

                    Handlers.Player.OnStatsUpdated(args);
                }
                catch (Exception e)
                {
                    Log.Error($"StatsUpdated.Postfix: {e}\n{e.StackTrace}");
                }
            }
        }
    }
}

