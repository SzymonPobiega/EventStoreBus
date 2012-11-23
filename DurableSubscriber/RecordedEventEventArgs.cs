using System;
using EventStore.ClientAPI;

namespace DurableSubscriber
{
    public class RecordedEventEventArgs : EventArgs
    {
        public readonly RecordedEvent Event;

        public RecordedEventEventArgs(RecordedEvent evnt)
        {
            Event = evnt;
        }
    }
}