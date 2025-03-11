namespace Subnautica.Server.Logic.Furnitures
{
    using Subnautica.API.Features;
    using Subnautica.Network.Models.Metadata;
    using Subnautica.Network.Models.Storage.Construction;
    using Subnautica.Server.Abstracts;
    using Subnautica.Server.Core;
    using System.Collections.Generic;
    using System.Linq;
    using Metadata = Subnautica.Network.Models.Metadata;
    using ServerModel = Subnautica.Network.Models.Server;

    public class Fridge : BaseLogic
    {
        public StopwatchItem Timing { get; set; } = new StopwatchItem(2000f);

        public override void OnUpdate(float deltaTime)
        {
            if (this.Timing.IsFinished() && World.IsLoaded)
            {
                this.Timing.Restart();

                foreach (var construction in this.GetFridges())
                {
                    var component = construction.Value.EnsureComponent<Metadata.Fridge>();
                    if (component.Components.Count <= 0)
                    {
                        continue;
                    }

                    var fridge = Network.Identifier.GetComponentByGameObject<global::Fridge>(construction.Value.UniqueId);
                    if (fridge == null)
                    {
                        continue;
                    }

                    var wasPowered = fridge.powerConsumer.IsPowered();

                    if (component.WasPowered == wasPowered)
                    {
                        continue;
                    }

                    var serverTime = Server.Instance.Logices.World.GetServerTime();

                    foreach (var item in component.Components)
                    {
                        if (wasPowered)
                        {
                            item.PauseDecay(serverTime);
                        }
                        else
                        {
                            item.UnpauseDecay(serverTime);
                        }
                    }

                    component.WasPowered = wasPowered;

                    this.SendPacketToAllClient(construction.Value.UniqueId, component.Components.ToList(), wasPowered);
                }
            }
        }

        private void SendPacketToAllClient(string uniqueId, List<FridgeItemComponent> components, bool wasPowered)
        {
            ServerModel.MetadataComponentArgs request = new ServerModel.MetadataComponentArgs()
            {
                UniqueId = uniqueId,
                TechType = TechType.Fridge,
                Component = new Metadata.Fridge()
                {
                    WasPowered = wasPowered,
                    Components = components,
                    IsPowerStateChanged = true
                },
            };

            Core.Server.SendPacketToAllClient(request);
        }

        public bool IsPowered(string constructionId)
        {
            var fridge = Network.Identifier.GetComponentByGameObject<global::Fridge>(constructionId);
            if (fridge == null)
            {
                return false;
            }

            return fridge.powerConsumer.IsPowered();
        }

        private List<KeyValuePair<string, ConstructionItem>> GetFridges()
        {
            return Core.Server.Instance.Storages.Construction.Storage.Constructions.Where(q => q.Value.TechType == TechType.Fridge && q.Value.ConstructedAmount == 1f).ToList();
        }
    }
}
