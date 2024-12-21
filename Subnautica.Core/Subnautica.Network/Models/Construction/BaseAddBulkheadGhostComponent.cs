namespace Subnautica.Network.Models.Construction
{
    using MessagePack;

    using Subnautica.Network.Models.Construction.Shared;

    [MessagePackObject]
    public class BaseAddBulkheadGhostComponent : BaseGhostComponent
    {
        [Key(2)]
        public BaseFaceComponent FaceStart { get; set; }
    }
}