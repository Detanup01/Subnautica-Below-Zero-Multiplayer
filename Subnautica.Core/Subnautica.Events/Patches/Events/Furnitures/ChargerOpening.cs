namespace Subnautica.Events.Patches.Events.Furnitures
{
    using HarmonyLib;
    using Subnautica.API.Features;
    using Subnautica.Events.EventArgs;
    using System;

    [HarmonyPatch(typeof(global::Charger), nameof(global::Charger.OnHandClick))]
    public static class ChargerOpening
    {
        private static bool Prefix(global::Charger __instance)
        {
            if (!Network.IsMultiplayerActive)
            {
                return true;
            }

            if (!__instance.enabled || __instance.sequence.active)
            {
                return false;
            }

            var constructable = __instance.GetComponentInParent<Constructable>();
            if (constructable == null)
            {
                return false;
            }

            if (EventBlocker.IsEventBlocked(constructable.techType))
            {
                return true;
            }

            try
            {
                ChargerOpeningEventArgs args = new ChargerOpeningEventArgs(Network.Identifier.GetIdentityId(constructable.gameObject), constructable.techType);

                Handlers.Furnitures.OnChargerOpening(args);

                return args.IsAllowed;
            }
            catch (Exception e)
            {
                Log.Error($"ChargerOpening.Prefix: {e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}