using System.Text.Json.Serialization;

namespace Backend.Models
{
    public class User
    {
        public int ID { get; set; }

        public string UserName { get; set; } = "";

        public string Name { get; set; } = "";

        public string Email { get; set; } = "";

        public RoleEnum Role { get; set; }

        [JsonIgnore]
        public string? Password { get; set; }

        public DateTime MemberSince { get; set; } = DateTime.UtcNow;

        public List<AuctionItem> AddedItemsList { get; set; } = new();

        public List<AuctionItem> WonItemsList { get; set; } = new();

        public List<Bid> Bids { get; set; } = new();

        public List<WishlistEntry> Wishlist { get; set; } = new();

        public List<Review> ReviewList { get; set; } = new();
    }
}
