using System;
using System.Collections.Generic;

namespace DurableSubscriber
{
    public interface IPullSource
    {
        IEnumerable<object> ReadSlice();
    }
}