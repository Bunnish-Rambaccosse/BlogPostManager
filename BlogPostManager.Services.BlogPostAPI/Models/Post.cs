namespace BlogPostManager.Services.BlogPostAPI.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string AuthorId { get; set; } = string.Empty; // matches Identity user
        public string AuthorName { get; set; } = string.Empty ;
        public string? Tags { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
