namespace Subnautica.Events.Patches.Events.Furnitures
{
    using HarmonyLib;

    using Subnautica.API.Features;

    [HarmonyPatch(typeof(global::HoverpadBuilderTooltip), nameof(global::HoverpadBuilderTooltip.GetTooltip))]
    public static class HoverpadTooltipChanged
    {
        private static void Prefix(global::GUI_HoverpadTerminal __instance)
        {
            if (Network.IsMultiplayerActive)
            {

            }
        }
    }
}
