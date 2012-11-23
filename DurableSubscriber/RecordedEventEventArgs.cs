using System;
using EventStore.ClientAPI;

namespace DurableSubscriber
{
    public class RecordedEventEventArgs : EventArgs
    {
        public readonly object Event;

        public RecordedEventEventArgs(object evnt)
        {
            Event = evnt;
        }
    }
}