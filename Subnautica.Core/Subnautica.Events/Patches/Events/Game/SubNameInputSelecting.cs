namespace Subnautica.Events.Patches.Events.Game
{
    using HarmonyLib;
    using Subnautica.API.Features;
    using Subnautica.Events.EventArgs;
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;

    [HarmonyPatch(typeof(global::FPSInputModule), nameof(global::FPSInputModule.ProcessMousePress))]
    public class SubNameInputSelecting
    {
        private static bool Prefix(global::FPSInputModule __instance, PointerInputModule.MouseButtonEventData data)
        {
            if (!Network.IsMultiplayerActive || !data.PressedThisFrame() || data.buttonData.pointerCurrentRaycast.gameObject == null || data.buttonData.button != PointerEventData.InputButton.Left)
            {
                return true;
            }

            var subNameInput = data.buttonData.pointerCurrentRaycast.gameObject.GetComponentInParent<SubNameInput>();
            if (subNameInput == null)
            {
                return true;
            }

            var detail = SubnameInputDetail.GetInformation(subNameInput.gameObject);
            if (detail.TechType == TechType.None)
            {
                return false;
            }

            try
            {
                SubNameInputSelectingEventArgs args = new SubNameInputSelectingEventArgs(detail.UniqueId, detail.TechType);

                Handlers.Game.OnSubNameInputSelecting(args);

                return args.IsAllowed;
            }
            catch (Exception e)
            {
                Log.Error($"SubNameInputSelecting.Prefix: {e}\n{e.StackTrace}");
                return true;
            }
        }
    }

    public class SubnameInputDetail
    {
        public string UniqueId { get; private set; }

        public TechType TechType { get; private set; }

        public static SubnameInputDetail GetInformation(GameObject gameObject)
        {
            var response = new SubnameInputDetail();

            var constructable = gameObject.GetComponentInParent<Constructable>();
            if (constructable)
            {
                response.UniqueId = Network.Identifier.GetIdentityId(constructable.gameObject, false);
                response.TechType = constructable.techType;
            }
            else
            {
                var baseDeconstructable = gameObject.GetComponentInParent<BaseDeconstructable>();
                if (baseDeconstructable)
                {
                    response.UniqueId = Network.Identifier.GetIdentityId(baseDeconstructable.gameObject, false);
                    response.TechType = baseDeconstructable.recipe;
                }
            }

            if (response.TechType == TechType.BaseUpgradeConsole)
            {
                foreach (var item in gameObject.transform.parent.transform.parent.gameObject.GetComponentsInChildren<BaseDeconstructable>())
                {
                    if (item.recipe == TechType.BaseMoonpool)
                    {
                        response.UniqueId = Network.Identifier.GetIdentityId(item.gameObject, false);
                        response.TechType = item.recipe;
                        break;
                    }
                }
            }

            return response;
        }
    }
}
