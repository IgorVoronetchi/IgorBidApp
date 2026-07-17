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
    [Route("api/[controller]")]
    public class UserController(ApplicationDbContext db, TokenProvider tokenProvider) : ControllerBase
    {
        [EnableRateLimiting("auth")]
        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterDto request)
        {
            if (string.IsNullOrWhiteSpace(request.UserName) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Completeaza toate campurile obligatorii.");

            if (await db.Users.AnyAsync(u => u.UserName == request.UserName))
                return BadRequest("Acest username este deja folosit.");

            if (await db.Users.AnyAsync(u => u.Email == request.Email))
                return BadRequest("Exista deja un cont cu acest email.");

            var user = new User
            {
                UserName = request.UserName,
                Name = string.IsNullOrWhiteSpace(request.Name) ? request.UserName : request.Name,
                Email = request.Email,
                Password = PasswordHasher.HashPassword(request.Password),
                Role = RoleEnum.User
            };

            db.Users.Add(user);
            await db.SaveChangesAsync();

            var token = tokenProvider.Create(user);
            return Ok(new { token, user = PublicUser(user) });
        }

        [EnableRateLimiting("auth")]
        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginDto request)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || user.Password == null ||
                !PasswordHasher.VerifyPassword(request.Password, user.Password))
                return Unauthorized("Email sau parola incorecta.");

            var token = tokenProvider.Create(user);
            return Ok(new { token, user = PublicUser(user) });
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult> Me()
        {
            int userId = int.Parse(User.FindFirst("id")!.Value);

            var user = await db.Users
                .Include(u => u.AddedItemsList).ThenInclude(i => i.Category)
                .Include(u => u.AddedItemsList).ThenInclude(i => i.Bids)
                .Include(u => u.WonItemsList).ThenInclude(i => i.Category)
                .Include(u => u.WonItemsList).ThenInclude(i => i.Bids)
                .Include(u => u.Wishlist).ThenInclude(w => w.Item)!.ThenInclude(i => i!.Category)
                .Include(u => u.Wishlist).ThenInclude(w => w.Item)!.ThenInclude(i => i!.Bids)
                .Include(u => u.ReviewList).ThenInclude(r => r.Reviewer)
                .FirstOrDefaultAsync(u => u.ID == userId);

            if (user == null) return NotFound();

            return Ok(new
            {
                id = user.ID,
                userName = user.UserName,
                name = user.Name,
                email = user.Email,
                role = user.Role.ToString(),
                memberSince = user.MemberSince,
                rating = user.Rating(),
                addedItems = user.AddedItemsList.Select(Mappers.ToListDto),
                wonItems = user.WonItemsList.Select(Mappers.ToListDto),
                wishlistItems = user.Wishlist
                    .Where(w => w.Item != null)
                    .Select(w => Mappers.ToListDto(w.Item!)),
                reviews = user.ReviewList
                    .OrderByDescending(r => r.CreatedAt)
                    .Select(r => new
                    {
                        id = r.ID,
                        reviewerUserName = r.Reviewer?.UserName ?? "",
                        rating = r.Rating,
                        comment = r.Comment,
                        createdAt = r.CreatedAt
                    })
            });
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetUser(int id)
        {
            var user = await db.Users
                .Include(u => u.AddedItemsList)
                .Include(u => u.ReviewList).ThenInclude(r => r.Reviewer)
                .FirstOrDefaultAsync(u => u.ID == id);

            if (user == null) return NotFound();

            return Ok(new
            {
                id = user.ID,
                userName = user.UserName,
                name = user.Name,
                memberSince = user.MemberSince,
                rating = user.Rating(),
                completedSales = user.AddedItemsList.Count(i => i.Status == StatusEnum.Sold),
                reviews = user.ReviewList
                    .OrderByDescending(r => r.CreatedAt)
                    .Select(r => new
                    {
                        id = r.ID,
                        reviewerUserName = r.Reviewer?.UserName ?? "",
                        rating = r.Rating,
                        comment = r.Comment,
                        createdAt = r.CreatedAt
                    })
            });
        }

        private static object PublicUser(User u) => new
        {
            id = u.ID,
            userName = u.UserName,
            name = u.Name,
            email = u.Email,
            role = u.Role.ToString()
        };
    }
}
