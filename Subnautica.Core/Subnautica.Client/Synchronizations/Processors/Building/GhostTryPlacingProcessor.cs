﻿namespace Subnautica.Client.Synchronizations.Processors.Building
{
    using Subnautica.API.Enums;
    using Subnautica.API.Extensions;
    using Subnautica.API.Features;
    using Subnautica.Client.Abstracts;
    using Subnautica.Client.Core;
    using Subnautica.Events.EventArgs;
    using Subnautica.Network.Models.Core;
    using Constructing = Subnautica.Client.Multiplayer.Constructing;
    using ServerModel = Subnautica.Network.Models.Server;

    public class GhostTryPlacingProcessor : NormalProcessor
    {
        public override bool OnDataReceived(NetworkPacket networkPacket)
        {
            var packet = networkPacket.GetPacket<ServerModel.ConstructionGhostTryPlacingArgs>();
            if (packet.TechType == TechType.None || packet.UniqueId.IsNull())
            {
                return false;
            }

            if (packet.IsError)
            {
                Constructing.Builder.CallErrorSound(packet.Position.ToVector3());
            }
            else
            {
                Log.Info("GhostTryPlacingProcessor: " + packet.UniqueId);

                var buildingProgress = Constructing.Builder.GetBuildingProgressType(packet.UniqueId);
                if (buildingProgress == BuildingProgressType.None)
                {
                    this.SetFinished(false);

                    var builder = Constructing.Builder.CreateBuilder(packet.UniqueId, packet.TechType);
                    builder.SetPosition(packet.Position.ToVector3());
                    builder.SetRotation(packet.Rotation.ToQuaternion());
                    builder.SetSubRootId(packet.SubrootId);
                    builder.SetAimTransform(packet.AimTransform);
                    builder.SetLastRotation(packet.LastRotation);
                    builder.SetIsGhostModelAnimation(false);
                    builder.SetIsCanPlace(true);
                    builder.SetUpdatePlacement(true);
                    builder.SetIsTryDefaultPlace(true);
                    builder.SetBaseGhostComponent(packet.BaseGhostComponent);
                    builder.StartBuild(this.OnFinishedSuccessCallback);

                    ConstructionSyncedProcessor.UpdateConstructionSync();
                }
                else if (buildingProgress == BuildingProgressType.GhostModelMoving)
                {
                    var builder = Constructing.Builder.GetBuilder(packet.UniqueId);
                    builder.SetPosition(packet.Position.ToVector3());
                    builder.SetRotation(packet.Rotation.ToQuaternion());
                    builder.SetSubRootId(packet.SubrootId);
                    builder.SetAimTransform(packet.AimTransform);
                    builder.SetLastRotation(packet.LastRotation);
                    builder.SetIsGhostModelAnimation(false);
                    builder.SetIsCanPlace(true);
                    builder.SetUpdatePlacement(true);
                    builder.SetBaseGhostComponent(packet.BaseGhostComponent);
                    builder.TryPlace(true);

                    ConstructionSyncedProcessor.UpdateConstructionSync();
                }
            }

            return true;
        }

        public override void OnStart()
        {
            this.SetWaitingForNextFrame(true);
        }

        public static void OnConstructingGhostTryPlacing(ConstructionGhostTryPlacingEventArgs ev)
        {
            ev.IsAllowed = false;

            ServerModel.ConstructionGhostTryPlacingArgs request = new ServerModel.ConstructionGhostTryPlacingArgs()
            {
                UniqueId = ev.UniqueId,
                SubrootId = ev.SubrootId,
                TechType = ev.TechType,
                LastRotation = ev.LastRotation,
                Position = ev.Position.ToZeroVector3(),
                Rotation = ev.Rotation.ToZeroQuaternion(),
                AimTransform = ev.AimTransform.ToZeroTransform(),
                IsCanPlace = ev.IsCanPlace,
                IsBasePiece = ev.IsBasePiece,
                IsError = ev.IsError,
                BaseGhostComponent = ev.GhostModel.GetBaseGhostComponent(),
            };

            NetworkClient.SendPacket(request);
        }
    }
}
