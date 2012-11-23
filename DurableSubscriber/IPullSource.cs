using System.Collections.Generic;
using EventStore.ClientAPI;

namespace DurableSubscriber
{
    public interface IPullSource
    {
        IEnumerable<object> ReadSlice();
    }
}