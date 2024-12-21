namespace Subnautica.Network.Models.Server
{
    using MessagePack;

    using Subnautica.API.Enums;
    using Subnautica.Network.Models.Core;

    [MessagePackObject]
    public class WelderArgs : NetworkPacket
    {
        [Key(0)]
        public override ProcessType Type { get; set; } = ProcessType.WelderRepair;

        [Key(5)]
        public string UniqueId { get; set; }

        [Key(6)]
        public TechType TechType { get; set; }

        [Key(7)]
        public float Health { get; set; }
    }
}
