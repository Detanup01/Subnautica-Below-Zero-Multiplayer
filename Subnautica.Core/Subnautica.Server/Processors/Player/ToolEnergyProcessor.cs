namespace Subnautica.Server.Processors.Player
{
    using Server.Core;
    using Subnautica.Network.Models.Core;
    using Subnautica.Server.Abstracts.Processors;

    using ServerModel = Subnautica.Network.Models.Server;

    public class ToolEnergyProcessor : NormalProcessor
    {
        public override bool OnExecute(AuthorizationProfile profile, NetworkPacket networkPacket)
        {
            var packet = networkPacket.GetPacket<ServerModel.PlayerToolEnergyArgs>();
            if (packet == null)
            {
                return this.SendEmptyPacketErrorLog(networkPacket);
            }

            if (profile.IsInventoryItemExists(packet.UniqueId) && packet.Item?.Item != null)
            {
                profile.AddInventoryItem(packet.Item);
            }

            return true;
        }
    }
}
