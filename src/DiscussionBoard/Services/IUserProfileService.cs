namespace DiscussionBoard.Services
{
    /// <summary>
    /// Defines the contract for services that validate user profile information in
    /// the DiscussionBoard application.
    /// </summary>
    public interface IUserProfileService
    {
        /// <summary>
        /// Determines whether a display name is valid and
        /// available for registration or profile updates.
        /// </summary>
        /// <param name="displayName">
        /// The display name to check for availability.
        /// </param>
        /// <param name="excludeUserId">
        /// An optional user ID to exclude from the duplicate check,
        /// allowing existing users to retain their own display name.
        /// </param>
        /// <returns>
        /// A task containing true if the display name is valid
        /// and available; otherwise, false.
        /// </returns>
        Task<bool> IsDisplayNameAvailableAsync(string displayName, string? excludeUserId = null);
    }
}
