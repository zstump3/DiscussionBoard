namespace DiscussionBoard.Models
{
    public class DiscussionThread : DiscussionBase
    {
        public Post? PostsId { get; set; }
        public ICollection<Post>  Posts { get; set; } = new List<Post>();
    }
}
