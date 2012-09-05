namespace CqrsBlog.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoDB.Driver.Builders;

    public abstract class Repository<TAggregateRoot>
        where TAggregateRoot : AggregateRoot
    {
        private readonly MongoDatabase _mongoDatabase;

        protected Repository(MongoDatabase mongoDatabase)
        {
            _mongoDatabase = mongoDatabase;
        }

        public void Save(TAggregateRoot aggregateRoot)
        {
            var aggregateRootId = aggregateRoot.Id;
            var version = aggregateRoot.Version;

            if (aggregateRootId == Guid.Empty)
            {
                var aggregateRootsCollection = CreateAggregateRootsCollection();

                aggregateRootId = Guid.NewGuid();
                version = 0;

                var document = new BsonDocument
                {
                    { "_id", aggregateRootId },
                    { "_version", version }
                };

                aggregateRootsCollection.Insert(document);
            }

            var eventsCollection = CreateEventsCollection();
            var events = aggregateRoot.Events.ToList();

            var reservedVersions = new Queue<long>(ReserveVersions(aggregateRootId, version, events.Count));

            foreach (var @event in events)
            {
                @event.SetVersion(reservedVersions.Dequeue());
                @event.AggregateRootId = aggregateRootId;
                @event.HappenedOn = DateTime.UtcNow;

                eventsCollection.Insert(@event);

                PublishEvent(@event);
            }
        }

        public TAggregateRoot Find(Guid aggregateRootId)
        {
            var eventsCollection = CreateEventsCollection();
            var find = Query.EQ("AggregateRootId", aggregateRootId);

            var events = eventsCollection.Find(find)
                .SetSortOrder(SortBy.Ascending("Version"))
                .ToList();

            return CreateInstance(aggregateRootId, events);
        }

        protected abstract TAggregateRoot CreateInstance(Guid aggregateRootId, IEnumerable<DomainEvent> events);

        private static void PublishEvent(dynamic @event)
        {
            DomainEvents.Raise(@event);
        }

        private MongoCollection<BsonDocument> CreateAggregateRootsCollection()
        {
            return _mongoDatabase.GetCollection<BsonDocument>("AggregateRoots");
        }

        private MongoCollection<DomainEvent> CreateEventsCollection()
        {
            return _mongoDatabase.GetCollection<DomainEvent>("Events");
        }

        private IEnumerable<long> ReserveVersions(Guid aggregateRootId, long currentVersion, long howManyToReserve)
        {
            var aggregateRootsCollection = CreateAggregateRootsCollection();

            var find = new QueryDocument
            {
                { "_id", aggregateRootId },
                { "_version", currentVersion }
            };

            var update = Update.Inc("_version", howManyToReserve);

            var result = aggregateRootsCollection.FindAndModify(find, SortBy.Null, update, true, false);

            var newVersion = result.ModifiedDocument["_version"].AsInt64;
            var reservedVersions = new List<long>();

            for (var i = currentVersion + 1; i <= newVersion; i++)
            {
                reservedVersions.Add(i);
            }

            return reservedVersions;
        }
    }
}