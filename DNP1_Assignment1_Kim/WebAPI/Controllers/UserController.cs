using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebAPI.Models;
using Microsoft.Extensions.Logging; // Import for logging

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly string _jwtSecret = "ThisIsASuperSecureSecretKey32Char"; 
    private readonly double _jwtLifespan = 120; // JWT lifespan in minutes.
    private readonly ILogger<UserController> _logger; // Logger field

    public UserController(ILogger<UserController> logger) // Constructor to inject logger
    {
        _logger = logger;
    }

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
                    var token = GenerateJwtToken(user);
                    return Ok(new { Token = token });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the login request.");
                return BadRequest("An error occurred while processing your request.");
            }
        }

        return Unauthorized("Invalid username or password.");
    }

    [HttpPost("CreateAccount")]
    public async Task<ActionResult<User>> CreateUser([FromBody] User newUser)
    {
        var folderName = "JSON_Storage";

        // Guard
        if (!Directory.Exists(folderName))
        {
            Directory.CreateDirectory(folderName);
        }

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
            _logger.LogError(ex, "An error occurred while creating the user.");
            return BadRequest("An error occurred while creating the user");
        }
    }
    
    
    [HttpPost("Logout")]
    public ActionResult Logout()
    {
        // Here, we're not doing much server-side. But this is where you'd add logic if you implement token blacklisting.
        return Ok(new { message = "Logged out successfully" });
    }


    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            // Add more claims as needed
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "https://localhost/auth", // Modify this to represent your application
            audience: "https://localhost/api", // Modify this to also represent your application
            claims: claims,
            expires: DateTime.Now.AddMinutes(_jwtLifespan),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
