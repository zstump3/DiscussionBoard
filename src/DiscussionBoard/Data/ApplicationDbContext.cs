using DiscussionBoard.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DiscussionBoard.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Forum> Forums { get; set; }
        public DbSet<DiscussionThread> DiscussionThreads { get; set; }
        public DbSet<Post> Posts { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Forum>()
                .HasMany(f => f.DiscussionThreads)
                .WithOne(d => d.Forum)
                .HasForeignKey(d => d.ForumId);

            builder.Entity<DiscussionThread>()
                .HasMany(d => d.Posts)
                .WithOne(p => p.DiscussionThread)
                .HasForeignKey(p => p.DiscussionThreadId);

            builder.Entity<Post>()
                .HasMany(p => p.Replies)
                .WithOne(p => p.ParentPost)
                .HasForeignKey(p => p.ParentPostId);

            builder.Entity<ApplicationUser>()
                .HasMany(a => a.DiscussionThreads)
                .WithOne(d => d.ApplicationUser)
                .HasForeignKey(d => d.ApplicationUserId);

            builder.Entity<ApplicationUser>()
                .HasMany(a => a.Posts)
                .WithOne(p => p.ApplicationUser)
                .HasForeignKey(p => p.ApplicationUserId);
        }
    }
}
