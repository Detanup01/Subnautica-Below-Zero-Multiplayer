namespace Subnautica.Server.Abstracts.Processors
{
    using Subnautica.API.Extensions;
    using Subnautica.Network.Models.Creatures;
    using Subnautica.Network.Models.Server;
    using Subnautica.Server.Core;

    public abstract class WorldCreatureProcessor : BaseProcessor
    {
        public abstract bool OnDataReceived(AuthorizationProfile profile, CreatureProcessArgs networkPacket, MultiplayerCreatureItem creature, string creatureId);

        public static bool ExecuteProcessor(AuthorizationProfile profile, CreatureProcessArgs networkPacket, MultiplayerCreatureItem creature)
        {
            if (ProcessorShared.WorldCreatureProcessors.TryGetValue(creature.TechType, out var processor))
            {
                processor.OnDataReceived(profile, networkPacket, creature, creature.Id.ToCreatureStringId());
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}