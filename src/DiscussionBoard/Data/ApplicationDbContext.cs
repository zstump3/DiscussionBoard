using DiscussionBoard.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DiscussionBoard.Data
{
    /// <summary>
    /// Represents the database context for the DiscussionBoard application. Manages forums, discussions
    /// threads, posts, and Identity user data. Configures entity relationships using the entity
    /// framework core.
    /// </summary>
    /// <param name="options">
    ///Configuration options used to initialize the database context.
    /// </param>
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        /// <summary>
        /// Gets or sets the collection of forums stored in the database.
        /// </summary>
        public DbSet<Forum> Forums { get; set; }

        /// <summary>
        /// Gets or sets the collection of discussion threads stored in the database.
        /// </summary>
        public DbSet<DiscussionThread> DiscussionThreads { get; set; }

        /// <summary>
        /// Gets or sets the collection of posts and replies stored in the database.
        /// </summary>
        public DbSet<Post> Posts { get; set; }

        /// <summary>
        /// Configures the relationships between the application's
        /// database entities using the Entity Framework Core Fluent API.
        /// </summary>
        /// <param name="builder">
        /// The model builder used to configure entity relationships.
        /// </param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //One forum can contain multiple discussion threads. Each
            //discussion thread belongs to one forum
            builder.Entity<Forum>()
                .HasMany(f => f.DiscussionThreads)
                .WithOne(d => d.Forum)
                .HasForeignKey(d => d.ForumId);

            //One discussion thread can contain multiple posts. Each post
            //belongs to one discussion thread.
            builder.Entity<DiscussionThread>()
                .HasMany(d => d.Posts)
                .WithOne(p => p.DiscussionThread)
                .HasForeignKey(p => p.DiscussionThreadId);

            //A post can have multiple replies from other posts. Each reply can have
            //one parent post or no parent.
            builder.Entity<Post>()
                .HasMany(p => p.Replies)
                .WithOne(p => p.ParentPost)
                .HasForeignKey(p => p.ParentPostId);

            //One user can create multiple discussion threads. Each discussion thread is
            //associated with one user
            builder.Entity<ApplicationUser>()
                .HasMany(a => a.DiscussionThreads)
                .WithOne(d => d.ApplicationUser)
                .HasForeignKey(d => d.ApplicationUserId);

            //One user can create multiple posts and replies. Each post or reply
            //is associated with one user.
            builder.Entity<ApplicationUser>()
                .HasMany(a => a.Posts)
                .WithOne(p => p.ApplicationUser)
                .HasForeignKey(p => p.ApplicationUserId);
        }
    }
}
