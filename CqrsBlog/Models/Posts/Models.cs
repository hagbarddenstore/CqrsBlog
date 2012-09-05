namespace CqrsBlog.Models.Posts
{
    using System;
    using System.Collections.Generic;

    public class AdminPost
    {
        public Guid Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime PublishedOn { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public bool IsPublished { get; set; }
    }

    public class PublishedPost
    {
        public Guid Id { get; set; }

        public DateTime PublishedOn { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }
    }

    public class PublishedPostWithComments
    {
        public Guid Id { get; set; }

        public DateTime PublishedOn { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public List<Comment> Comments { get; set; }
    }

    public class UnapprovedComment
    {
        public Guid Id { get; set; }

        public Guid PostId { get; set; }

        public string Author { get; set; }

        public string Content { get; set; }

        public DateTime AddedOn { get; set; }
    }
}