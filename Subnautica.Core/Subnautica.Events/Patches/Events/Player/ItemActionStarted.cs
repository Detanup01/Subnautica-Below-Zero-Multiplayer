namespace Subnautica.Events.Patches.Events.Player
{
    using HarmonyLib;
    using Subnautica.API.Features;
    using Subnautica.Events.EventArgs;
    using System;

    [HarmonyPatch(typeof(global::PlayerTool), nameof(global::PlayerTool.OnToolActionStart))]
    public static class ItemActionStarted
    {
        private static void Prefix(global::PlayerTool __instance)
        {
            if (Network.IsMultiplayerActive)
            {
                try
                {
                    ItemActionStartedEventArgs args = new ItemActionStartedEventArgs(__instance.pickupable != null ? __instance.pickupable.GetTechType() : TechType.None, __instance.firstUseAnimationStarted);

                    Handlers.Player.OnItemActionStarted(args);
                }
                catch (Exception e)
                {
                    Log.Error($"ItemActionStarted.Prefix: {e}\n{e.StackTrace}");
                }
            }
        }
    }
}
