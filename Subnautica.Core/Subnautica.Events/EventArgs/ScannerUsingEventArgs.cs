namespace Subnautica.Events.EventArgs
{
    using System;

    public class ScannerUsingEventArgs : EventArgs
    {
        public ScannerUsingEventArgs(string uniqueId)
        {
            this.UniqueId = uniqueId;
        }

        public string UniqueId { get; set; }
    }
}