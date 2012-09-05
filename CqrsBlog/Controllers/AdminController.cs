namespace CqrsBlog.Controllers
{
    using System;
    using System.Web.Mvc;

    using CqrsBlog.Models.Posts;
    using CqrsBlog.ViewModels.Admin;

    public class AdminController : Controller
    {
        private readonly PostsRepository _postsRepository = new PostsRepository(MvcApplication.MongoDatabase);

        public ActionResult Posts()
        {
            var view = new AdminPostsView(MvcApplication.MongoDatabase);

            var adminPosts = view.FindAll();

            var model = new PostsViewModel
            {
                Posts = adminPosts
            };

            return View(model);
        }

        public ActionResult Comments()
        {
            var view = new UnapprovedCommentsView(MvcApplication.MongoDatabase);

            var unapprovedComments = view.FindAll();

            var model = new CommentsViewModel
            {
                Comments = unapprovedComments
            };

            return View(model);
        }

        public ActionResult ApproveComment(Guid postId, Guid commentId)
        {
            var post = _postsRepository.Find(postId);

            post.ApproveComment(commentId);

            _postsRepository.Save(post);

            return RedirectToRoute("Admin.Comments");
        }

        public ActionResult DisapproveComment(Guid postId, Guid commentId)
        {
            var post = _postsRepository.Find(postId);

            post.DisapproveComment(commentId);

            _postsRepository.Save(post);

            return RedirectToRoute("Admin.Comments");
        }

        public ActionResult PublishPost(Guid postId)
        {
            var post = _postsRepository.Find(postId);

            post.Publish();

            _postsRepository.Save(post);

            return RedirectToRoute("Admin.Posts");
        }

        public ActionResult UnpublishPost(Guid postId)
        {
            var post = _postsRepository.Find(postId);

            post.Unpublish();

            _postsRepository.Save(post);

            return RedirectToRoute("Admin.Posts");
        }

        public ActionResult CreatePost()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreatePost(CreatePostViewModel model)
        {
            if (ModelState.IsValid)
            {
                var post = new Post(model.Title, model.Content);

                _postsRepository.Save(post);

                return RedirectToRoute("Admin.Posts");
            }

            return View();
        }

        public ActionResult DeletePost()
        {
            // Find post and call Delete()
            return RedirectToRoute("Admin.Posts");
        }
    }
}