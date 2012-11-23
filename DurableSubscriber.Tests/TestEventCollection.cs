using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace DurableSubscriber.Tests
{
    public class TestEventCollection
    {
        private readonly List<object> generatedEvents = new List<object>();
        private readonly List<object> processedEvents = new List<object>();

        public object GenerateEvent()
        {
            lock (this)
            {
                var generatedEvent = new object();
                generatedEvents.Add(generatedEvent);
                return generatedEvent;
            }            
        }

        public void MarkAsProcessed(object evnt)
        {
            processedEvents.Add(evnt);
        }

        public void VerifyAllEventsProcessed()
        {
            Assert.IsTrue(generatedEvents.SequenceEqual(processedEvents));
        }
    }
}