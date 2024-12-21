namespace Subnautica.Server.Processors.World
{
    using Server.Core;

    using Subnautica.Network.Models.Core;
    using Subnautica.Server.Abstracts.Processors;

    using ServerModel = Subnautica.Network.Models.Server;

    public class SpawnOnKillProcessor : NormalProcessor
    {
        public override bool OnExecute(AuthorizationProfile profile, NetworkPacket networkPacket)
        {
            var packet = networkPacket.GetPacket<ServerModel.SpawnOnKillArgs>();
            if (packet == null)
            {
                return this.SendEmptyPacketErrorLog(networkPacket);
            }

            if (Server.Instance.Logices.Storage.TryPickupToWorld(packet.WorldPickupItem, out var entity))
            {
                entity.SetPositionAndRotation(packet.Entity.Position, packet.Entity.Rotation);
                entity.SetOwnership(profile.UniqueId);

                packet.Entity = entity;

                profile.SendPacketToAllClient(packet);
            }

            return true;
        }
    }
}
