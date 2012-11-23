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
            
        }

        [Test]
        public void When_there_are_some_outstanding_events_it_pulls_them_and_then_switches_to_push_source()
        {
            
        }

        [Test]
        public void When_some_events_are_pulled_after_push_is_started_they_are_processed_before_pushed_events()
        {
            
        }

        [Test]
        public void Events_pushed_before_last_pull_is_done_are_processed()
        {
            
        }

        [Test]
        public void Events_pushed_after_last_pull_is_done_are_processed()
        {
            
        }
    }
}
// ReSharper restore InconsistentNaming
