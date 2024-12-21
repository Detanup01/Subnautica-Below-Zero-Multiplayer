﻿namespace Subnautica.Client.Synchronizations.Processors.Building
{
    using Subnautica.API.Enums;
    using Subnautica.API.Extensions;
    using Subnautica.Client.Abstracts;
    using Subnautica.Client.Core;
    using Subnautica.Events.EventArgs;
    using Subnautica.Network.Models.Core;
    using UnityEngine;
    using Constructing = Subnautica.Client.Multiplayer.Constructing;
    using ServerModel = Subnautica.Network.Models.Server;

    public class GhostMovedProcessor : NormalProcessor
    {
        public override bool OnDataReceived(NetworkPacket networkPacket)
        {
            var packet = networkPacket.GetPacket<ServerModel.ConstructionGhostMovingArgs>();
            if (packet.TechType == TechType.None || packet.UniqueId.IsNull())
            {
                return false;
            }

            if (Constructing.Builder.GetBuildingProgressType(packet.UniqueId) == BuildingProgressType.None)
            {
                this.SetFinished(false);

                var builder = Constructing.Builder.CreateBuilder(packet.UniqueId, packet.TechType);
                builder.SetPosition(packet.Position.ToVector3());
                builder.SetRotation(packet.Rotation.ToQuaternion());
                builder.SetAimTransform(packet.AimTransform);
                builder.SetLastRotation(packet.LastRotation);
                builder.SetBaseGhostComponent(packet.BaseGhostComponent);
                builder.SetIsCanPlace(packet.IsCanPlace);
                builder.SetUpdatePlacement(packet.UpdatePlacement);
                builder.SetUpdatedTime(Time.time);
                builder.StartBuild(this.OnFinishedSuccessCallback);
            }
            else
            {
                var builder = Constructing.Builder.GetBuilder(packet.UniqueId);
                builder.SetPosition(packet.Position.ToVector3());
                builder.SetRotation(packet.Rotation.ToQuaternion());
                builder.SetAimTransform(packet.AimTransform);
                builder.SetBaseGhostComponent(packet.BaseGhostComponent);
                builder.SetIsCanPlace(packet.IsCanPlace);
                builder.SetUpdatePlacement(packet.UpdatePlacement);
                builder.SetLastRotation(packet.LastRotation);
                builder.SetUpdatedTime(Time.time);
            }

            return true;
        }

        public static void OnConstructingGhostMoved(ConstructionGhostMovedEventArgs ev)
        {
            ServerModel.ConstructionGhostMovingArgs request = new ServerModel.ConstructionGhostMovingArgs()
            {
                UniqueId = ev.UniqueId,
                TechType = ev.TechType,
                Position = ev.Position.ToZeroVector3(),
                Rotation = ev.Rotation.ToZeroQuaternion(),
                AimTransform = ev.AimTransform.ToZeroTransform(),
                IsCanPlace = ev.IsCanPlace,
                UpdatePlacement = ev.UpdatePlacement,
                LastRotation = ev.LastRotation,
                BaseGhostComponent = ev.UpdatePlacement ? ev.GhostModel.GetBaseGhostComponent() : null,
            };

            NetworkClient.SendPacket(request);
        }
    }
}
