namespace CqrsBlog.Models
{
    using System;

    public abstract class DomainEvent
    {
        public Guid Id { get; set; }

        public Guid AggregateRootId { get; set; }

        public DateTime HappenedOn { get; set; }

        public long Version { get; private set; }

        public void SetVersion(long version)
        {
            Version = version;
        }
    }
}