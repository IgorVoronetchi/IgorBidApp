using System.Text.Json.Serialization;

namespace Backend.Models
{
    public class CategoryItem
    {
        public int ID { get; set; }

        public string Name { get; set; } = "";

        [JsonIgnore]
        public List<AuctionItem> Items { get; set; } = new();
    }
}
