namespace Subnautica.Events.Patches.Fixes.Creatures
{
    using HarmonyLib;
    using Subnautica.API.Extensions;
    using Subnautica.API.Features;

    [HarmonyPatch]
    public class CreatureFrozenMixin
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(global::FrozenMixin), nameof(global::FrozenMixin.Freeze))]
        private static void Freeze(global::FrozenMixin __instance)
        {
            if (Network.IsMultiplayerActive)
            {
                ToggleKinematic(__instance);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(global::FrozenMixin), nameof(global::FrozenMixin.Unfreeze))]
        private static void Unfreeze(global::FrozenMixin __instance)
        {
            if (Network.IsMultiplayerActive)
            {
                ToggleKinematic(__instance);
            }
        }

        private static bool ToggleKinematic(global::FrozenMixin __instance)
        {
            if (!__instance.gameObject.GetTechType().IsSynchronizedCreature())
            {
                return false;
            }

            if (Network.Creatures.IsMine(__instance.gameObject))
            {
                return false;
            }

            __instance.rb.SetKinematic();
            return true;
        }
    }
}
