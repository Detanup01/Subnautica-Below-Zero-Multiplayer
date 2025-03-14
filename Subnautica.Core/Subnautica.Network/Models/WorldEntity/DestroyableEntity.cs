namespace Subnautica.Network.Models.WorldEntity
{
    using MessagePack;

    using Subnautica.API.Enums;
    using Subnautica.Network.Core.Components;

    [MessagePackObject]
    public class DestroyableEntity : NetworkWorldEntityComponent
    {
        [Key(2)]
        public override EntityProcessType ProcessType { get; set; } = EntityProcessType.Destroyable;

        [Key(4)]
        public float Health { get; set; }
    }
}
