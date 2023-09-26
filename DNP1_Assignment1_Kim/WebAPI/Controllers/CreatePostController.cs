using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class CreatePostController : ControllerBase
{
    private static readonly List<Post> Posts = new List<Post>();


    //GETTER
    [HttpGet]
    public ActionResult<IEnumerable<Post>> GetPosts()
    {
        return Posts;
    }


    //Create-Post
    [HttpPost]
    public ActionResult<Post> CreatePost([FromBody] Post newPost)
    {
        newPost.ID = Posts.Count + 1;
        Posts.Add(newPost);
        return CreatedAtAction(nameof(CreatePost), newPost);
    }
}