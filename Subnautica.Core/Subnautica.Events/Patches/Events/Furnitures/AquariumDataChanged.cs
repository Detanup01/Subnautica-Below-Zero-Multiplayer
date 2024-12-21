namespace Subnautica.Events.Patches.Events.Furnitures
{
    using HarmonyLib;
    using Subnautica.API.Features;
    using Subnautica.Events.EventArgs;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public static class AquariumDataChangedShared
    {
        private static bool IsTriggered { get; set; } = false;

        public static void TriggerEvent(global::Aquarium __instance)
        {
            if (Network.IsMultiplayerActive && !IsTriggered && !EventBlocker.IsEventBlocked(TechType.Aquarium))
            {
                UWE.CoroutineHost.StartCoroutine(TriggerEventCallback(__instance));
            }
        }

        private static IEnumerator TriggerEventCallback(global::Aquarium __instance)
        {
            IsTriggered = true;

            yield return new WaitForSecondsRealtime(0.2f);

            IsTriggered = false;

            var constructable = __instance.GetComponentInParent<Constructable>();
            if (constructable != null)
            {
                try
                {
                    AquariumDataChangedEventArgs args = new AquariumDataChangedEventArgs(Network.Identifier.GetIdentityId(constructable.gameObject), GetFishTypes(__instance));

                    Handlers.Furnitures.OnAquariumDataChanged(args);
                }
                catch (Exception e)
                {
                    Log.Error($"AquariumDataChangedShared.TriggerEventCallback: {e}\n{e.StackTrace}");
                }
            }
        }

        private static List<TechType> GetFishTypes(global::Aquarium __instance)
        {
            return __instance.tracks.Where(q => q.fishType != TechType.None).Select(q => q.fishType).ToList();
        }
    }


    [HarmonyPatch(typeof(global::Aquarium), nameof(global::Aquarium.AddItem))]
    public static class AquariumDataChangedAdded
    {
        private static void Postfix(global::Aquarium __instance)
        {
            AquariumDataChangedShared.TriggerEvent(__instance);
        }
    }

    [HarmonyPatch(typeof(global::Aquarium), nameof(global::Aquarium.RemoveItem))]
    public static class AquariumDataChangedRemoved
    {
        private static void Postfix(global::Aquarium __instance)
        {
            AquariumDataChangedShared.TriggerEvent(__instance);
        }
    }
}