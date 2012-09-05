namespace CqrsBlog.ViewModels.Admin
{
    using System.Collections.Generic;

    using CqrsBlog.Models.Posts;

    public class PostsViewModel
    {
        public IEnumerable<AdminPost> Posts { get; set; }
    }
}