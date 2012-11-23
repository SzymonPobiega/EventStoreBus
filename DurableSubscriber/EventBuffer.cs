using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace DurableSubscriber
{
    public class EventBuffer
    {
        private readonly BlockingCollection<RecordedEvent> store = new BlockingCollection<RecordedEvent>();

        public void Add(RecordedEvent evnt)
        {
            store.Add(evnt);
        }

        public void Complete()
        {
            store.CompleteAdding();
        }

        public Task StartProcessing(Action<RecordedEvent> handler)
        {
            var task = Task.Factory.StartNew(() =>
                                      {
                                          foreach (var evnt in store.GetConsumingEnumerable())
                                          {
                                              handler(evnt);
                                          }
                                      });
            return task;
        }
    }
}