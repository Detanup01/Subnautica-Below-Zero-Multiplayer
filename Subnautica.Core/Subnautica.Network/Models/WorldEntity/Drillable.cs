namespace Subnautica.Network.Models.WorldEntity
{
    using MessagePack;

    using Subnautica.API.Enums;
    using Subnautica.Network.Core.Components;
    using Subnautica.Network.Models.WorldEntity.DynamicEntityComponents.Shared;

    [MessagePackObject]
    public class Drillable : NetworkWorldEntityComponent
    {
        [Key(2)]
        public override EntityProcessType ProcessType { get; set; } = EntityProcessType.Drillable;

        [Key(4)]
        public LiveMixin LiveMixin { get; set; }
    }
}
