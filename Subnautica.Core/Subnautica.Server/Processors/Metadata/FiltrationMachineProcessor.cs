namespace Subnautica.Server.Processors.Metadata
{
    using Subnautica.Network.Models.Server;
    using Subnautica.Network.Models.Storage.Construction;
    using Subnautica.Server.Abstracts.Processors;
    using Subnautica.Server.Core;
    using System.Linq;
    using Metadata = Subnautica.Network.Models.Metadata;

    public class FiltrationMachineProcessor : MetadataProcessor
    {
        public override bool OnDataReceived(AuthorizationProfile profile, MetadataComponentArgs packet, ConstructionItem construction)
        {
            if (Server.Instance.Logices.Interact.IsBlocked(construction.UniqueId, profile.UniqueId))
            {
                return false;
            }

            var component = packet.Component.GetComponent<Metadata.FiltrationMachine>();
            if (component == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(component.RemovingItemId))
            {
                return false;
            }

            var constructionComponent = construction.EnsureComponent<Metadata.FiltrationMachine>();

            var item = constructionComponent.Items.Where(q => q.ItemId == component.RemovingItemId).FirstOrDefault();
            if (item != null)
            {
                item.Clear();

                if (Server.Instance.Storages.Construction.UpdateMetadata(packet.UniqueId, constructionComponent))
                {
                    Server.Instance.Logices.FiltrationMachine.TryFilterSalt(constructionComponent);
                    Server.Instance.Logices.FiltrationMachine.TryFilterWater(constructionComponent);

                    component.TimeRemainingWater = constructionComponent.TimeRemainingWater;
                    component.TimeRemainingSalt = constructionComponent.TimeRemainingSalt;

                    profile.SendPacketToOtherClients(packet);
                }
            }

            return true;
        }
    }
}
