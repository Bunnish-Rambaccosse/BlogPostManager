using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using BlogPostManager.Services.BlogPostAPI.Controllers;
using BlogPostManager.Services.BlogPostAPI.Data;
using BlogPostManager.Services.BlogPostAPI.Models;

namespace BlogAPI.Tests
{
    public  class BlogPostControllerTests
    {
        private readonly BlogDbContext _dbContext;
        private readonly BlogPostController _controller;

        public BlogPostControllerTests()
        {
            // Use a unique in-memory DB to avoid test pollution
            var options = new DbContextOptionsBuilder<BlogDbContext>()
                .UseInMemoryDatabase(databaseName: $"BlogDb_{System.Guid.NewGuid()}")
                .Options;

            _dbContext = new BlogDbContext(options);

            // Seed posts
            _dbContext.Posts.AddRange(
                new Post { Id = 1, Title = "First Post", Content = "Content 1", AuthorId = "user1" },
                new Post { Id = 2, Title = "Second Post", Content = "Content 2", AuthorId = "user2" }
            );
            _dbContext.SaveChanges();

            var logger = new Mock<ILogger<BlogPostController>>();
            _controller = new BlogPostController(_dbContext, logger.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WithAllPosts()
        {
            var result = await _controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var posts = Assert.IsType<List<Post>>(okResult.Value);

            Assert.Equal(2, posts.Count);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenPostMissing()
        {
            var result = await _controller.GetById(999); // non-existent

            Assert.IsType<NotFoundResult>(result);
        }

    }
}
