namespace Subnautica.Server.Processors.Vehicle
{
    using Server.Core;
    using Subnautica.Network.Models.Core;
    using Subnautica.Server.Abstracts.Processors;
    using ServerModel = Subnautica.Network.Models.Server;
    using WorldEntityModel = Subnautica.Network.Models.WorldEntity.DynamicEntityComponents;

    public class ExosuitJumpProcessor : NormalProcessor
    {
        public override bool OnExecute(AuthorizationProfile profile, NetworkPacket networkPacket)
        {
            var packet = networkPacket.GetPacket<ServerModel.ExosuitJumpArgs>();
            if (packet == null)
            {
                return this.SendEmptyPacketErrorLog(networkPacket);
            }

            var vehicle = Server.Instance.Storages.World.GetDynamicEntity(packet.UniqueId);
            if (vehicle == null || vehicle.TechType != TechType.Exosuit)
            {
                return false;
            }

            var component = vehicle.Component.GetComponent<WorldEntityModel.Exosuit>();
            if (component != null)
            {
                Server.Instance.Logices.VehicleEnergyTransmission.ConsumeEnergy(component.PowerCells, Server.Instance.Logices.VehicleEnergyTransmission.ExosuitJumpPowerConsumption);
                Server.Instance.Logices.VehicleEnergyTransmission.VehicleEnergyUpdateQueue(vehicle, true);
            }

            return true;
        }
    }
}
