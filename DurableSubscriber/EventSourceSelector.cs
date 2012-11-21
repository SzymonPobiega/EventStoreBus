namespace DurableSubscriber
{
    public class EventSourceSelector
    {
        private readonly IPullSource pullSource;
        private readonly IPushSource pushSource;

        public EventSourceSelector(IPullSource pullSource, IPushSource pushSource)
        {
            this.pullSource = pullSource;
            this.pushSource = pushSource;
        }
    }
}