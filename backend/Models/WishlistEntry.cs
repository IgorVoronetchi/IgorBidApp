using System.Text.Json.Serialization;

namespace Backend.Models
{
    public class WishlistEntry
    {
        public int ID { get; set; }

        [JsonIgnore]
        public User? User { get; set; }

        public int UserId { get; set; }

        [JsonIgnore]
        public AuctionItem? Item { get; set; }

        public int ItemId { get; set; }
    }
}
