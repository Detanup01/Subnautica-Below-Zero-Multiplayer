namespace Subnautica.Events.Patches.Events.Furnitures
{
    using HarmonyLib;
    using Subnautica.API.Features;
    using Subnautica.Events.EventArgs;
    using System;

    [HarmonyPatch(typeof(global::TechLight), nameof(global::TechLight.Start))]
    public static class TechLightInitialized
    {
        private static void Postfix(global::TechLight __instance)
        {
            if (Network.IsMultiplayerActive)
            {
                try
                {
                    TechLightInitializedEventArgs args = new TechLightInitializedEventArgs(Network.Identifier.GetIdentityId(__instance.gameObject));

                    Handlers.Furnitures.OnTechLightInitialized(args);
                }
                catch (Exception e)
                {
                    Log.Error($"TechLightInitialized.Postfix: {e}\n{e.StackTrace}");
                }
            }
        }
    }
}
