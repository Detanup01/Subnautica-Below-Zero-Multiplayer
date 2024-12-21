﻿namespace Subnautica.Client.Abstracts.Processors
{
    using Subnautica.API.Features;
    using Subnautica.Network.Core.Components;
    using Subnautica.Network.Models.Server;

    public abstract class MetadataProcessor : BaseProcessor
    {
        public abstract bool OnDataReceived(string uniqueId, TechType techType, MetadataComponentArgs packet, bool isSilence);

        public static bool ExecuteProcessor(TechType techType, string uniqueId, MetadataComponentArgs packet, bool isSilence = false)
        {
            if (ProcessorShared.MetadataProcessors.TryGetValue(techType, out MetadataProcessor processor))
            {
                processor.OnDataReceived(uniqueId, techType, packet, isSilence);
                return true;
            }
            else
            {
                Log.Error(string.Format("Metadata Processor Not Found: {0}, UniqueId: {1}", techType, uniqueId));
                return false;
            }
        }

        public static bool ExecuteProcessor(TechType techType, string uniqueId, MetadataComponent component, bool isSilence = false)
        {
            var packet = new MetadataComponentArgs()
            {
                TechType  = techType,
                UniqueId  = uniqueId,
                Component = component,
            };

            return ExecuteProcessor(techType, uniqueId, packet, isSilence);
        }
    }
}