namespace Subnautica.Server.Processors.Metadata
{
    using Subnautica.Network.Models.Server;
    using Subnautica.Network.Models.Storage.Construction;
    using Subnautica.Server.Abstracts.Processors;
    using Subnautica.Server.Core;

    public class SmallStoveProcessor : MetadataProcessor
    {
        public override bool OnDataReceived(AuthorizationProfile profile, MetadataComponentArgs packet, ConstructionItem construction)
        {
            if (Server.Instance.Storages.Construction.UpdateMetadata(packet.UniqueId, packet.Component))
            {
                profile.SendPacketToAllClient(packet);
            }

            return true;
        }
    }
}
