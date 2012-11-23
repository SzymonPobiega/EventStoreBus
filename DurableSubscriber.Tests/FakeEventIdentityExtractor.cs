namespace DurableSubscriber.Tests
{
    public class FakeEventIdentityExtractor : IEventIdentityExtractor
    {
        public object GetIdentity(object evnt)
        {
            return evnt;
        }
    }
}