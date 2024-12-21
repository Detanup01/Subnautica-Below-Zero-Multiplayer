namespace Subnautica.Server.Extensions
{
    using System.Collections.Generic;
    using System.Linq;

    using Subnautica.API.Extensions;
    using Subnautica.Network.Models.Storage.World.Childrens;

    public static class WorldExtensions
    {
        public static void RenewId(this WorldDynamicEntity entity)
        {
            entity.Id = Server.Core.Server.Instance.Logices.World.GetNextItemId();
        }

        public static WorldDynamicEntity GetSeaTruckHeadModule(this WorldDynamicEntity entity)
        {
            var uniqueId = entity.UniqueId;

            while (uniqueId.IsNotNull())
            {
                if (Server.Core.Server.Instance.Storages.World.Storage.SeaTruckConnections.TryGetValue(uniqueId, out var frontConnectionId))
                {
                    uniqueId = frontConnectionId;
                }
                else
                {
                    break;
                }
            }

            var seaTruck = Core.Server.Instance.Storages.World.GetDynamicEntity(uniqueId);
            if (seaTruck == null)
            {
                return null;
            }

            return seaTruck;
        }

        public static IEnumerable<WorldDynamicEntity> GetSeaTruckFrontModule(this WorldDynamicEntity entity)
        {
            var uniqueId = entity.UniqueId;

            while (uniqueId.IsNotNull())
            {
                if (Server.Core.Server.Instance.Storages.World.Storage.SeaTruckConnections.TryGetValue(uniqueId, out var frontConnectionId))
                {
                    uniqueId = frontConnectionId;

                    yield return Core.Server.Instance.Storages.World.GetDynamicEntity(uniqueId);
                }
                else
                {
                    break;
                }
            }
        }

        public static WorldDynamicEntity GetSeaTruckRearModule(this WorldDynamicEntity entity)
        {
            var connection = Server.Core.Server.Instance.Storages.World.Storage.SeaTruckConnections.FirstOrDefault(q => q.Value == entity.UniqueId);
            if (connection.Value == null)
            {
                return null;
            }

            return Core.Server.Instance.Storages.World.GetDynamicEntity(connection.Key);
        }
    }
}
