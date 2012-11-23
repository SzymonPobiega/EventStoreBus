using EventStore.ClientAPI;

namespace DurableSubscriber
{
    public class RecordedEventIdentityExtractor : IEventIdentityExtractor
    {
        public object GetIdentity(object evnt)
        {
            return ((RecordedEvent) evnt).EventId;
        }
    }
}