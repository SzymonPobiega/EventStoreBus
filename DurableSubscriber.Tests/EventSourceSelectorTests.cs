using System;
using System.Threading;
using NUnit.Framework;

// ReSharper disable InconsistentNaming
namespace DurableSubscriber.Tests
{
    [TestFixture]
    public class EventSourceSelectorTests
    {
        [Test]
        public void When_there_are_no_outstanding_events_it_switches_to_push_source()
        {
            var eventCollection = new TestEventCollection();

            var pushSource = new FakePushSource();
            var pullSource = new FakePullSource();

            var selector = new EventSourceSelector(pullSource, pushSource, new FakeEventIdentityExtractor());

            selector.Event += (s, e) => eventCollection.MarkAsProcessed(e.Event);
                        
            selector.Start();

            pushSource.PushEvent(eventCollection.GenerateEvent());

            selector.Stop();

            eventCollection.VerifyAllEventsProcessed();
        }

        [Test]
        public void When_there_are_some_outstanding_events_it_pulls_them_and_then_switches_to_push_source()
        {
            var eventCollection = new TestEventCollection();

            var pushSource = new FakePushSource();
            var pullSource = new FakePullSource();

            pullSource.PrepareSlice(eventCollection.GenerateEvent());

            var selector = new EventSourceSelector(pullSource, pushSource, new FakeEventIdentityExtractor());

            selector.Event += (s, e) => eventCollection.MarkAsProcessed(e.Event);

            selector.Start();

            pushSource.PushEvent(eventCollection.GenerateEvent());

            selector.Stop();

            eventCollection.VerifyAllEventsProcessed();
        }

        [Test]
        public void When_some_events_are_pulled_after_push_is_started_they_are_processed_before_pushed_events()
        {
            var eventCollection = new TestEventCollection();

            var pushSource = new FakePushSource();
            var pullSource = new FakePullSource();

            pullSource.PrepareSlice(eventCollection.GenerateEvent());

            var selector = new EventSourceSelector(pullSource, pushSource, new FakeEventIdentityExtractor());

            selector.Event += (s, e) => eventCollection.MarkAsProcessed(e.Event);

            var pullAfterPushEvent = eventCollection.GenerateEvent();
            pushSource.Started += (s, e) => pullSource.PrepareSlice(pullAfterPushEvent);

            selector.Start();

            pushSource.PushEvent(eventCollection.GenerateEvent());

            selector.Stop();

            eventCollection.VerifyAllEventsProcessed();
        }

        [Test]
        public void When_same_event_is_pulled_and_pushed_it_is_processed_only_once()
        {
            var eventCollection = new TestEventCollection();

            var pushSource = new FakePushSource();
            var pullSource = new FakePullSource();

            pullSource.PrepareSlice(eventCollection.GenerateEvent());

            var selector = new EventSourceSelector(pullSource, pushSource, new FakeEventIdentityExtractor());

            selector.Event += (s, e) => eventCollection.MarkAsProcessed(e.Event);

            var duplicate = eventCollection.GenerateEvent();
            pushSource.Started += (s, e) => pullSource.PrepareSlice(duplicate);

            selector.Start();

            pushSource.PushEvent(duplicate);

            selector.Stop();

            eventCollection.VerifyAllEventsProcessed();
        }
    }
}
// ReSharper restore InconsistentNaming
