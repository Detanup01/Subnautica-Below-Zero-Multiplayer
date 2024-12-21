namespace Subnautica.Events.EventArgs
{
    using System;

    public class VehicleExitedEventArgs : EventArgs
    {
        public VehicleExitedEventArgs(string uniqueId, TechType techType)
        {
            this.UniqueId = uniqueId;
            this.TechType = techType;
        }

        public string UniqueId { get; private set; }

        public TechType TechType { get; private set; }
    }
}
