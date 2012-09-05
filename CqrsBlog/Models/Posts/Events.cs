namespace CqrsBlog.Models.Posts
{
    using System;

    public class PostCreated : DomainEvent
    {
        public PostCreated(string title, string content, DateTime createdOn)
        {
            Title = title;
            Content = content;
            CreatedOn = createdOn;
        }

        public string Title { get; private set; }

        public string Content { get; private set; }

        public DateTime CreatedOn { get; private set; }
    }

    public class PostDeleted : DomainEvent
    {
    }

    public class PostPublished : DomainEvent
    {
        public PostPublished(DateTime publishedOn)
        {
            PublishedOn = publishedOn;
        }

        public DateTime PublishedOn { get; private set; }
    }

    public class PostUnpublished : DomainEvent
    {
    }

    public class CommentAdded : DomainEvent
    {
        public CommentAdded(Guid commentId, string author, string content)
        {
            CommentId = commentId;
            Author = author;
            Content = content;
        }

        public Guid CommentId { get; private set; }

        public string Author { get; private set; }

        public string Content { get; private set; }
    }

    public class CommentApproved : DomainEvent
    {
        public CommentApproved(Guid commentId)
        {
            CommentId = commentId;
        }

        public Guid CommentId { get; private set; }
    }

    public class CommentDisapproved : DomainEvent
    {
        public CommentDisapproved(Guid commentId)
        {
            CommentId = commentId;
        }

        public Guid CommentId { get; private set; }
    }
}