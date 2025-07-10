using BlogPostManager.Services.BlogPostAPI.Data;
using BlogPostManager.Services.BlogPostAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlogPostManager.Services.BlogPostAPI.Controllers
{
    [Route("api/blogpost")]
    [ApiController]
    public class BlogPostController : ControllerBase
    {
        private readonly BlogDbContext _context;
        private readonly ILogger<BlogPostController> _logger;

        public BlogPostController(BlogDbContext context, ILogger<BlogPostController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Publicly view all posts (no authorization required)
        [HttpGet("all")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Fetching all blog posts");
            try
            {
                var posts = await _context.Posts.ToListAsync();
                _logger.LogInformation("Successfully retrieved {Count} posts", posts.Count);
                return Ok(posts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all posts");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // 2. Get post by id - Require login (frontend can redirect if user is not logged in)
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("Fetching post with ID: {PostId}", id);

            var post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                _logger.LogWarning("Post with ID {PostId} not found", id);
                return NotFound();
            }

            _logger.LogInformation("Post with ID {PostId} retrieved successfully", id);
            return Ok(post);
        }

        // 3. Create post - only logged-in users
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] Post post)
        {
            // Get logged-in user's ID from claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Unauthorized post creation attempt");
                return Unauthorized();
            }

            post.AuthorId = userId;
            post.CreatedAt = DateTime.UtcNow;

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Post created by user {UserId} with ID {PostId}", userId, post.Id);

            return CreatedAtAction(nameof(GetById), new { id = post.Id }, post);
        }

        // 4. Delete post - only the owner can delete
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Unauthorized delete attempt");
                return Unauthorized();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                _logger.LogWarning("Delete failed: Post with ID {PostId} not found", id);
                return NotFound();
            }

            if (post.AuthorId != userId)
            {
                _logger.LogWarning("User {UserId} tried to delete post {PostId} they do not own", userId, id);
                return Forbid("You can only delete your own posts.");
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User {UserId} deleted post with ID {PostId}", userId, id);
            return NoContent();
        }

        // 5. (Bonus) Get posts created by logged-in user
        [HttpGet("myposts")]
        [Authorize]
        public async Task<IActionResult> GetMyPosts()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Unauthorized attempt to access user's posts");
                return Unauthorized();
            }

            _logger.LogInformation("Fetching posts for user {UserId}", userId);

            var myPosts = await _context.Posts
                .Where(p => p.AuthorId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            _logger.LogInformation("User {UserId} retrieved {Count} posts", userId, myPosts.Count);
            return Ok(myPosts);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] Post updatedPost)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Unauthorized update attempt");
                return Unauthorized();
            }

            var existingPost = await _context.Posts.FindAsync(id);
            if (existingPost == null)
            {
                _logger.LogWarning("Update failed: Post with ID {PostId} not found", id);
                return NotFound();
            }

            if (existingPost.AuthorId != userId)
            {
                _logger.LogWarning("User {UserId} tried to update post {PostId} they do not own", userId, id);
                return Forbid("You can only edit your own posts.");
            }

            existingPost.Title = updatedPost.Title;
            existingPost.Content = updatedPost.Content;
            existingPost.Tags = updatedPost.Tags;

            _context.Posts.Update(existingPost);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User {UserId} updated post with ID {PostId}", userId, id);

            return Ok(existingPost);
        }
    }
}
