namespace Subnautica.Events.EventArgs
{
    using System;

    public class SignSelectEventArgs : EventArgs
    {
        public SignSelectEventArgs(string uniqueId, TechType techType, bool isAllowed = true)
        {
            this.UniqueId = uniqueId;
            this.TechType = techType;
            this.IsAllowed = isAllowed;
        }

        public string UniqueId { get; set; }

        public TechType TechType { get; set; }

        public bool IsAllowed { get; set; }
    }
}
