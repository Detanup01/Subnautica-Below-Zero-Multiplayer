namespace Subnautica.Server.Processors.Metadata
{
    using Subnautica.Network.Models.Metadata;
    using Subnautica.Network.Models.Server;
    using Subnautica.Network.Models.Storage.Construction;
    using Subnautica.Server.Abstracts.Processors;
    using Subnautica.Server.Core;
    using System.Linq;
    using Metadata = Subnautica.Network.Models.Metadata;

    public class FridgeProcessor : MetadataProcessor
    {
        public override bool OnDataReceived(AuthorizationProfile profile, MetadataComponentArgs packet, ConstructionItem construction)
        {
            if (Server.Instance.Logices.Interact.IsBlocked(construction.UniqueId, profile.UniqueId))
            {
                return false;
            }

            var component = packet.Component.GetComponent<Metadata.Fridge>();
            if (component == null)
            {
                return false;
            }

            var constructionComponent = construction.EnsureComponent<Metadata.Fridge>();
            if (constructionComponent == null)
            {
                return false;
            }

            var storageContainer = this.GetStorageContainer(constructionComponent);
            if (storageContainer == null)
            {
                return false;
            }

            if (component.IsAdded)
            {
                if (Server.Instance.Logices.Storage.TryPickupItem(component.WorldPickupItem, storageContainer, profile.InventoryItems))
                {
                    component.ItemComponent = this.UpdateFridgeItemComponent(constructionComponent, component.WorldPickupItem.Item.ItemId, construction.UniqueId, component.IsDecomposes, component.TimeDecayStart);

                    profile.SendPacketToAllClient(packet);
                }
            }
            else
            {
                if (Server.Instance.Logices.Storage.TryPickupItem(component.WorldPickupItem, profile.InventoryItems, storageContainer))
                {
                    component.ItemComponent = this.UpdateFridgeItemComponent(constructionComponent, component.WorldPickupItem.Item.ItemId, isAdded: false);

                    profile.SendPacketToAllClient(packet);
                }
            }

            return true;
        }

        private Metadata.FridgeItemComponent UpdateFridgeItemComponent(Metadata.Fridge fridge, string itemId, string constructionUniqueId = null, bool isDecomposes = false, float timeDecayStart = 0f, bool isAdded = true)
        {
            if (isAdded)
            {
                if (!isDecomposes)
                {
                    return null;
                }

                var itemComponent = new Metadata.FridgeItemComponent()
                {
                    ItemId = itemId,
                    IsDecomposes = isDecomposes,
                    TimeDecayStart = timeDecayStart,
                    TimeDecayPause = Server.Instance.Logices.World.GetServerTime(),
                    IsPaused = Server.Instance.Logices.Fridge.IsPowered(constructionUniqueId)
                };

                fridge.Components.RemoveAll(q => q.ItemId == itemId);
                fridge.Components.Add(itemComponent);
                return itemComponent;
            }
            else
            {
                var itemComponent = fridge.Components.FirstOrDefault(q => q.ItemId == itemId);
                if (itemComponent != null)
                {
                    itemComponent.UnpauseDecay(Server.Instance.Logices.World.GetServerTime());
                }
                fridge.Components.RemoveAll((FridgeItemComponent q) => q.ItemId == itemId);
                return itemComponent;
            }
        }

        private Metadata.StorageContainer GetStorageContainer(Metadata.Fridge fridge)
        {
            if (fridge.StorageContainer == null)
            {
                fridge.StorageContainer = Metadata.StorageContainer.Create(5, 7);
            }

            return fridge.StorageContainer;
        }
    }
}
