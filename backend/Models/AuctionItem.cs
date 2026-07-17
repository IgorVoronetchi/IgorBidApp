using System.Text.Json.Serialization;

namespace Backend.Models
{
    public class AuctionItem
    {
        public int ID { get; set; }

        public string Name { get; set; } = "";

        public decimal StartPrice { get; set; }

        public decimal CurrentPrice { get; set; }

        public CategoryItem? Category { get; set; }

        public int CategoryId { get; set; }

        public string? Description { get; set; }

        public string Location { get; set; } = "";

        /// <summary>Poza listarii, stocata ca data URL (base64) - suficient pentru un demo fara storage extern.</summary>
        public string? ImageDataUrl { get; set; }

        [JsonIgnore]
        public User? Owner { get; set; }

        public int OwnerId { get; set; }

        [JsonIgnore]
        public User? Winner { get; set; }

        public int? WinnerId { get; set; }

        public StatusEnum Status { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public List<Bid> Bids { get; set; } = new();
    }
}
