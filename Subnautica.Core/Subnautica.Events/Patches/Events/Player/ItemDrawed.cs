namespace Subnautica.Events.Patches.Events.Player
{
    using HarmonyLib;
    using Subnautica.API.Features;
    using Subnautica.Events.EventArgs;
    using System;

    [HarmonyPatch(typeof(global::PlayerTool), nameof(global::PlayerTool.OnDraw))]
    public static class ItemDrawed
    {
        private static void Prefix(global::PlayerTool __instance)
        {
            if (Network.IsMultiplayerActive)
            {
                try
                {
                    ItemDrawedEventArgs args = new ItemDrawedEventArgs(__instance.pickupable != null ? __instance.pickupable.GetTechType() : TechType.None, __instance.hasFirstUseAnimation && __instance.pickupable && __instance.ShouldPlayFirstUseAnimation());

                    Handlers.Player.OnItemDrawed(args);
                }
                catch (Exception e)
                {
                    Log.Error($"ItemOnDrawed.Prefix: {e}\n{e.StackTrace}");
                }
            }
        }
    }
}
