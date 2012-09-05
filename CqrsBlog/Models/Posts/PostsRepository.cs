namespace CqrsBlog.Models.Posts
{
    using System;
    using System.Collections.Generic;

    using MongoDB.Driver;

    public class PostsRepository : Repository<Post>
    {
        public PostsRepository(MongoDatabase mongoDatabase)
            : base(mongoDatabase)
        {
        }

        protected override Post CreateInstance(Guid aggregateRootId, IEnumerable<DomainEvent> events)
        {
            var post = new Post(aggregateRootId, events);

            return post;
        }
    }
}