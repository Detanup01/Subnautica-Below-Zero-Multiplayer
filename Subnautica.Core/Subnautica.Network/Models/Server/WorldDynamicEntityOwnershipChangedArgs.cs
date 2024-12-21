namespace Subnautica.Network.Models.Server
{
    using System.Collections.Generic;

    using MessagePack;

    using Subnautica.API.Enums;
    using Subnautica.Network.Models.Core;
    using Subnautica.Network.Structures;

    [MessagePackObject]
    public class WorldDynamicEntityOwnershipChangedArgs : NetworkPacket
    {
        [Key(0)]
        public override ProcessType Type { get; set; } = ProcessType.WorldDynamicEntityOwnershipChanged;

        [Key(5)]
        public Dictionary<string, List<ushort>> Entities { get; set; }
    }
}
