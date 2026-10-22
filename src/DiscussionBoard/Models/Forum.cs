using System.ComponentModel.DataAnnotations;

namespace DiscussionBoard.Models
{
    public class Forum : EntityBase
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = String.Empty;

        [StringLength(500)]
        public string Description { get; set; } = String.Empty;

        //public DiscussionThread? DiscussionThreadId { get; set; }

        public ICollection<DiscussionThread> DiscussionThreads  { get; set; } = new List<DiscussionThread>();
    }
}
