namespace CqrsBlog.ViewModels.Posts
{
    using System.ComponentModel.DataAnnotations;

    using CqrsBlog.Models.Posts;

    public class DisplayPublishedPostViewModel
    {
        public PublishedPostWithComments Post { get; set; }

        public AddCommentViewModel NewComment { get; set; }

        public class AddCommentViewModel
        {
            [Required]
            public string Author { get; set; }

            [Required]
            [DataType(DataType.MultilineText)]
            public string Comment { get; set; }
        }
    }
}