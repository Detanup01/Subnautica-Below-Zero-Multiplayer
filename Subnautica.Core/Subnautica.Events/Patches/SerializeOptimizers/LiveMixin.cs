namespace Subnautica.Events.Patches.SerializeOptimizers
{
    using HarmonyLib;

    using Subnautica.API.Features;

    [HarmonyPatch(typeof(ProtobufSerializerPrecompiled), nameof(ProtobufSerializerPrecompiled.Serialize729882159))]
    public class LiveMixin
    {
        private static bool Prefix()
        {
            return !Network.IsMultiplayerActive;
        }
    }
}
