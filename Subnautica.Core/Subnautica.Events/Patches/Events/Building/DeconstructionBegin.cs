namespace Subnautica.Events.Patches.Events.Building
{
    using HarmonyLib;

    using Subnautica.API.Extensions;
    using Subnautica.API.Features;
    using Subnautica.Events.EventArgs;

    using System;

    [HarmonyPatch(typeof(global::BaseDeconstructable), nameof(global::BaseDeconstructable.Deconstruct))]
    public static class DeconstructionBegin
    {
        private static bool Prefix(global::BaseDeconstructable __instance)
        {
            if (!Network.IsMultiplayerActive)
            {
                return true;
            }

            if (__instance.GetComponentInParent<global::Base>() == null)
            {
                return false;
            }

            var uniqueId = DeconstructionBegin.GetUniqueId(__instance);
            if (uniqueId.IsNull())
            {
                return false;
            }

            if (DeconstructionBegin.IsPlayerInWaterPark(__instance))
            {
                ErrorMessage.AddMessage(Language.main.Get("PlayerObstacle"));
                return false;
            }

            try
            {
                DeconstructionBeginEventArgs args = new DeconstructionBeginEventArgs(uniqueId, __instance, __instance.recipe);

                Handlers.Building.OnDeconstructionBegin(args);

                return args.IsAllowed;
            }
            catch (Exception e)
            {
                Log.Error($"DeconstructionBegin.Prefix: {e}\n{e.StackTrace}");
                return true;
            }
        }

        private static bool IsPlayerInWaterPark(BaseDeconstructable baseDeconstructable)
        {
            bool flag = baseDeconstructable.recipe != TechType.BaseWaterPark;
            bool flag2;
            if (flag)
            {
                flag2 = false;
            }
            else
            {
                WaterPark baseWaterPark = baseDeconstructable.GetBaseWaterPark();
                bool flag3 = baseWaterPark == null;
                if (flag3)
                {
                    flag2 = false;
                }
                else
                {
                    foreach (ZeroPlayer zeroPlayer in ZeroPlayer.GetPlayers())
                    {
                        bool flag4 = baseWaterPark.IsPointInside(zeroPlayer.PlayerModel.transform.position);
                        if (flag4)
                        {
                            return true;
                        }
                    }
                    flag2 = false;
                }
            }
            return flag2;
        }

        private static string GetUniqueId(global::BaseDeconstructable baseDeconstructable)
        {
            var uniqueId = baseDeconstructable.gameObject.GetIdentityId();
            if (uniqueId.IsNotNull())
            {
                return uniqueId;
            }

            if (baseDeconstructable.recipe == TechType.BaseLadder)
            {
                try
                {
                    var baseComponent = baseDeconstructable.GetComponentInParent<global::Base>();
                    if (baseComponent && baseDeconstructable.face.HasValue)
                    {
                        baseComponent.GetLadderExitCell(baseDeconstructable.face.Value.cell, baseDeconstructable.face.Value.direction, out var exitFace);

                        var aboveFace = new global::Base.Face(exitFace, global::Base.ReverseDirection(baseDeconstructable.face.Value.direction));
                        var aboveBound = baseDeconstructable.bounds.Union(exitFace);

                        if (baseDeconstructable.name.Contains("BaseRoomLadderBottom") || baseDeconstructable.name.Contains("BaseLargeRoomLadderBottom"))
                        {
                            aboveBound.maxs.y = aboveBound.mins.y;
                        }

                        foreach (var item in baseDeconstructable.gameObject.transform.parent.gameObject.transform.parent.gameObject.GetComponentsInChildren<global::BaseDeconstructable>())
                        {
                            if (item.name.Contains("LadderTop"))
                            {
                                if (aboveBound == item.bounds && aboveFace == item.face && item.faceType == baseDeconstructable.faceType)
                                {
                                    return item.gameObject.GetIdentityId();
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {

                }
            }

            return null;
        }
    }
}
