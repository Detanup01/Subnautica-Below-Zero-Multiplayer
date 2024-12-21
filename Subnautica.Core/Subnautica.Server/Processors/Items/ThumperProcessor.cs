namespace Subnautica.Server.Processors.Items
{
    using Subnautica.API.Enums;
    using Subnautica.Network.Models.Metadata;
    using Subnautica.Network.Models.Server;
    using Subnautica.Network.Models.WorldEntity.DynamicEntityComponents.Shared;
    using Subnautica.Network.Structures;
    using Subnautica.Server.Abstracts.Processors;
    using Subnautica.Server.Core;

    using ItemModel = Subnautica.Network.Models.Items;
    using WorldEntityModel = Subnautica.Network.Models.WorldEntity.DynamicEntityComponents;

    public class ThumperProcessor : PlayerItemProcessor
    {
        public override bool OnDataReceived(AuthorizationProfile profile, PlayerItemActionArgs packet)
        {
            var component = packet.Item.GetComponent<ItemModel.Thumper>();
            if (component == null)
            {
                return false;
            }

            var worldPickupItem = WorldPickupItem.Create(StorageItem.Create(component.UniqueId, TechType.Thumper), PickupSourceType.PlayerInventoryDrop);

            if (Server.Instance.Logices.Storage.TryPickupToWorld(worldPickupItem, profile.InventoryItems, out var entity))
            {
                entity.SetPositionAndRotation(component.Position, component.Rotation);
                entity.SetOwnership(profile.UniqueId);
                entity.SetDeployed(true);
                entity.SetComponent(this.GetComponent(component.Charge, component.Position));

                component.Entity = entity;

                profile.SendPacketToAllClient(packet);
            }

            return true;
        }

        public WorldEntityModel.Thumper GetComponent(float charge, ZeroVector3 position)
        {
            return new WorldEntityModel.Thumper()
            {
                Charge = charge,
                Position = position,
            };
        }
    }
}
