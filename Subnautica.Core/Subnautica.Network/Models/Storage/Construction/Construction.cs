namespace Subnautica.Network.Models.Storage.Construction
{
    using MessagePack;

    using System;
    using System.Collections.Generic;

    [MessagePackObject]
    [Serializable]
    public class Construction
    {
        [Key(0)]
        public Dictionary<string, ConstructionItem> Constructions { get; set; } = new Dictionary<string, ConstructionItem>();
    }
}