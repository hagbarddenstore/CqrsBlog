namespace CqrsBlog.Models
{
    using System;
    using System.Collections.Generic;

    public abstract class AggregateRoot
    {
        private readonly Queue<DomainEvent> _events = new Queue<DomainEvent>();

        public abstract Guid Id { get; }

        public long Version { get; set; }

        public IEnumerable<DomainEvent> Events
        {
            get { return _events; }
        }

        protected void AppendEvent(DomainEvent newEvent)
        {
            _events.Enqueue(newEvent);
        }

        protected void Replay(IEnumerable<DomainEvent> events)
        {
            dynamic me = this;

            foreach (dynamic @event in events)
            {
                me.Apply(@event);

                Version = @event.Version;
            }
        }
    }
}