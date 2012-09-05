namespace CqrsBlog.ViewModels.Admin
{
    using System.Collections.Generic;

    using CqrsBlog.Models.Posts;

    public class CommentsViewModel
    {
        public IEnumerable<UnapprovedComment> Comments { get; set; }
    }
}