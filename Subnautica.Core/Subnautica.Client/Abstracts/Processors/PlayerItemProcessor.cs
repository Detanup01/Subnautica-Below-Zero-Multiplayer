﻿namespace Subnautica.Client.Abstracts.Processors
{
    using Subnautica.API.Extensions;
    using Subnautica.API.Features;
    using Subnautica.Client.MonoBehaviours.Player;
    using Subnautica.Network.Core.Components;

    public abstract class PlayerItemProcessor : BaseProcessor
    {
        public abstract bool OnDataReceived(NetworkPlayerItemComponent packet, byte playerId);

        public static bool ExecuteProcessor(NetworkPlayerItemComponent packet, byte playerId)
        {
            if (ProcessorShared.PlayerItemProcessors.TryGetValue(packet.TechType, out var processor))
            {
                processor.OnDataReceived(packet, playerId);
                return true;
            }
            else
            {
                Log.Error(string.Format("PlayerItemProcessor Not Found: {0}, UniqueId: {1}", packet.TechType, packet.UniqueId));
                return false;
            }
        }

        public T GetPlayerTool<T>(ZeroPlayer player, TechType techType)
        {
            var itemManager = player.GetComponent<PlayerHandItemManager>();
            if (itemManager == null)
            {
                return default(T);
            }

            var item = itemManager.GetItem(techType);
            if (item == null)
            {
                return default(T);
            }

            var tool = item.GetComponent<T>();
            if (tool == null)
            {
                return default(T);
            }

            return tool;
        }
    }
}