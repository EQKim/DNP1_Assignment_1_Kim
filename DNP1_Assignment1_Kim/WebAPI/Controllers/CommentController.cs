using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CommentController(AppDbContext context)
        {
            _context = context;
        }

        // Create Comment
        [HttpPost("Create")]
        public async Task<ActionResult<Comment>> CreateComment([FromBody] Comment newComment)
        {
            if (newComment == null || string.IsNullOrEmpty(newComment.CommentText))
            {
                return BadRequest("Comment content can't be empty.");
            }

            // Optionally, you could check if the associated post exists
            var postExists = await _context.Posts.AnyAsync(p => p.PostID == newComment.PostID);
            if (!postExists)
            {
                return BadRequest("Associated post does not exist.");
            }

            // Optionally, you could check if the username exists
            var userExists = await _context.Users.AnyAsync(u => u.Username == newComment.Username);
            if (!userExists)
            {
                return BadRequest("User does not exist.");
            }

            try
            {
                _context.Comments.Add(newComment);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(CreateComment), newComment);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }

        [HttpGet("ByPost")]
        public async Task<ActionResult<IEnumerable<Comment>>> GetCommentsByPostId(int postId)
        {
            var comments = await _context.Comments.Where(c => c.PostID == postId).ToListAsync();
            return Ok(comments);
        }
    }
}
