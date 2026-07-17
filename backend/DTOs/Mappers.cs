using Backend.Models;

namespace Backend.DTOs
{
    public static class Mappers
    {
        public static object ToListDto(AuctionItem i) => new
        {
            id = i.ID,
            name = i.Name,
            startPrice = i.StartPrice,
            currentPrice = i.CurrentPrice,
            category = i.Category?.Name ?? "",
            categoryId = i.CategoryId,
            description = i.Description,
            location = i.Location,
            imageUrl = i.ImageDataUrl,
            ownerId = i.OwnerId,
            ownerUserName = i.Owner?.UserName ?? "",
            winnerId = i.WinnerId,
            winnerUserName = i.Winner?.UserName,
            status = i.Status.ToString(),
            startDate = i.StartDate,
            endDate = i.EndDate,
            bidCount = i.Bids.Count
        };

        public static object ToDetailDto(AuctionItem i) => new
        {
            id = i.ID,
            name = i.Name,
            startPrice = i.StartPrice,
            currentPrice = i.CurrentPrice,
            category = i.Category?.Name ?? "",
            categoryId = i.CategoryId,
            description = i.Description,
            location = i.Location,
            imageUrl = i.ImageDataUrl,
            ownerId = i.OwnerId,
            ownerUserName = i.Owner?.UserName ?? "",
            ownerRating = i.Owner?.Rating(),
            ownerSales = i.Owner?.AddedItemsList.Count(x => x.Status == StatusEnum.Sold) ?? 0,
            winnerId = i.WinnerId,
            winnerUserName = i.Winner?.UserName,
            status = i.Status.ToString(),
            startDate = i.StartDate,
            endDate = i.EndDate,
            bidCount = i.Bids.Count,
            bids = i.Bids
                .OrderByDescending(b => b.Amount)
                .Select(b => new
                {
                    id = b.ID,
                    bidderId = b.BidderId,
                    bidderUserName = b.Bidder?.UserName ?? "",
                    amount = b.Amount,
                    placedAt = b.PlacedAt
                })
        };

        public static double? Rating(this User u) =>
            u.ReviewList.Count == 0
                ? null
                : Math.Round(u.ReviewList.Average(r => r.Rating), 1);
    }
}
