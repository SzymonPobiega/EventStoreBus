using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using EventStore.ClientAPI;

namespace DurableSubscriber.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var tcpEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1113);
            using (var connection = EventStoreConnection.Create())
            {
                connection.Connect(tcpEndpoint);

                var pullSource = new AllEventStreamsPullSource(connection);
                var pushSource = new AllEventStreamsPushSource(connection);

                var selector = new EventSourceSelector(pullSource, pushSource, new RecordedEventIdentityExtractor());
                selector.Event += OnEvent;
                selector.Start();

                Console.WriteLine("Press <enter> to exit.");
                Console.ReadLine();
                selector.Stop();
            }
        }

        static void OnEvent(object sender, RecordedEventEventArgs e)
        {
            var recordedEvent = (RecordedEvent) e.Event;
            var dataString = Encoding.UTF8.GetString(recordedEvent.Data);
            Console.WriteLine("Event: " + dataString);
        }
    }
}
