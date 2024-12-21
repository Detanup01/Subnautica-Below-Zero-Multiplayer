namespace Subnautica.Client.Synchronizations.Processors.Story
{
    using global::Story;
    using Subnautica.API.Features;
    using Subnautica.Client.Abstracts;
    using Subnautica.Client.Core;
    using Subnautica.Network.Models.Core;
    using System.Collections.Generic;
    using UnityEngine;

    public class EndGameProcessor : NormalProcessor
    {
        private List<string> AttachedArms { get; set; } = new List<string>();
        private BelowZeroEndGame BelowZeroEndGame { get; set; }

        public override bool OnDataReceived(NetworkPacket networkPacket)
        {
            Log.Info("--------- Subnautica Below Zero Multiplayer (by BOT Benson) ---------");
            Log.Info("- Oynadığınız İçin Teşekkürler <3");
            Log.Info("- Modu beğendiyseniz beni patreon üzerinden destekleyebilirsiniz.");
            Log.Info("- Steam     : https://steamcommunity.com/id/botbenson/");
            Log.Info("- Patreon   : https://patreon.com/botbenson");
            Log.Info("- Discord   : https://discord.gg/kxC2EFNcrM");
            Log.Info("- Discord   : @botbenson");
            Log.Info("--------- Subnautica Below Zero Multiplayer (by BOT Benson) ---------");

            NetworkClient.IsSafeDisconnecting = true;
            NetworkClient.Disconnect(true);
            return true;
        }

        public override void OnUpdate()
        {
            if (BelowZeroEndGame.isActive)
            {
                Network.Session.SetEndGameWorldTime((double)Time.unscaledDeltaTime, true);
            }
        }

        public override void OnFixedUpdate()
        {
            if (BelowZeroEndGame.isActive)
            {
                foreach (var player in ZeroPlayer.GetPlayers())
                {
                    if (player.IsPrecursorArm)
                    {
                        if (!this.AttachedArms.Contains(player.UniqueId))
                        {
                            this.SpawnPrecursorArm(player);
                        }
                    }
                    else
                    {
                        if (this.AttachedArms.Contains(player.UniqueId))
                        {
                            this.DestroyPrecursorArm(player);
                        }
                    }
                }

                if (this.BelowZeroEndGame == null)
                {
                    this.BelowZeroEndGame = UnityEngine.GameObject.FindObjectOfType<BelowZeroEndGame>();
                }

                if (this.BelowZeroEndGame)
                {
                    if (!StoryGoal.IsComplete(this.BelowZeroEndGame.endGameTeleportBeginTrigger) && !StoryGoal.IsComplete(this.BelowZeroEndGame.endGameHomeworldTrigger))
                    {
                        if (DayNightCycle.main.GetDayScalar() != this.BelowZeroEndGame.gatePlatformTimeOfDay)
                        {
                            DayNightCycle.main.SetDayNightTime(this.BelowZeroEndGame.gatePlatformTimeOfDay);
                        }
                    }
                }
            }
        }

        private bool SpawnPrecursorArm(ZeroPlayer player)
        {
            var precursorArm = UnityEngine.GameObject.FindObjectOfType<PrecursorArms>();
            if (precursorArm == null)
            {
                return false;
            }

            var arm = UnityEngine.GameObject.Instantiate(precursorArm, null);
            if (arm == null)
            {
                return false;
            }

            this.AttachedArms.Add(player.UniqueId);

            arm.transform.parent = player.PlayerModel.transform;
            arm.transform.localPosition = Vector3.zero;
            arm.transform.localRotation = Quaternion.identity;
            return true;
        }

        private bool DestroyPrecursorArm(ZeroPlayer player)
        {
            this.AttachedArms.Remove(player.UniqueId);

            if (!player.PlayerModel)
            {
                return false;
            }

            var precursorArm = player.PlayerModel.GetComponentInChildren<PrecursorArms>();
            if (precursorArm)
            {
                precursorArm.transform.parent = null;

                UnityEngine.GameObject.Destroy(precursorArm.gameObject);
            }

            return true;
        }

        public override void OnDispose()
        {
            this.BelowZeroEndGame = null;
            this.AttachedArms.Clear();
        }
    }
}
