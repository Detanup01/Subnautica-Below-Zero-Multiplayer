﻿namespace Subnautica.Client.Multiplayer.Cinematics
{
    using Subnautica.API.Extensions;
    using Subnautica.API.Features;
    using Subnautica.Client.MonoBehaviours.Player;

    public class SeaTruckPilotingCinematic : CinematicController
    {
        private global::SeaTruckMotor SeaTruckMotor { get; set; }

        public override void OnResetAnimations(PlayerCinematicQueueItem item)
        {
            this.SeaTruckMotor = this.Target.GetComponentInChildren<global::SeaTruckMotor>();

            if (item.CinematicAction == this.StartPilotingCinematic)
            {
                this.SeaTruckMotor.animator.Rebind();
            }
            else 
            {
                this.PrepareStopPiloting();
            }
        }

        public void StartPilotingCinematic()
        {
            if (this.SeaTruckMotor?.seatruckanimation)
            {
                this.SetCinematic(this.SeaTruckMotor.seatruckanimation.GetController(SeaTruckAnimation.Animation.BeginPilot));
                this.StartCinematicMode();
            }
        }

        public void StopPilotingCinematic()
        {
            if (this.SeaTruckMotor?.seatruckanimation)
            {
                this.SetCinematic(this.SeaTruckMotor.seatruckanimation.GetController(SeaTruckAnimation.Animation.EndPilot));
                this.SetCinematicEndMode(this.StopPilotingEndMode);
                this.StartCinematicMode();
            }
        }

        private void PrepareStopPiloting()
        {
            var controller = this.SeaTruckMotor.seatruckanimation.GetController(SeaTruckAnimation.Animation.BeginPilot);
            if (controller)
            {
                controller.animator.ImmediatelyPlay(controller.animParam, true);
                this.PlayerAnimator.ImmediatelyPlay(controller.playerViewAnimationName, true);
            }
        }

        public override bool OnPlayerDisconnected()
        {
            if (this.ZeroPlayer.VehicleId <= 0)
            {
                return false;
            }

            var entity = Network.DynamicEntity.GetEntity(this.ZeroPlayer.VehicleId);
            if (entity == null || entity.TechType != TechType.SeaTruck)
            {
                return false;
            }
            
            if (this.SeaTruckMotor && this.SeaTruckMotor.seatruckanimation)
            {
                this.SeaTruckMotor.seatruckanimation.GetController(SeaTruckAnimation.Animation.BeginPilot).animator.Rebind();
                this.SeaTruckMotor.seatruckanimation.GetController(SeaTruckAnimation.Animation.EndPilot).animator.Rebind();
            }

            return true;
        }

        private void StopPilotingEndMode()
        {
            if (this.SeaTruckMotor && this.ZeroPlayer.IsInSeaTruck && CraftData.GetTechType(this.SeaTruckMotor.gameObject) == TechType.SeaTruck)
            {
                this.ZeroPlayer.GetComponent<PlayerAnimation>().UpdateIsInSeaTruck(true);
            }
        }
    }
}
