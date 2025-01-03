namespace Subnautica.Client.Synchronizations.Processors.Metadata
{
    using Subnautica.API.Enums;
    using Subnautica.API.Features;
    using Subnautica.API.Features.Helper;
    using Subnautica.Client.Abstracts.Processors;
    using Subnautica.Client.Core;
    using Subnautica.Events.EventArgs;
    using Subnautica.Network.Models.Server;
    using Subnautica.Network.Models.Storage.World.Childrens;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using Metadata = Subnautica.Network.Models.Metadata;
    using ServerModel = Subnautica.Network.Models.Server;

    public class ChargerProcessor : MetadataProcessor
    {
        public override bool OnDataReceived(string uniqueId, TechType techType, MetadataComponentArgs packet, bool isSilence)
        {
            var component = packet.Component.GetComponent<Metadata.Charger>();
            if (component == null)
            {
                return false;
            }

            var gameObject = Network.Identifier.GetComponentByGameObject<global::Charger>(uniqueId);
            if (gameObject == null)
            {
                return false;
            }

            if (isSilence)
            {
                using (EventBlocker.Create(ProcessType.MetadataRequest))
                {
                    gameObject.equipment.ClearItems();
                }

                gameObject.opened = component.Items != null && component.Items.Where(q => q.IsActive).Any();
                gameObject.animator.SetBool(gameObject.animParamOpen, gameObject.opened);
                gameObject.animator.Play(gameObject.opened ? gameObject.animatorOpenedStateName : gameObject.animatorClosedStateName);
                gameObject.ToggleUI(gameObject.opened);
            }

            if (component.IsClosing)
            {
                gameObject.opened = false;
                gameObject.sequence.Reset();
                gameObject.animator.SetBool(gameObject.animParamOpen, false);
                gameObject.ToggleUI(false);

                if (gameObject.soundClose != null)
                {
                    FMODUWE.PlayOneShot(gameObject.soundClose, gameObject.transform.position);
                }
            }
            else if (component.IsOpening)
            {
                ZeroPlayer player = ZeroPlayer.GetPlayerById(packet.GetPacketOwnerId());
                if (player == null)
                {
                    return false;
                }

                if (player.IsMine)
                {
                    using (EventBlocker.Create(techType))
                    {
                        gameObject.OnHandClick(new HandTargetEventData(EventSystem.current)
                        {
                            guiHand = global::Player.main.guiHand
                        });
                    }
                }
                else
                {
                    gameObject.opened = true;
                    gameObject.OnOpen();
                    gameObject.UpdateVisuals();
                }
            }
            else
            {
                foreach (BatteryItem battery in component.Items)
                {
                    ItemQueueAction action = new ItemQueueAction();
                    action.RegisterProperty("BatteryItem", battery);
                    action.RegisterProperty("Charger", gameObject);

                    if (battery.IsActive)
                    {
                        action.OnEntitySpawning = this.OnEntitySpawning;
                        action.OnEntitySpawned = this.OnEntitySpawned;

                        Entity.SpawnToQueue(battery.SlotId, battery.TechType, gameObject.equipment, action);
                    }
                    else
                    {
                        action.OnEntityRemoved = this.OnEntityRemoved;

                        Entity.RemoveToQueue(battery.SlotId, gameObject.equipment);
                    }
                }
            }

            return true;
        }

        public void OnEntityRemoved(ItemQueueProcess item)
        {
            var battery = item.Action.GetProperty<BatteryItem>("BatteryItem");
            var charger = item.Action.GetProperty<Charger>("Charger");
            if (battery != null && battery != null)
            {
                charger.batteries[battery.SlotId] = null;

                if (charger.slots.TryGetValue(battery.SlotId, out var definition))
                {
                    charger.UpdateVisuals(definition, -1f, TechType.None);
                }
            }
        }

        public bool OnEntitySpawning(ItemQueueProcess item)
        {
            if (item.Equipment == null)
            {
                return false;
            }

            var itemInSlot = item.Equipment.GetItemInSlot(item.SlotId);
            if (itemInSlot == null)
            {
                return true;
            }

            var battery = item.Action.GetProperty<BatteryItem>("BatteryItem");
            if (itemInSlot.item.GetTechType() != battery.TechType)
            {
                return true;
            }

            return false;
        }

        public void OnEntitySpawned(ItemQueueProcess item, Pickupable pickupable, GameObject gameObject)
        {
            var battery = item.Action.GetProperty<BatteryItem>("BatteryItem");
            var charger = item.Action.GetProperty<Charger>("Charger");
            if (battery != null && charger != null)
            {
                pickupable.GetComponent<Battery>().charge = battery.Charge;

                if (charger.slots.TryGetValue(battery.SlotId, out var definition))
                {
                    charger.UpdateVisuals(definition, battery.Charge / battery.Capacity, battery.TechType);
                }
            }
        }

        public static void OnChargerItemAdded(ChargerItemAddedEventArgs ev)
        {
            ChargerProcessor.SendDataToServer(ev.ConstructionId, ev.Item.GetTechType(), ev.SlotId, ev.Item.GetComponent<Battery>().charge, false, false, false);
        }

        public static void OnChargerItemRemoved(ChargerItemRemovedEventArgs ev)
        {
            ChargerProcessor.SendDataToServer(ev.ConstructionId, ev.Item.GetTechType(), ev.SlotId, 0.0f, false, true, false);
        }

        public static void OnChargerOpening(ChargerOpeningEventArgs ev)
        {
            ev.IsAllowed = false;

            if (!Interact.IsBlocked(ev.UniqueId))
            {
                ChargerProcessor.SendDataToServer(ev.UniqueId, TechType.None, null, 0.0f, true, false, false);
            }
        }

        public static void OnClosing(PDAClosingEventArgs ev)
        {
            if (ev.TechType == TechType.BatteryCharger || ev.TechType == TechType.PowerCellCharger)
            {
                if (!string.IsNullOrEmpty(ev.UniqueId) && Interact.IsBlockedByMe(ev.UniqueId))
                {
                    var charger = Network.Identifier.GetComponentByGameObject<global::Charger>(ev.UniqueId);
                    if (charger && !charger.HasChargables())
                    {
                        ChargerProcessor.SendDataToServer(ev.UniqueId, TechType.None, null, 0.0f, false, false, true);
                    }
                }
            }
        }

        private static void SendDataToServer(string uniqueId, TechType techType, string slotId, float currentCharge, bool isOpening, bool isRemoving, bool isClosing)
        {
            ServerModel.MetadataComponentArgs result = new ServerModel.MetadataComponentArgs()
            {
                UniqueId = uniqueId,
                Component = new Metadata.Charger()
                {
                    IsOpening = isOpening,
                    IsRemoving = isRemoving,
                    IsClosing = isClosing,
                    Items = new List<BatteryItem>()
                    {
                        new BatteryItem(slotId, techType, currentCharge)
                    },
                },
            };

            NetworkClient.SendPacket(result);
        }
    }
}
