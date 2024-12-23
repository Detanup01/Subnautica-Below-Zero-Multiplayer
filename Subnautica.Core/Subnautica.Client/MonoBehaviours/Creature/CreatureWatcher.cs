﻿namespace Subnautica.Client.MonoBehaviours.Creature
{
    using Subnautica.API.Extensions;
    using Subnautica.API.Features;
    using Subnautica.API.Features.Creatures;
    using Subnautica.Client.Core;
    using Subnautica.Network.Models.Server;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using ServerModel = Subnautica.Network.Models.Server;

    public class CreatureWatcher : MonoBehaviour
    {
        public StopwatchItem Timing { get; set; } = new StopwatchItem(BroadcastInterval.CreaturePosition);

        public bool IsNormalTrigger { get; set; } = false;

        public List<WorldCreaturePosition> Positions { get; set; } = new List<WorldCreaturePosition>();

        public void Update()
        {
            if (World.IsLoaded)
            {
                if (this.Timing.IsFinished())
                {
                    this.Timing.Restart();
                    this.IsNormalTrigger = !this.IsNormalTrigger;

                    foreach (var creature in Network.Creatures.GetActiveCreatures())
                    {
                        if (creature.IsMine())
                        {
                            if (creature.Data.IsFastSyncActivated)
                            {
                                this.AddPositionToQueue(creature.Id, creature.GetCreatureObject());
                                continue;
                            }

                            if (this.IsNormalTrigger)
                            {
                                this.AddPositionToQueue(creature.Id, creature.GetCreatureObject());
                                continue;
                            }
                        }
                    }

                    this.SendPositionPacketToServer();
                }

                foreach (var creature in Network.Creatures.GetActiveCreatures())
                {
                    if (creature.IsMine() == false)
                    {
                        creature.GetCreatureObject()?.Movement.SimpleMoveV2();
                    }
                }
            }
        }

        public void AddPositionToQueue(ushort creatureId, MultiplayerCreature creature)
        {
            if (creature != null && creature.IsActive)
            {
                if (creature.GameObject)
                {
                    this.Positions.Add(new WorldCreaturePosition()
                    {
                        CreatureId = creatureId,
                        Position = creature.GameObject.transform.position.Compress(),
                        Rotation = creature.GameObject.transform.rotation.Compress(),
                    });
                }
                else
                {
                    Log.Info("NULL => " + creatureId + ", TYPE => " + creature.CreatureItem.TechType);
                }
            }
        }

        public void SendPositionPacketToServer()
        {
            if (this.Positions.Count > 0)
            {
                foreach (var positions in this.Positions.Split(21))
                {
                    ServerModel.WorldCreaturePositionArgs request = new ServerModel.WorldCreaturePositionArgs()
                    {
                        Positions = positions.ToList(),
                    };

                    NetworkClient.SendPacket(request);
                }

                this.Positions.Clear();
            }
        }
    }
}

