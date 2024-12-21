﻿namespace Subnautica.Client.Synchronizations.Processors.Inventory
{
    using Subnautica.API.Enums;
    using Subnautica.API.Features;
    using Subnautica.Client.Abstracts;
    using Subnautica.Client.Core;
    using Subnautica.Network.Models.Core;
    using System.Collections;
    using UnityEngine;
    using ServerModel = Subnautica.Network.Models.Server;

    public class QuickSlotProcessor : NormalProcessor
    {
        private static bool IsSending { get; set; } = false;

        public override bool OnDataReceived(NetworkPacket networkPacket)
        {
            return true;
        }

        public static void OnProcessQuickSlot()
        {
            if (!IsSending && !EventBlocker.IsEventBlocked(ProcessType.InventoryQuickSlot))
            {
                UWE.CoroutineHost.StartCoroutine(SendServerData());
            }
        }

        private static IEnumerator SendServerData()
        {
            IsSending = true;

            yield return new WaitForSecondsRealtime(1f);

            ServerModel.InventoryQuickSlotItemArgs result = new ServerModel.InventoryQuickSlotItemArgs()
            {
                Slots = global::Inventory.main.quickSlots.SaveBinding(),
                ActiveSlot = global::Inventory.main.quickSlots.activeSlot,
            };

            NetworkClient.SendPacket(result);

            IsSending = false;
        }
    }
}