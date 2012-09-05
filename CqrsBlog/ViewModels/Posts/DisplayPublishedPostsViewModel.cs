namespace CqrsBlog.ViewModels.Posts
{
    using System.Collections.Generic;

    using CqrsBlog.Models.Posts;

    public class DisplayPublishedPostsViewModel
    {
        public IEnumerable<PublishedPost> Posts { get; set; }
    }
}