using DiscussionBoard.Models;

namespace DiscussionBoard.Services
{
    public interface ForumService
    {
        public bool GetAllAsync()
        {
            if (IsDeleted() == false)
            {

            }

            return true;
        }
    }
}
