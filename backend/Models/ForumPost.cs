using System.Text.Json.Serialization;

namespace Backend.Models
{
    public class ForumPost
    {
        public int ID { get; set; }

        public string Title { get; set; } = "";

        public string Description { get; set; } = "";

        [JsonIgnore]
        public User? Author { get; set; }

        public int AuthorId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<ForumComment> Comments { get; set; } = new();
    }
}
