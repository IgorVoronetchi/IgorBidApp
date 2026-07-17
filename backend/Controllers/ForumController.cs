using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/forum")]
    public class ForumController(ApplicationDbContext db) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult> GetPosts()
        {
            var posts = await db.ForumPosts
                .Include(p => p.Author)
                .Include(p => p.Comments).ThenInclude(c => c.Author)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return Ok(posts.Select(p => new
            {
                id = p.ID,
                title = p.Title,
                description = p.Description,
                authorUserName = p.Author?.UserName ?? "",
                createdAt = p.CreatedAt,
                comments = p.Comments
                    .OrderBy(c => c.CreatedAt)
                    .Select(c => new
                    {
                        id = c.ID,
                        authorUserName = c.Author?.UserName ?? "",
                        text = c.Text,
                        createdAt = c.CreatedAt
                    })
            }));
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> AddPost(CreateForumPostDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Description))
                return BadRequest("Titlul si continutul sunt obligatorii.");

            int userId = int.Parse(User.FindFirst("id")!.Value);
            var post = new ForumPost
            {
                Title = dto.Title,
                Description = dto.Description,
                AuthorId = userId
            };

            db.ForumPosts.Add(post);
            await db.SaveChangesAsync();
            return Ok(new { id = post.ID });
        }

        [Authorize]
        [HttpPost("{postId:int}/comments")]
        public async Task<ActionResult> AddComment(int postId, CreateForumCommentDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Text))
                return BadRequest("Comentariul nu poate fi gol.");

            if (!await db.ForumPosts.AnyAsync(p => p.ID == postId))
                return NotFound("Postarea nu exista.");

            int userId = int.Parse(User.FindFirst("id")!.Value);
            var comment = new ForumComment
            {
                PostId = postId,
                AuthorId = userId,
                Text = dto.Text
            };

            db.ForumComments.Add(comment);
            await db.SaveChangesAsync();
            return Ok(new { id = comment.ID });
        }
    }
}
