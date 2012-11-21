using System;
using EventStore.ClientAPI;

namespace DurableSubscriber
{
    public interface IPushSource
    {
        IDisposable Subscribe(Action<RecordedEvent> handler);
    }
}