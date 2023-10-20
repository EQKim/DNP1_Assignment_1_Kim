using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PostController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PostController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetAllPost")]
        public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
        {
            return await _context.Posts.ToListAsync();
        }

        [HttpGet("ByTitle")]
        public async Task<ActionResult<IEnumerable<Post>>> GetPostFromTitle(string title)
        {
            return await _context.Posts
                .Where(p => p.Title.Contains(title, StringComparison.OrdinalIgnoreCase))
                .ToListAsync();
        }

        [HttpPost("CreatePost")]
        public async Task<ActionResult<Post>> CreatePost([FromBody] Post newPost)
        {
            if (string.IsNullOrEmpty(newPost.Context))
                return BadRequest("Your description can't be empty");

            if (string.IsNullOrEmpty(newPost.Title))
                return BadRequest("Your title can't be empty");

            var userExists = await _context.Users.AnyAsync(u => u.Username == newPost.Username);
            if (!userExists)
                return BadRequest("User does not exist.");

            try
            {
                _context.Posts.Add(newPost);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetPostFromTitle), new { title = newPost.Title }, newPost);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }
    }
}