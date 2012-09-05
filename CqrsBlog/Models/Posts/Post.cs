namespace CqrsBlog.Models.Posts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Post : AggregateRoot
    {
        private readonly Guid _id;

        private readonly IList<Comment> _comments = new List<Comment>();

        public Post(Guid postId, IEnumerable<DomainEvent> events)
        {
            _id = postId;

            Replay(events);
        }

        public Post(string title, string content)
        {
            var @event = new PostCreated(title, content, DateTime.UtcNow);

            Apply(@event);

            AppendEvent(@event);
        }

        public override Guid Id
        {
            get { return _id; }
        }

        public string Title { get; private set; }

        public string Content { get; private set; }

        public DateTime CreatedOn { get; private set; }

        public bool IsPublished { get; private set; }

        public DateTime PublishedOn { get; private set; }

        public IEnumerable<Comment> Comments
        {
            get { return _comments; }
        }

        public void Publish()
        {
            var @event = new PostPublished(DateTime.UtcNow);

            Apply(@event);

            AppendEvent(@event);
        }

        public void Unpublish()
        {
            var @event = new PostUnpublished();

            Apply(@event);

            AppendEvent(@event);
        }

        public void AddComment(string author, string content)
        {
            var commentId = Guid.NewGuid();
            var @event = new CommentAdded(commentId, author, content);

            Apply(@event);

            AppendEvent(@event);
        }

        public void ApproveComment(Guid commentId)
        {
            var @event = new CommentApproved(commentId);

            Apply(@event);

            AppendEvent(@event);
        }

        public void DisapproveComment(Guid commentId)
        {
            var @event = new CommentDisapproved(commentId);

            Apply(@event);

            AppendEvent(@event);
        }

        public void Apply(PostCreated @event)
        {
            Title = @event.Title;
            Content = @event.Content;
            CreatedOn = @event.CreatedOn;
        }

        public void Apply(PostDeleted @event)
        {
            // TODO: How the fuck do we handle this?!
        }

        public void Apply(PostPublished @event)
        {
            IsPublished = true;
            PublishedOn = @event.PublishedOn;
        }

        public void Apply(PostUnpublished @event)
        {
            IsPublished = false;
            PublishedOn = default(DateTime);
        }

        public void Apply(CommentAdded @event)
        {
            _comments.Add(new Comment
            {
                Id = @event.CommentId,
                Approved = false,
                Author = @event.Author,
                Content = @event.Content
            });
        }

        public void Apply(CommentApproved @event)
        {
            var comment = _comments.First(x => x.Id == @event.CommentId);

            comment.Approved = true;
        }

        public void Apply(CommentDisapproved @event)
        {
            var comment = _comments.First(x => x.Id == @event.CommentId);

            comment.Approved = false;
        }
    }
}