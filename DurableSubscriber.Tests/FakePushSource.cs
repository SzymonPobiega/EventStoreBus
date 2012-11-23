using System;

namespace DurableSubscriber.Tests
{
    public class FakePushSource : IPushSource
    {
        public event EventHandler<RecordedEventEventArgs> Event;

        public bool IsStarted { get; private set; }

        public void Start()
        {
            IsStarted = true;
            if (Started != null)
            {
                Started(this, new EventArgs());
            }
        }

        public void Stop()
        {
            IsStarted = false;
            if (Stopped != null)
            {
                Stopped(this, new EventArgs());
            }
        }

        public void PushEvent(object evnt)
        {
            if (Event != null)
            {
                Event(this, new RecordedEventEventArgs(evnt));
            }
        }

        public event EventHandler Started;
        public event EventHandler Stopped;
    }
}