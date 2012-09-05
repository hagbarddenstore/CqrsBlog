namespace CqrsBlog
{
    using System.Web.Mvc;
    using System.Web.Routing;

    using CqrsBlog.Models;
    using CqrsBlog.Models.Posts;

    using MongoDB.Bson.Serialization;
    using MongoDB.Driver;

    public class MvcApplication : System.Web.HttpApplication
    {
        public static readonly MongoServer MongoServer = MongoServer.Create("mongodb://localhost");

        public static readonly MongoDatabase MongoDatabase = MongoServer.GetDatabase("CqrsBlog");

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            
            routes.MapRoute("Posts.DisplayPublishedPosts", string.Empty, new { controller = "Posts", action = "DisplayPublishedPosts" });
            routes.MapRoute("Posts.DisplayPublishedPost", "post/{postId}", new { controller = "Posts", action = "DisplayPublishedPost" });
            routes.MapRoute("Posts.AddCommentToPublishedPost", "post/{postId}/add-comment", new { controller = "Posts", action = "AddCommentToPublishedPost" });

            routes.MapRoute("Admin.Posts", "admin", new { controller = "Admin", action = "Posts" });
            routes.MapRoute("Admin.Comments", "admin/comments", new { controller = "Admin", action = "Comments" });
            routes.MapRoute("Admin.ApproveComment", "admin/approve-comment/{postId}/{commentId}", new { controller = "Admin", action = "ApproveComment" });
            routes.MapRoute("Admin.DisapproveComment", "admin/disapprove-comment/{postId}/{commentId}", new { controller = "Admin", action = "DisapproveComment" });
            routes.MapRoute("Admin.PublishPost", "admin/publish-post", new { controller = "Admin", action = "PublishPost" });
            routes.MapRoute("Admin.UnpublishPost", "admin/unpublis-post", new { controller = "Admin", action = "UnpublishPost" });
            routes.MapRoute("Admin.CreatePost", "admin/create-post", new { controller = "Admin", action = "CreatePost" });
            routes.MapRoute("Admin.DeletePost", "admin/delete-post", new { controller = "Admin", action = "DeletePost" });
        }

        public static void RegisterDomainEventHandlers()
        {
            DomainEvents.RegisterHandler(() => new UnapprovedCommentsView(MongoDatabase));
            DomainEvents.RegisterHandler(() => new PublishedPostWithCommentsView(MongoDatabase));
            DomainEvents.RegisterHandler(() => new PublishedPostsView(MongoDatabase));
            DomainEvents.RegisterHandler(() => new AdminPostsView(MongoDatabase));
        }

        protected void Application_Start()
        {
            BsonClassMap.RegisterClassMap<PostCreated>();
            BsonClassMap.RegisterClassMap<PostDeleted>();
            BsonClassMap.RegisterClassMap<PostPublished>();
            BsonClassMap.RegisterClassMap<PostUnpublished>();
            BsonClassMap.RegisterClassMap<CommentAdded>();
            BsonClassMap.RegisterClassMap<CommentApproved>();
            BsonClassMap.RegisterClassMap<CommentDisapproved>();

            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            RegisterDomainEventHandlers();
        }
    }
}