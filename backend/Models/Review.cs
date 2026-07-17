using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Backend.Models
{
    public class Review
    {
        public int ID { get; set; }

        [JsonIgnore]
        public User? Reviewer { get; set; }

        public int ReviewerId { get; set; }

        [JsonIgnore]
        public User? ReviewedUser { get; set; }

        public int ReviewedUserId { get; set; }

        [Range(0, 5)]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string Comment { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
