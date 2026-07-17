using System.Text.Json.Serialization;

namespace Backend.Models
{
    public class ForumComment
    {
        public int ID { get; set; }

        [JsonIgnore]
        public ForumPost? Post { get; set; }

        public int PostId { get; set; }

        [JsonIgnore]
        public User? Author { get; set; }

        public int AuthorId { get; set; }

        public string Text { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
