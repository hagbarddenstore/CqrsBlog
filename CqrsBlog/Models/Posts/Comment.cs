namespace CqrsBlog.Models.Posts
{
    using System;

    public class Comment
    {
        public Guid Id { get; set; }

        public string Author { get; set; }

        public string Content { get; set; }

        public bool Approved { get; set; }
    }
}