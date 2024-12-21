namespace Subnautica.Network.Models.Metadata
{
    using MessagePack;

    using Subnautica.Network.Core.Components;
    using Subnautica.Network.Models.WorldEntity.DynamicEntityComponents.Shared;

    [MessagePackObject]
    public class StorageLocker : MetadataComponent
    {
        [Key(0)]
        public bool IsAdded { get; set; }

        [Key(1)]
        public WorldPickupItem WorldPickupItem { get; set; }
    }
}
