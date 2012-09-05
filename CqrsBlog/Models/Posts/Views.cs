namespace CqrsBlog.Models.Posts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoDB.Driver.Builders;

    public class UnapprovedCommentsView : IHandles<CommentAdded>, IHandles<CommentApproved>, IHandles<CommentDisapproved>
    {
        private readonly MongoDatabase _mongoDatabase;

        public UnapprovedCommentsView(MongoDatabase mongoDatabase)
        {
            _mongoDatabase = mongoDatabase;
        }

        public IEnumerable<UnapprovedComment> FindAll()
        {
            var collection = CreateCollection();

            var query = Query.LTE("AddedOn", DateTime.UtcNow);

            var unapprovedComments = collection.Find(query)
                .SetSortOrder(SortBy.Descending("AddedOn"))
                .ToList();

            return unapprovedComments;
        }

        public void Handle(CommentAdded @event)
        {
            var collection = CreateCollection();

            var newUnapprovedComment = new UnapprovedComment
            {
                Id = @event.CommentId,
                Author = @event.Author,
                Content = @event.Content,
                PostId = @event.AggregateRootId,
                AddedOn = @event.HappenedOn
            };

            collection.Insert(newUnapprovedComment);
        }

        public void Handle(CommentApproved @event)
        {
            var collection = CreateCollection();

            var find = Query.EQ("_id", @event.CommentId);
        
            collection.Remove(find, RemoveFlags.Single);
        }

        public void Handle(CommentDisapproved @event)
        {
            var collection = CreateCollection();

            var find = Query.EQ("_id", @event.CommentId);
        
            collection.Remove(find, RemoveFlags.Single);
        }

        private MongoCollection<UnapprovedComment> CreateCollection()
        {
            var collection = _mongoDatabase.GetCollection<UnapprovedComment>("UnapprovedComments");

            return collection;
        }
    }

    public class PublishedPostWithCommentsView : IHandles<PostPublished>, IHandles<PostUnpublished>, IHandles<CommentApproved>
    {
        private readonly MongoDatabase _mongoDatabase;

        public PublishedPostWithCommentsView(MongoDatabase mongoDatabase)
        {
            _mongoDatabase = mongoDatabase;
        }

        public PublishedPostWithComments Find(Guid postId)
        {
            var collection = CreateCollection();

            var query = Query.LTE("_id", postId);

            var publishedPostWithComments = collection.FindOne(query);

            return publishedPostWithComments;
        }

        public void Handle(PostPublished @event)
        {
            var collection = CreateCollection();

            var postsRepository = new PostsRepository(_mongoDatabase);

            var post = postsRepository.Find(@event.AggregateRootId);

            var publishedPost = new PublishedPostWithComments
            {
                Id = post.Id,
                Content = post.Content,
                PublishedOn = post.PublishedOn,
                Title = post.Title,
                Comments = new List<Comment>()
            };

            collection.Insert(publishedPost);
        }

        public void Handle(PostUnpublished @event)
        {
            var collection = CreateCollection();

            var find = Query.EQ("_id", @event.AggregateRootId);

            collection.Remove(find, RemoveFlags.Single);
        }

        public void Handle(CommentApproved @event)
        {
            var collection = CreateCollection();

            var postsRepository = new PostsRepository(_mongoDatabase);

            var post = postsRepository.Find(@event.AggregateRootId);

            var comment = post.Comments.First(x => x.Id == @event.CommentId);

            var find = Query.EQ("_id", @event.AggregateRootId);

            var update = Update.AddToSet("Comments", comment.ToBsonDocument());

            collection.Update(find, update);
        }

        private MongoCollection<PublishedPostWithComments> CreateCollection()
        {
            var collection = _mongoDatabase.GetCollection<PublishedPostWithComments>("PublishedPostsWithComments");

            return collection;
        }
    }

    public class PublishedPostsView : IHandles<PostPublished>, IHandles<PostUnpublished>
    {
        private readonly MongoDatabase _mongoDatabase;

        public PublishedPostsView(MongoDatabase mongoDatabase)
        {
            _mongoDatabase = mongoDatabase;
        }

        public IEnumerable<PublishedPost> FindAll()
        {
            var collection = CreateCollection();

            var query = Query.LTE("PublishedOn", DateTime.UtcNow);

            var publishedPosts = collection.Find(query)
                .SetSortOrder(SortBy.Descending("PublishedOn"))
                .ToList();

            return publishedPosts;
        }

        public void Handle(PostPublished @event)
        {
            var collection = CreateCollection();

            var postsRepository = new PostsRepository(_mongoDatabase);

            var post = postsRepository.Find(@event.AggregateRootId);

            var publishedPost = new PublishedPost
            {
                Id = post.Id,
                Content = post.Content,
                PublishedOn = post.PublishedOn,
                Title = post.Title
            };

            collection.Insert(publishedPost);
        }

        public void Handle(PostUnpublished @event)
        {
            var collection = CreateCollection();

            var find = Query.EQ("_id", @event.AggregateRootId);

            collection.Remove(find, RemoveFlags.Single);
        }

        private MongoCollection<PublishedPost> CreateCollection()
        {
            var collection = _mongoDatabase.GetCollection<PublishedPost>("PublishedPosts");

            return collection;
        }
    }

    public class AdminPostsView : IHandles<PostCreated>, IHandles<PostDeleted>, IHandles<PostPublished>, IHandles<PostUnpublished>
    {
        private readonly MongoDatabase _mongoDatabase;

        public AdminPostsView(MongoDatabase mongoDatabase)
        {
            _mongoDatabase = mongoDatabase;
        }

        public IEnumerable<AdminPost> FindAll()
        {
            var collection = CreateCollection();

            var query = Query.LTE("CreatedOn", DateTime.UtcNow);

            var adminPosts = collection.Find(query)
                .SetSortOrder(SortBy.Descending("CreatedOn"))
                .ToList();

            return adminPosts;
        }

        public void Handle(PostCreated @event)
        {
            var collection = CreateCollection();

            var newAdminPost = new AdminPost
            {
                Id = @event.AggregateRootId,
                Content = @event.Content,
                Title = @event.Title,
                CreatedOn = @event.CreatedOn
            };

            collection.Insert(newAdminPost);
        }

        public void Handle(PostDeleted @event)
        {
            var collection = CreateCollection();

            var find = Query.EQ("_id", @event.AggregateRootId);

            collection.Remove(find, RemoveFlags.Single);
        }

        public void Handle(PostPublished @event)
        {
            var collection = CreateCollection();

            var find = Query.EQ("_id", @event.AggregateRootId);
            var update = Update.Set("PublishedOn", @event.PublishedOn)
                .Set("IsPublished", true);

            collection.Update(find, update);
        }

        public void Handle(PostUnpublished @event)
        {
            var collection = CreateCollection();

            var find = Query.EQ("_id", @event.AggregateRootId);
            var update = Update.Set("PublishedOn", DateTime.MinValue)
                .Set("IsPublished", false);

            collection.Update(find, update);
        }

        private MongoCollection<AdminPost> CreateCollection()
        {
            var collection = _mongoDatabase.GetCollection<AdminPost>("AdminPosts");

            return collection;
        }
    }
}