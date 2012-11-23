using System;
using EventStore.ClientAPI;

namespace DurableSubscriber
{
    public class AllEventStreamsPushSource : IPushSource
    {
        private readonly EventStoreConnection connection;

        public AllEventStreamsPushSource(EventStoreConnection connection)
        {
            this.connection = connection;
        }

        public event EventHandler<RecordedEventEventArgs> Event;

        public void Start()
        {
            connection.SubscribeToAllStreamsAsync(OnEvent,
                                                  () =>
                                                      { throw new NotImplementedException("Dropping not yet implemted");
                                                      });
        }

        private void OnEvent(RecordedEvent recordedEvent, Position position)
        {
            if (Event != null)
            {
                Event(this, new RecordedEventEventArgs(recordedEvent));
            }
        }

        public void Stop()
        {
            connection.UnsubscribeFromAllStreamsAsync().Wait();
        }
    }
}