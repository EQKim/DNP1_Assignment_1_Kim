using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PostController : ControllerBase
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
    public async Task<ActionResult<Post>> CreatePost([FromBody] Post newPost)
    {
        newPost.ID = Posts.Count + 1;
        Posts.Add(newPost);
        var foldername = "JSON_Storage";
        
        //Guard
        if (!Directory.Exists("JSON_Storage"))
        {
            Directory.CreateDirectory(foldername);
        }
        
        
        //JSON
        var json = JsonSerializer.Serialize(newPost);
        var fileName = $"{foldername}/Post_{newPost.ID}.json";
        await System.IO.File.WriteAllTextAsync(fileName, json);
        return CreatedAtAction(nameof(CreatePost), newPost);
    }
}