using System.Collections.Generic;
using EventStore.ClientAPI;

namespace DurableSubscriber
{
    public class AllEventStreamsPullSource : IPullSource
    {
        private const int SliceSize = 100;
        private readonly EventStoreConnection connection;
        private Position currentPosition;

        public AllEventStreamsPullSource(EventStoreConnection connection)
            : this(connection, Position.Start)
        {
        }

        public AllEventStreamsPullSource(EventStoreConnection connection, Position startFrom)
        {
            this.connection = connection;
            currentPosition = startFrom;
        }

        public IEnumerable<object> ReadSlice()
        {
            var slice = connection.ReadAllEventsForward(currentPosition, SliceSize);
            currentPosition = slice.Position;
            return slice.Events;
        }
    }
}