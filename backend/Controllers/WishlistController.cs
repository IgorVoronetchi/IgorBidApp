using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/wishlist")]
    [Authorize]
    public class WishlistController(ApplicationDbContext db) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult> GetWishlist()
        {
            int userId = int.Parse(User.FindFirst("id")!.Value);
            var ids = await db.WishlistEntries
                .Where(w => w.UserId == userId)
                .Select(w => w.ItemId)
                .ToListAsync();
            return Ok(ids);
        }

        [HttpPost("{itemId:int}")]
        public async Task<ActionResult> Toggle(int itemId)
        {
            int userId = int.Parse(User.FindFirst("id")!.Value);

            if (!await db.AuctionItems.AnyAsync(i => i.ID == itemId))
                return NotFound("Obiectul nu exista.");

            var entry = await db.WishlistEntries
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ItemId == itemId);

            bool inWishlist;
            if (entry != null)
            {
                db.WishlistEntries.Remove(entry);
                inWishlist = false;
            }
            else
            {
                db.WishlistEntries.Add(new WishlistEntry { UserId = userId, ItemId = itemId });
                inWishlist = true;
            }

            await db.SaveChangesAsync();
            return Ok(new { itemId, inWishlist });
        }
    }
}
