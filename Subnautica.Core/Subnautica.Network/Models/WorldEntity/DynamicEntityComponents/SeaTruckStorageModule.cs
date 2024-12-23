namespace Subnautica.Network.Models.WorldEntity.DynamicEntityComponents
{
    using MessagePack;
    using Subnautica.API.Features;
    using Subnautica.Network.Core.Components;
    using Subnautica.Network.Models.WorldEntity.DynamicEntityComponents.Shared;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [MessagePackObject]
    public class SeaTruckStorageModule : NetworkDynamicEntityComponent
    {
        [Key(0)]
        public List<SeaTruckLockerItem> Lockers { get; set; } = new List<SeaTruckLockerItem>()
        {
            new SeaTruckLockerItem(),
            new SeaTruckLockerItem(),
            new SeaTruckLockerItem(),
            new SeaTruckLockerItem(),
            new SeaTruckLockerItem()
        };

        [Key(1)]
        public LiveMixin LiveMixin { get; set; } = new LiveMixin(500f, 500f);

        public SeaTruckStorageModule Initialize(Action<NetworkDynamicEntityComponent> onEntityComponentInitialized)
        {
            foreach (var locker in this.Lockers)
            {
                locker.UniqueId = Network.Identifier.GenerateUniqueId();
            }

            this.Lockers.ElementAt(0).StorageContainer = Metadata.StorageContainer.Create(3, 5);
            this.Lockers.ElementAt(1).StorageContainer = Metadata.StorageContainer.Create(6, 3);
            this.Lockers.ElementAt(2).StorageContainer = Metadata.StorageContainer.Create(4, 3);
            this.Lockers.ElementAt(3).StorageContainer = Metadata.StorageContainer.Create(4, 3);
            this.Lockers.ElementAt(4).StorageContainer = Metadata.StorageContainer.Create(3, 5);

            onEntityComponentInitialized?.Invoke(this);
            return this;
        }
    }
}
