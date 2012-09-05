namespace CqrsBlog.Controllers
{
    using System;
    using System.Web.Mvc;

    using CqrsBlog.Models.Posts;
    using CqrsBlog.ViewModels.Posts;

    public class PostsController : Controller
    {
        public ActionResult DisplayPublishedPosts()
        {
            var view = new PublishedPostsView(MvcApplication.MongoDatabase);

            var publishedPosts = view.FindAll();

            var model = new DisplayPublishedPostsViewModel
            {
                Posts = publishedPosts
            };

            return View(model);
        }

        public ActionResult DisplayPublishedPost(Guid postId)
        {
            var view = new PublishedPostWithCommentsView(MvcApplication.MongoDatabase);

            var publishedPostWithComments = view.Find(postId);

            var model = new DisplayPublishedPostViewModel
            {
                Post = publishedPostWithComments
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult AddCommentToPublishedPost(Guid postId, DisplayPublishedPostViewModel model)
        {
            if (ModelState.IsValid)
            {
                var postsRepository = new PostsRepository(MvcApplication.MongoDatabase);

                var post = postsRepository.Find(postId);

                post.AddComment(model.NewComment.Author, model.NewComment.Comment);

                postsRepository.Save(post);
            }

            return RedirectToRoute("Posts.DisplayPublishedPost", new { postId });
        }
    }
}