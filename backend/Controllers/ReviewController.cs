using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/review")]
    public class ReviewController(ApplicationDbContext db) : ControllerBase
    {
        [HttpGet("stats")]
        public async Task<ActionResult> GetStats()
        {
            var ratings = await db.Reviews.Select(r => r.Rating).ToListAsync();
            return Ok(new
            {
                avgRating = ratings.Count == 0 ? (double?)null : Math.Round(ratings.Average(), 1),
                count = ratings.Count
            });
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> AddReview(CreateReviewDto dto)
        {
            int userId = int.Parse(User.FindFirst("id")!.Value);

            if (dto.ReviewedUserId == userId)
                return BadRequest("Nu iti poti lasa recenzie singur.");
            if (dto.Rating < 0 || dto.Rating > 5)
                return BadRequest("Nota trebuie sa fie intre 0 si 5.");
            if (dto.Comment.Length > 1000)
                return BadRequest("Comentariul nu poate depasi 1000 de caractere.");
            if (!await db.Users.AnyAsync(u => u.ID == dto.ReviewedUserId))
                return NotFound("Utilizatorul nu exista.");

            var review = new Review
            {
                ReviewerId = userId,
                ReviewedUserId = dto.ReviewedUserId,
                Rating = dto.Rating,
                Comment = dto.Comment
            };

            db.Reviews.Add(review);
            await db.SaveChangesAsync();
            return Ok(new { id = review.ID });
        }
    }
}
