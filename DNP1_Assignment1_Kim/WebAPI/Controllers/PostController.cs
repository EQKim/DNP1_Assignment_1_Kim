using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PostController : ControllerBase
{
    private static readonly List<Post> Posts = new List<Post>();


    //Get All Posts
    [HttpGet("GetAllPost")]
    public ActionResult<IEnumerable<Post>> GetPosts()
    {
        //Fields 
        var foldername = "JSON_Storage";
        var posts = new List<Post>();

        //Guard
        if (Directory.Exists(foldername))
        {
            var files = Directory.GetFiles(foldername, "Post_*.json");
            foreach (var file in files)
            {
                try
                {
                    var json = System.IO.File.ReadAllText(file);
                    var post = JsonSerializer.Deserialize<Post>(json);
                    posts.Add(post);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        return posts;
    }


    //Get Specified Post
    [HttpGet("ByTitle")]
    public ActionResult<IEnumerable<Post>> GetPostFromTitle(String title)
    {
        var foldername = "JSON_Storage";
        var posts = new List<Post>();

        if (Directory.Exists(foldername))
        {
            var files = Directory.GetFiles(foldername, "Post_*.json");
            foreach (var file in files)
            {
                try
                {
                    var json = System.IO.File.ReadAllText(file);
                    var post = JsonSerializer.Deserialize<Post>(json);

                    // Check if the post title contains the search term (case-insensitive)
                    if (post.title.IndexOf(title, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        posts.Add(post);
                    }
                }

                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        return posts;
    }


    //Create
    [HttpPost("CreatePost")]
    public async Task<ActionResult<Post>> CreatePost([FromBody] Post newPost)
    {
        //Revised maybe? --------------
        newPost.ID = Posts.Count + 1;
        Posts.Add(newPost);
        //-----------------------------
        var foldername = "JSON_Storage";

        //Guards
        if (!Directory.Exists("JSON_Storage"))
        {
            Directory.CreateDirectory(foldername);
        }

        if (string.IsNullOrEmpty(newPost.context))
        {
            throw new Exception("Your description cant be empty");
        }

        if (string.IsNullOrEmpty(newPost.title))
        {
            throw new Exception("Your title cant be empty");
        }


        //JSON
        var json = JsonSerializer.Serialize(newPost);
        var fileName = $"{foldername}/Post_{newPost.ID}.json";
        await System.IO.File.WriteAllTextAsync(fileName, json);
        return CreatedAtAction(nameof(CreatePost), newPost);
    }
}