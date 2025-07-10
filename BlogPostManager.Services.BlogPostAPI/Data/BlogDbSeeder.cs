using BlogPostManager.Services.BlogPostAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogPostManager.Services.BlogPostAPI.Data
{
    public class BlogDbSeeder
    {
        public static async Task SeedSamplePostsAsync(BlogDbContext context)
        {
            if (await context.Posts.AnyAsync())
            {
                // Already has data, skip seeding
                return;
            }

            var posts = new List<Post>
            {
                new Post { Title = "Welcome to BlogPostAPI", Content = "This is the first sample post.", AuthorId = "admin-fixed-guid" },
                new Post { Title = "Second Post", Content = "Here is some more sample content.", AuthorId = "admin-fixed-guid" },
                new Post { Title = "Third Post", Content = "BlogPostAPI is running smoothly!", AuthorId = "admin-fixed-guid" },
                new Post { Title = "Fourth Post", Content = "Enjoy creating your blog posts.", AuthorId = "admin-fixed-guid" }
            };

            context.Posts.AddRange(posts);
            await context.SaveChangesAsync();
        }
    }
}
