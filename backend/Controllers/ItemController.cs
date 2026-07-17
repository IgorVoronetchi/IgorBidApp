using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/item")]
    public class ItemController(ApplicationDbContext db) : ControllerBase
    {
        /// <summary>Toate item-urile vizibile public (live sau incheiate).</summary>
        [HttpGet]
        public async Task<ActionResult> GetItems()
        {
            await AuctionLifecycleService.RunTransitionsAsync(db);

            var items = await db.AuctionItems
                .Include(i => i.Category)
                .Include(i => i.Owner)
                .Include(i => i.Winner)
                .Include(i => i.Bids)
                .Where(i => i.Status == StatusEnum.ActiveBid ||
                            i.Status == StatusEnum.Sold ||
                            i.Status == StatusEnum.NoWinner)
                .OrderBy(i => i.EndDate)
                .ToListAsync();

            return Ok(items.Select(Mappers.ToListDto));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetItem(int id)
        {
            await AuctionLifecycleService.RunTransitionsAsync(db);

            var item = await db.AuctionItems
                .Include(i => i.Category)
                .Include(i => i.Owner).ThenInclude(o => o!.ReviewList)
                .Include(i => i.Owner).ThenInclude(o => o!.AddedItemsList)
                .Include(i => i.Winner)
                .Include(i => i.Bids).ThenInclude(b => b.Bidder)
                .FirstOrDefaultAsync(i => i.ID == id);

            if (item == null) return NotFound();

            return Ok(Mappers.ToDetailDto(item));
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> AddItem(CreateItemDto dto)
        {
            int userId = int.Parse(User.FindFirst("id")!.Value);

            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Numele obiectului este obligatoriu.");
            if (dto.StartPrice <= 0)
                return BadRequest("Pretul de pornire trebuie sa fie pozitiv.");
            if (dto.EndDate <= DateTime.UtcNow)
                return BadRequest("Data de final trebuie sa fie in viitor.");
            if (!await db.Categories.AnyAsync(c => c.ID == dto.CategoryId))
                return BadRequest("Categorie inexistenta.");
            if (dto.ImageDataUrl != null &&
                (dto.ImageDataUrl.Length > 3_000_000 ||
                 !dto.ImageDataUrl.StartsWith("data:image/jpeg;base64,", StringComparison.Ordinal)))
                return BadRequest("Imagine invalida sau prea mare.");

            var item = new AuctionItem
            {
                Name = dto.Name,
                StartPrice = dto.StartPrice,
                CurrentPrice = dto.StartPrice,
                CategoryId = dto.CategoryId,
                Description = dto.Description,
                Location = dto.Location,
                ImageDataUrl = dto.ImageDataUrl,
                OwnerId = userId,
                Status = StatusEnum.Added,
                StartDate = dto.StartDate ?? DateTime.UtcNow,
                EndDate = dto.EndDate
            };

            db.AuctionItems.Add(item);
            await db.SaveChangesAsync();

            return Ok(new { id = item.ID, status = item.Status.ToString() });
        }

        // ---------- Admin: coada de validare ----------

        [Authorize(Roles = "Admin")]
        [HttpGet("pending")]
        public async Task<ActionResult> GetPending()
        {
            var items = await db.AuctionItems
                .Include(i => i.Category)
                .Include(i => i.Owner)
                .Include(i => i.Bids)
                .Where(i => i.Status == StatusEnum.Added)
                .OrderBy(i => i.ID)
                .ToListAsync();

            return Ok(items.Select(Mappers.ToListDto));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id:int}/approve")]
        public async Task<ActionResult> Approve(int id)
        {
            var item = await db.AuctionItems.FindAsync(id);
            if (item == null) return NotFound();
            if (item.Status != StatusEnum.Added)
                return BadRequest("Obiectul nu este in asteptarea validarii.");

            item.Status = item.StartDate <= DateTime.UtcNow
                ? StatusEnum.ActiveBid
                : StatusEnum.Validated;

            await db.SaveChangesAsync();
            return Ok(new { id = item.ID, status = item.Status.ToString() });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id:int}/reject")]
        public async Task<ActionResult> Reject(int id)
        {
            var item = await db.AuctionItems.FindAsync(id);
            if (item == null) return NotFound();
            if (item.Status != StatusEnum.Added)
                return BadRequest("Obiectul nu este in asteptarea validarii.");

            item.Status = StatusEnum.Rejected;
            await db.SaveChangesAsync();
            return Ok(new { id = item.ID, status = item.Status.ToString() });
        }
    }
}
