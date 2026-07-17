using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs
{
    public class CreateItemDto
    {
        [Required, StringLength(120, MinimumLength = 2)]
        public string Name { get; set; } = "";

        [Range(0.01, 100_000_000)]
        public decimal StartPrice { get; set; }

        public int CategoryId { get; set; }

        [StringLength(2000)]
        public string? Description { get; set; }

        [Required, StringLength(120)]
        public string Location { get; set; } = "";

        public DateTime? StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string? ImageDataUrl { get; set; }
    }

    public class PlaceBidDto
    {
        public int ItemId { get; set; }

        [Range(0.01, 1_000_000_000)]
        public decimal Amount { get; set; }
    }

    public class CreateReviewDto
    {
        public int ReviewedUserId { get; set; }

        [Range(0, 5)]
        public int Rating { get; set; }

        [StringLength(1000)]
        public string Comment { get; set; } = "";
    }

    public class CreateForumPostDto
    {
        [Required, StringLength(150, MinimumLength = 3)]
        public string Title { get; set; } = "";

        [Required, StringLength(4000, MinimumLength = 1)]
        public string Description { get; set; } = "";
    }

    public class CreateForumCommentDto
    {
        [Required, StringLength(1000, MinimumLength = 1)]
        public string Text { get; set; } = "";
    }
}
