namespace Subnautica.Events.Patches.Events.Furnitures
{
    using HarmonyLib;
    using Subnautica.API.Extensions;
    using Subnautica.API.Features;
    using Subnautica.Events.EventArgs;
    using System;

    [HarmonyPatch(typeof(global::Snowman), nameof(global::Snowman.OnHandClick))]
    public static class SnowmanDestroying
    {
        private static bool Prefix(global::Snowman __instance)
        {
            if (!Network.IsMultiplayerActive)
            {
                return true;
            }

            if (!__instance.enabled)
            {
                return false;
            }

            if (EventBlocker.IsEventBlocked(TechType.Snowman))
            {
                return true;
            }

            var uniqueId = GetUniqueId(__instance);
            if (uniqueId.IsNull())
            {
                return false;
            }

            try
            {
                SnowmanDestroyingEventArgs args = new SnowmanDestroyingEventArgs(uniqueId, !__instance.GetComponentInParent<Constructable>());

                Handlers.Furnitures.OnSnowmanDestroying(args);

                return args.IsAllowed;
            }
            catch (Exception e)
            {
                Log.Error($"Building.SnowmanDestroying: {e}\n{e.StackTrace}");
                return true;
            }
        }

        private static string GetUniqueId(global::Snowman __instance)
        {
            var constructable = __instance.GetComponentInParent<Constructable>();
            if (constructable)
            {
                return constructable.gameObject.GetIdentityId();
            }

            return __instance.gameObject.GetIdentityId();
        }
    }
}
