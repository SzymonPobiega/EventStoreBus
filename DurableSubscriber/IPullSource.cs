using EventStore.ClientAPI;

namespace DurableSubscriber
{
    public interface IPullSource
    {
        EventStreamSlice ReadSlice();
    }
}