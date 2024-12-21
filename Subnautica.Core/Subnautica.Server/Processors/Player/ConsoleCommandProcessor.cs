namespace Subnautica.Server.Processors.Player
{
    using Server.Core;

    using Subnautica.API.Features;
    using Subnautica.Network.Models.Core;
    using Subnautica.Server.Abstracts.Processors;

    using ServerModel = Subnautica.Network.Models.Server;

    public class ConsoleCommandProcessor : NormalProcessor
    {
        public override bool OnExecute(AuthorizationProfile profile, NetworkPacket networkPacket)
        {
            var packet = networkPacket.GetPacket<ServerModel.PlayerConsoleCommandArgs>();
            if (packet == null)
            {
                return this.SendEmptyPacketErrorLog(networkPacket);
            }

            Log.Info(string.Format("{0} ({1}), Command: {2}", profile.PlayerName, profile.IsHost ? "Y" : "N", packet.Command));
            return true;
        }
    }
}
