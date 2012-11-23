using System;

namespace DurableSubscriber
{
    public interface IPushSource
    {
        event EventHandler<RecordedEventEventArgs> Event;
        void Start();
        void Stop();
    }
}