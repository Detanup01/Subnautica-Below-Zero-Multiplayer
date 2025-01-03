namespace Subnautica.Events.Patches.Events.Building
{
    using HarmonyLib;
    using Subnautica.API.Features;
    using Subnautica.Events.EventArgs;
    using System;

    [HarmonyPatch(typeof(Builder), nameof(Builder.Update))]
    public class ConstructionGhostMoved
    {
        private static StopwatchItem StopwatchItem = new StopwatchItem(BroadcastInterval.ConstructingGhostMoved);

        private static void Postfix()
        {
            if (Network.IsMultiplayerActive)
            {
                if (StopwatchItem.IsFinished())
                {
                    StopwatchItem.Restart();

                    if (Builder.prefab == null || Builder.ghostModel == null)
                    {
                        return;
                    }

                    try
                    {
                        ConstructionGhostMovedEventArgs args = new ConstructionGhostMovedEventArgs(
                            Builder.ghostModel,
                            Builder.lastTechType,
                            Builder.GetAimTransform(),
                            Builder.canPlace,
                            Builder.lastRotation
                        );

                        Handlers.Building.OnConstructingGhostMoved(args);
                    }
                    catch (Exception e)
                    {
                        Log.Error($"ConstructingGhostMoved.Postfix: {e}\n{e.StackTrace}");
                    }
                }
            }
        }
    }
}
