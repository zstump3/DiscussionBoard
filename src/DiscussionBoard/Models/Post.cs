namespace DiscussionBoard.Models
{
    public class Post : DiscussionBase
    {
        //public Post? Replies { get; set; }
        public ICollection<Post> Replies { get; set; } = new List<Post>();

        public ICollection<Post> ParentPosts { get; set; } = new List<Post>();

        public Post? ParentPost { get; set; } = null!;


    }
}
