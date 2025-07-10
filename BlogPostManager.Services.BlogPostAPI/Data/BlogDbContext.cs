using BlogPostManager.Services.BlogPostAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogPostManager.Services.BlogPostAPI.Data
{
    public class BlogDbContext : DbContext
    {
        public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options) { }

        public DbSet<Post> Posts { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
