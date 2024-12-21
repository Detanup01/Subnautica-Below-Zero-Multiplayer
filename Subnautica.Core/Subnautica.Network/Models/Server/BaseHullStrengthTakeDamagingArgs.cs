namespace Subnautica.Network.Models.Server
{
    using MessagePack;
    using Subnautica.API.Enums;
    using Subnautica.Network.Models.Core;
    using Subnautica.Network.Structures;
    using System.Collections.Generic;

    [MessagePackObject]
    public class BaseHullStrengthTakeDamagingArgs : NetworkPacket
    {
        [Key(0)]
        public override ProcessType Type { get; set; } = ProcessType.BaseHullStrength;

        [Key(1)]
        public override NetworkChannel ChannelType { get; set; } = NetworkChannel.Construction;

        [Key(3)]
        public override byte ChannelId { get; set; } = 1;

        [Key(5)]
        public string UniqueId { get; set; }

        [Key(6)]
        public float Damage { get; set; }

        [Key(7)]
        public DamageType DamageType { get; set; }

        [Key(8)]
        public float CurrentHealth { get; set; }

        [Key(9)]
        public float MaxHealth { get; set; }

        [Key(10)]
        public List<ZeroVector3> LeakPoints { get; set; }
    }
}
