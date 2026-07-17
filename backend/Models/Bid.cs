using System.Text.Json.Serialization;

namespace Backend.Models
{
    public class Bid
    {
        public int ID { get; set; }

        [JsonIgnore]
        public AuctionItem? Item { get; set; }

        public int ItemId { get; set; }

        [JsonIgnore]
        public User? Bidder { get; set; }

        public int BidderId { get; set; }

        public decimal Amount { get; set; }

        public DateTime PlacedAt { get; set; } = DateTime.UtcNow;
    }
}
