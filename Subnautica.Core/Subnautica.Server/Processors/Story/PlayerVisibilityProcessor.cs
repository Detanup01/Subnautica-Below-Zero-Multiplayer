namespace Subnautica.Server.Processors.Story
{
    using Server.Core;
    using Subnautica.Network.Models.Core;
    using Subnautica.Server.Abstracts.Processors;
    using System.Linq;
    using ClientModel = Subnautica.Network.Models.Client;
    using ServerModel = Subnautica.Network.Models.Server;

    public class PlayerVisibilityProcessor : NormalProcessor
    {
        public override bool OnExecute(AuthorizationProfile profile, NetworkPacket networkPacket)
        {
            var packet = networkPacket.GetPacket<ServerModel.StoryPlayerVisibilityArgs>();
            if (packet == null)
            {
                return this.SendEmptyPacketErrorLog(networkPacket);
            }

            profile.SetStoryCinematicMode(packet.IsCinematicActive);

            ClientModel.StoryPlayerVisibilityArgs request = new ClientModel.StoryPlayerVisibilityArgs();

            foreach (var player in Server.Instance.GetPlayers())
            {
                request.Visibility.Add(player.UniqueId, player.IsStoryCinematicModeActive);
            }

            if (request.Visibility.Any())
            {
                profile.SendPacketToAllClient(request);
            }

            return true;
        }
    }
}
