using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace DurableSubscriber
{
    public class EventSourceSelector : IPushSource
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly IPullSource pullSource;
        private readonly IPushSource pushSource;
        private readonly EventBuffer buffer = new EventBuffer();

        public EventSourceSelector(IPullSource pullSource, IPushSource pushSource)
        {
            this.pullSource = pullSource;
            this.pushSource = pushSource;
            pushSource.Event += BufferEvent;
        }

        private void BufferEvent(object sender, RecordedEventEventArgs e)
        {
            buffer.Add(e.Event);
        }

        public event EventHandler<RecordedEventEventArgs> Event;

        public void Start()
        {
            var processingTask = Task.Factory
                .StartNew(ReadAllFromPullSource, cancellationTokenSource.Token)
                .ContinueWith(x => StartPushSource(), cancellationTokenSource.Token)
                .ContinueWith(x => ReadOnceFromPullSource(), cancellationTokenSource.Token)
                .ContinueWith(x => StartProcessingFromPushSource(), cancellationTokenSource.Token);

            processingTask.Wait();
        }

        private void StartProcessingFromPushSource()
        {
            var processorTask = buffer.StartProcessing(OnEvent);
            processorTask.Wait();
        }

        private void ReadOnceFromPullSource()
        {
            var slice = pullSource.ReadSlice();
            if (slice.IsEndOfStream)
            {
                return;
            }
            ProcessEvents(slice.Events);
        }
        
        private void StartPushSource()
        {
            pushSource.Start();
        }

        private void ReadAllFromPullSource()
        {
            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                var slice = pullSource.ReadSlice();                
                if (slice.IsEndOfStream)
                {
                    break;
                }
                ProcessEvents(slice.Events);
            }
        }

        private void ProcessEvents(IEnumerable<RecordedEvent> events)
        {
            foreach (var evnt in events)
            {
                OnEvent(evnt);
            }
        }

        private void OnEvent(RecordedEvent evnt)
        {
            if (Event != null)
            {
                Event(this, new RecordedEventEventArgs(evnt));
            }
        }

        public void Stop()
        {
            pushSource.Stop();
            buffer.Complete();
            cancellationTokenSource.Cancel();                        
        }
    }
}