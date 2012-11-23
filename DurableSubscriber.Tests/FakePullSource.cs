using System;
using System.Collections.Generic;
using System.Linq;

namespace DurableSubscriber.Tests
{
    public class FakePullSource : IPullSource
    {
        public int SliceNumber { get; private set; }

        public object NextEvent { get; private set; }

        public IEnumerable<object> ReadSlice()
        {
            if (NextEvent != null)
            {
                SliceNumber++;
                var evnt = NextEvent;
                NextEvent = null;
                if (SliceReturning != null)
                {
                    SliceReturning(this, new EventArgs());
                }
                return new[] { evnt };
            }
            if (NoSlicesLeft != null)
            {
                NoSlicesLeft(this, new EventArgs());
            }
            return Enumerable.Empty<object>();
        }

        public void PrepareSlice(object evnt)
        {
            NextEvent = evnt;
        }

        public event EventHandler NoSlicesLeft;
        public event EventHandler SliceReturning;
    }
}