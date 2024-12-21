namespace Subnautica.Client.Synchronizations.Processors.Vehicle
{
    using Subnautica.API.Enums;
    using Subnautica.API.Features;
    using Subnautica.Client.Abstracts;
    using Subnautica.Client.Core;
    using Subnautica.Events.EventArgs;
    using Subnautica.Network.Models.Core;
    using Subnautica.Network.Models.WorldEntity.DynamicEntityComponents.Shared;

    using ServerModel = Subnautica.Network.Models.Server;

    public class SeaTruckStorageModuleProcessor : NormalProcessor
    {
        public override bool OnDataReceived(NetworkPacket networkPacket)
        {
            var packet = networkPacket.GetPacket<ServerModel.SeaTruckStorageModuleArgs>();
            if (string.IsNullOrEmpty(packet.UniqueId))
            {
                return false;
            }

            if (packet.IsSignProcess)
            {
                if (!packet.IsSignSelect)
                {
                    using (EventBlocker.Create(TechType.Sign))
                    {
                        var signInput = Network.Identifier.GetComponentByGameObject<global::uGUI_SignInput>(ZeroGame.GetSeaTruckColoredLabelUniqueId(packet.UniqueId));
                        if (signInput)
                        {
                            signInput.text = packet.SignText;
                            signInput.colorIndex = packet.SignColorIndex;
                        }
                    }
                }
            }
            else
            {
                if (packet.IsAdded)
                {
                    Network.Storage.AddItemToStorage(packet.UniqueId, packet.GetPacketOwnerId(), packet.WorldPickupItem);
                }
                else
                {
                    Network.Storage.AddItemToInventory(packet.GetPacketOwnerId(), packet.WorldPickupItem);
                }
            }

            return true;
        }

        public static void OnStorageItemAdding(StorageItemAddingEventArgs ev)
        {
            if (ev.TechType == TechType.SeaTruckStorageModule)
            {
                ev.IsAllowed = false;

                SeaTruckStorageModuleProcessor.SendPacketToServer(ev.UniqueId, pickupItem: WorldPickupItem.Create(ev.Item, PickupSourceType.PlayerInventory), isAdded: true);
            }
        }

        public static void OnStorageItemRemoving(StorageItemRemovingEventArgs ev)
        {
            if (ev.TechType == TechType.SeaTruckStorageModule)
            {
                ev.IsAllowed = false;

                SeaTruckStorageModuleProcessor.SendPacketToServer(ev.UniqueId, pickupItem: WorldPickupItem.Create(ev.Item, PickupSourceType.StorageContainer));
            }
        }

        public static void OnSignSelect(SignSelectEventArgs ev)
        {
            if (ev.TechType == TechType.SeaTruckStorageModule)
            {
                if (Interact.IsBlocked(ev.UniqueId))
                {
                    ev.IsAllowed = false;
                }
                else
                {
                    SeaTruckStorageModuleProcessor.SendPacketToServer(ev.UniqueId, isSignProcess: true, isSignSelect: true);
                }
            }
        }

        public static void OnSignDataChanged(SignDataChangedEventArgs ev)
        {
            if (ev.TechType == TechType.SeaTruckStorageModule)
            {
                SeaTruckStorageModuleProcessor.SendPacketToServer(ev.UniqueId, isSignProcess: true, signText: ev.Text, signColorIndex: ev.ColorIndex);
            }
        }

        public static void SendPacketToServer(string uniqueId, bool isSignProcess = false, bool isSignSelect = false, string signText = null, int signColorIndex = -1, WorldPickupItem pickupItem = null, bool isAdded = false)
        {
            ServerModel.SeaTruckStorageModuleArgs request = new ServerModel.SeaTruckStorageModuleArgs()
            {
                UniqueId = uniqueId,
                IsSignProcess = isSignProcess,
                IsSignSelect = isSignSelect,
                IsAdded = isAdded,
                SignText = signText,
                SignColorIndex = signColorIndex,
                WorldPickupItem = pickupItem,

            };

            NetworkClient.SendPacket(request);
        }
    }
}
