namespace Subnautica.Server.Logic
{
    using Subnautica.API.Features;
    using Subnautica.Server.Abstracts;

    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Timing : BaseLogic
    {
        public Dictionary<string, StopwatchItem> Queue { get; set; } = new Dictionary<string, StopwatchItem>();

        public override void OnFixedUpdate(float fixedTime)
        {
            if (API.Features.World.IsLoaded && this.Queue.Count > 0)
            {
                foreach (var item in this.Queue.ToList())
                {
                    if (item.Value.IsFinished())
                    {
                        try
                        {
                            item.Value.GetCustomData<Action>()?.Invoke();
                        }
                        catch (Exception ex)
                        {
                            Log.Error($"Server.Timing - Exception: {ex}");
                        }
                        finally
                        {
                            this.Queue.Remove(item.Key);
                        }
                    }
                }
            }
        }

        public string AddQueue(string processId, Action action, float time)
        {
            if (time < 0f)
            {
                time = 0f;
            }

            if (time >= 2000000f)
            {
                time = 2000000f;
            }

            this.Queue[processId] = new StopwatchItem(time * 1000f, action);
            return processId;
        }

        public string AddQueue(Action action, float time)
        {
            return AddQueue(Network.Identifier.GenerateUniqueId(), action, time);
        }

        public void RemoveFromQueue(string processId)
        {
            this.Queue.Remove(processId);
        }
    }
}
