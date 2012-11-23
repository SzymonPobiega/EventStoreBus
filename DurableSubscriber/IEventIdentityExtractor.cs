namespace DurableSubscriber
{
    public interface IEventIdentityExtractor
    {
        object GetIdentity(object evnt);
    }
}