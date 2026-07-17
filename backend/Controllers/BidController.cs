using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/bid")]
    public class BidController(ApplicationDbContext db) : ControllerBase
    {
        [Authorize]
        [EnableRateLimiting("bid")]
        [HttpPost]
        public async Task<ActionResult> PlaceBid(PlaceBidDto dto)
        {
            int userId = int.Parse(User.FindFirst("id")!.Value);

            await AuctionLifecycleService.RunTransitionsAsync(db);

            var item = await db.AuctionItems
                .Include(i => i.Bids)
                .FirstOrDefaultAsync(i => i.ID == dto.ItemId);

            if (item == null) return NotFound("Obiectul nu exista.");

            if (item.Status != StatusEnum.ActiveBid)
                return BadRequest("Licitatia nu este activa.");

            if (item.OwnerId == userId)
                return BadRequest("Nu poti licita pe propriul tau obiect.");

            var minimum = item.Bids.Count == 0 ? item.StartPrice : item.CurrentPrice;
            if (item.Bids.Count == 0 ? dto.Amount < minimum : dto.Amount <= minimum)
                return BadRequest($"Oferta trebuie sa fie mai mare decat {minimum:0.##} lei.");

            var bid = new Bid
            {
                ItemId = item.ID,
                BidderId = userId,
                Amount = dto.Amount,
                PlacedAt = DateTime.UtcNow
            };

            item.CurrentPrice = dto.Amount;
            db.Bids.Add(bid);
            await db.SaveChangesAsync();

            return Ok(new
            {
                itemId = item.ID,
                currentPrice = item.CurrentPrice,
                bidCount = item.Bids.Count
            });
        }
    }
}
