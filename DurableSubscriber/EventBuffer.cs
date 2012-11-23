using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace DurableSubscriber
{
    public class EventBuffer
    {
        private readonly IEventIdentityExtractor eventIdentityExtractor;
        private readonly BlockingCollection<object> store = new BlockingCollection<object>();

        public EventBuffer(IEventIdentityExtractor eventIdentityExtractor)
        {
            this.eventIdentityExtractor = eventIdentityExtractor;
        }

        public void Add(object evnt)
        {
            store.Add(evnt);
        }

        public void Complete()
        {
            store.CompleteAdding();
        }

        public Task StartProcessing(IEnumerable<object> lastPulledSlice, Action<object> handler)
        {
            var processed = new HashSet<object>(lastPulledSlice.Select(x => eventIdentityExtractor.GetIdentity(x)));

            var task = Task.Factory.StartNew(() =>
                                      {
                                          foreach (var evnt in store.GetConsumingEnumerable())
                                          {
                                              var identity = eventIdentityExtractor.GetIdentity(evnt);
                                              if (processed.Contains(identity))
                                              {
                                                  continue;
                                              }
                                              handler(evnt);
                                          }
                                      });
            return task;
        }
    }
}