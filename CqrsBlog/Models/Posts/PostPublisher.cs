namespace CqrsBlog.Models.Posts
{
    public class PostPublisher : IHandles<PostPublished>
    {
        private readonly PostsRepository _postsRepository;

        private readonly PostsQuery _postsQuery;

        public PostPublisher(PostsRepository postsRepository, PostsQuery postsQuery)
        {
            _postsRepository = postsRepository;
            _postsQuery = postsQuery;
        }

        public void Handle(PostPublished @event)
        {
            var post = _postsRepository.Find(@event.AggregateRootId);

            var publishedPost = new PublishedPost
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                PublishedOn = post.PublishedOn
            };

            _postsQuery.AddPublishedPost(publishedPost);
        }
    }
}