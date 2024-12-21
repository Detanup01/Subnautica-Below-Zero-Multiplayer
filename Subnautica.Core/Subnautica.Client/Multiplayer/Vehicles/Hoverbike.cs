﻿namespace Subnautica.Client.Multiplayer.Vehicles
{
    using Subnautica.API.Extensions;
    using Subnautica.Network.Models.Server;

    public class Hoverbike : VehicleController
    {
        private global::Hoverbike HoverBike { get; set; }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        public override void OnComponentDataReceived(VehicleUpdateComponent component)
        {
            var vehicleComponent = component.GetComponent<HoverbikeUpdateComponent>();
            if (vehicleComponent.IsJumping)
            {
                if (!this.HoverBike.jumpFxControl.IsPlaying(0))
                {
                    this.HoverBike.jumpFxControl.Play();
                    this.HoverBike.sfx_jump.Play();
                }
            }

            if (vehicleComponent.IsBoosting)
            {
                if (!this.HoverBike.boostFxControl.IsPlaying(0))
                {
                    this.HoverBike.boostFxControl.Play();
                    this.HoverBike.sfx_boost.Play();
                }
            }
        }

        public override void OnEnterVehicle()
        {
            base.OnEnterVehicle();

            if (this.Management.Vehicle)
            {
                this.Management.Player.InstantyAnimationMode();

                this.HoverBike = this.Management.Vehicle.GetComponent<global::Hoverbike>();
                this.HoverBike.kinematicOverride = true;
                this.HoverBike.animator.SetBool("player_in", true);
                this.SetPlayerParent(this.HoverBike.playerPosition.transform);

                this.Management.Player.Animator.SetBool("in_hovercraft", true);
            }
        }

        public override void OnExitVehicle()
        {
            if (this.HoverBike)
            {
                this.HoverBike.kinematicOverride = false;
                this.HoverBike.animator.SetBool("player_in", false);
            }

            this.Management.Player.NormalAnimationMode();
            this.Management.Player.Animator.SetBool("in_hovercraft", false);

            this.HoverBike = null;
            base.OnExitVehicle();
        }
    }
}
