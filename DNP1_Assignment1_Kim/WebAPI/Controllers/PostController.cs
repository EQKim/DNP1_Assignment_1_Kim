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
        private readonly AppDbContext context;
        private readonly ILogger logger;

        public PostController(AppDbContext context, ILogger<PostController> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        
        //Get All Posts
        [HttpGet("GetAllPost")]
        public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
        {
            return await context.Posts.ToListAsync();
        }

        
        //Get Post By Title
        [HttpGet("ByTitle")]
        public async Task<ActionResult<IEnumerable<Post>>> GetPostFromTitle(string title)
        {
            title = title.ToLower();

            return await context.Posts
                .Where(p => EF.Functions.Like(p.Title.ToLower(), $"%{title}%"))
                .ToListAsync();
        }



        //Create Post
        [HttpPost("CreatePost")]
        public async Task<ActionResult<Post>> CreatePost([FromBody] Post newPost)
        {
            if (string.IsNullOrEmpty(newPost.Context))
                return BadRequest("Your description can't be empty");

            if (string.IsNullOrEmpty(newPost.Title))
                return BadRequest("Your title can't be empty");

            var userExists = await context.Users.AnyAsync(u => u.Username == newPost.Username);
            if (!userExists)
                return BadRequest("User does not exist.");

            try
            {
                context.Posts.Add(newPost);
                await context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetPostFromTitle), new { title = newPost.Title }, newPost);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }
        
        
        
        //Get Post By ID
        [HttpGet("GetById")]
        public async Task<ActionResult<Post>> GetPostFromID(int ID)
        {
            logger.LogInformation($"Fetching post with ID {ID}");

            var post = await context.Posts.FirstOrDefaultAsync(p => p.PostID == ID);
            if (post == null)
            {
                logger.LogWarning($"No post found with ID {ID}");
                return NotFound("Post not found.");
            }
            return post;
        }
    }
}