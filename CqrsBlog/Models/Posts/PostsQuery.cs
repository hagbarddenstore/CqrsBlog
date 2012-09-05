namespace CqrsBlog.Models.Posts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MongoDB.Driver;
    using MongoDB.Driver.Builders;

    public class PostsQuery
    {
        private readonly MongoDatabase _mongoDatabase;

        public PostsQuery(MongoDatabase mongoDatabase)
        {
            _mongoDatabase = mongoDatabase;
        }

        public IEnumerable<PublishedPost> FindAllPublishedPosts()
        {
            var publishedPostsCollection = CreatePublishedPostsCollection();

            var find = Query.LTE("PublishedOn", DateTime.UtcNow);

            var publishedPosts = publishedPostsCollection.Find(find)
                .SetSortOrder(SortBy.Descending("PublishedOn"))
                .ToList();

            return publishedPosts;
        }

        public void AddPublishedPost(PublishedPost publishedPost)
        {
            var publishedPostsCollection = CreatePublishedPostsCollection();

            publishedPostsCollection.Save(publishedPost, SafeMode.True);
        }

        private MongoCollection<PublishedPost> CreatePublishedPostsCollection()
        {
            return _mongoDatabase.GetCollection<PublishedPost>("PublishedPosts");
        }
    }
}