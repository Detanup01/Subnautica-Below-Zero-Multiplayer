namespace Subnautica.Network.Models.Client
{
    using MessagePack;
    using Subnautica.API.Enums;
    using Subnautica.Network.Models.Core;
    using Subnautica.Network.Models.Storage.Player;
    using Subnautica.Network.Models.WorldStreamer;
    using System.Collections.Generic;

    [MessagePackObject]
    public class WorldLoadedArgs : NetworkPacket
    {
        [Key(0)]
        public override ProcessType Type { get; set; } = ProcessType.WorldLoaded;

        [Key(1)]
        public override NetworkChannel ChannelType { get; set; } = NetworkChannel.Startup;

        [Key(5)]
        public bool IsSpawnPointRequest { get; set; }

        [Key(6)]
        public bool IsSpawnPointExists { get; set; }

        [Key(7)]
        public Dictionary<string, Metadata.PictureFrame> Images { get; set; } = new Dictionary<string, Metadata.PictureFrame>();

        [Key(8)]
        public List<string> ExistImages { get; set; } = new List<string>();

        [Key(9)]
        public List<PlayerItem> Players { get; set; } = new List<PlayerItem>();

        [Key(10)]
        public HashSet<ZeroSpawnPointSimple> SpawnPoints { get; set; } = new HashSet<ZeroSpawnPointSimple>();
    }
}
