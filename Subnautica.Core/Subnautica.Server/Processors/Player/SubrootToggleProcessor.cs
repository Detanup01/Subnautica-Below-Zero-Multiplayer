namespace Subnautica.Server.Processors.Player
{
    using Server.Core;
    
    using Subnautica.Network.Models.Core;
    using Subnautica.Server.Abstracts.Processors;

    using ServerModel = Subnautica.Network.Models.Server;

    public class SubrootToggleProcessor : NormalProcessor
    {
        public override bool OnExecute(AuthorizationProfile profile, NetworkPacket networkPacket)
        {
            var packet = networkPacket.GetPacket<ServerModel.SubrootToggleArgs>();
            if (packet == null)
            {
                return this.SendEmptyPacketErrorLog(networkPacket);
            }

            if (packet.IsEntered)
            {
                profile.SetSubroot(packet.SubrootId);
                profile.SetInterior(null);
            }
            else
            {
                profile.SetSubroot(null);
            }

            profile.SendPacketToOtherClients(packet);
            return true;
        }
    }
}
