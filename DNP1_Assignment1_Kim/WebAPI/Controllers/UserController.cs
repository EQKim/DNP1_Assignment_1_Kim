using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]


public class UserController : ControllerBase
{

    //Login
    [HttpPost("login")]
    public ActionResult<User> GetUser([FromBody] User loginRequest)
    {
        var folderName = "JSON_Storage";
        var fileName = $"{folderName}/User{loginRequest.Username}.json";

        if (System.IO.File.Exists(fileName))
        {
            try
            {
                var json = System.IO.File.ReadAllText(fileName);
                var user = JsonSerializer.Deserialize<User>(json);

                if (user.Password == loginRequest.Password)
                {
                    return user;
                }
            }
            catch (Exception ex)
            {
                return BadRequest("An error occurred while processing your request.");
            }
        }
        return Unauthorized("Invalid username or password.");
    }



    //Create
    [HttpPost("CreateAccount")]
    public async Task<ActionResult<User>> CreateUser([FromBody] User newUser)
    {
        var folderName = "JSON_Storage";

        
        // Guard
        if (!Directory.Exists(folderName))
        {
            Directory.CreateDirectory(folderName);
        }
        
        //File Location
        var fileName = $"{folderName}/User{newUser.Username}.json";

        
        // Guard, check if username exists
        if (System.IO.File.Exists(fileName))
        {
            return Conflict("Username already exists");
        }
        try
        {
            var json = JsonSerializer.Serialize(newUser);
            await System.IO.File.WriteAllTextAsync(fileName, json);
            return CreatedAtAction(nameof(CreateUser), newUser);
        }
        catch (Exception ex)
        {
            return BadRequest("An error occurred while creating the user");
        }
    }
}
