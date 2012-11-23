using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly EventBuffer buffer;
        private Task processingTask;

        public EventSourceSelector(IPullSource pullSource, IPushSource pushSource, IEventIdentityExtractor eventIdentityExtractor)
        {
            this.pullSource = pullSource;
            this.pushSource = pushSource;
            pushSource.Event += BufferEvent;
            this.buffer = new EventBuffer(eventIdentityExtractor);
        }

        private void BufferEvent(object sender, RecordedEventEventArgs e)
        {
            buffer.Add(e.Event);
        }

        public event EventHandler<RecordedEventEventArgs> Event;

        public void Start()
        {
            processingTask = Task.Factory
                .StartNew(ReadAllFromPullSource, cancellationTokenSource.Token)
                .ContinueWith(x => StartPushSource(), cancellationTokenSource.Token)
                .ContinueWith(x => ReadOnceFromPullSource(), cancellationTokenSource.Token)
                .ContinueWith(x => StartProcessingFromPushSource(x.Result), cancellationTokenSource.Token);
        }

        private void StartProcessingFromPushSource(IEnumerable<object> lastPulledSlice)
        {
            var processorTask = buffer.StartProcessing(lastPulledSlice, OnEvent);
            processorTask.Wait();
        }

        private IEnumerable<object> ReadOnceFromPullSource()
        {
            var slice = pullSource.ReadSlice();
            ProcessEvents(slice);
            return slice;
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
                if (!slice.Any())
                {
                    break;
                }
                ProcessEvents(slice);
            }
        }

        private void ProcessEvents(IEnumerable<object> events)
        {
            foreach (var evnt in events)
            {
                OnEvent(evnt);
            }
        }

        private void OnEvent(object evnt)
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
            //cancellationTokenSource.Cancel();
            processingTask.Wait();                 
        }
    }
}