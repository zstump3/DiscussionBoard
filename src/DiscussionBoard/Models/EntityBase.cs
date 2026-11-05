namespace DiscussionBoard.Models
{
    /// <summary>
    /// Represents the base class for the database entities. Provides a unique identifier and
    /// property used to track whether an entity has been deleted.
    /// </summary>
    public abstract class EntityBase
    {
        /// <summary>
        /// Gets or sets the unique identifier for the entity
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity has been marked as deleted
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}
