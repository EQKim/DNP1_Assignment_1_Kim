using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using WebAPI.Models;
using System.Threading.Tasks;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly string _jwtSecret = "ThisIsASuperSecureSecretKey32Char";
    private readonly double _jwtLifespan = 120;
    private readonly ILogger<UserController> _logger;
    private readonly AppDbContext _context;

    public UserController(ILogger<UserController> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpPost("login")]
    public ActionResult<User> GetUser([FromBody] User loginRequest)
    {
        var user = _context.Users.FirstOrDefault(u => u.Username == loginRequest.Username);

        if (user == null || user.Password != loginRequest.Password)
            return Unauthorized("Invalid username or password.");

        var token = GenerateJwtToken(user);
        return Ok(new { Token = token });
    }

    [HttpPost("CreateAccount")]
    public async Task<ActionResult<User>> CreateUser([FromBody] User newUser)
    {
        if (_context.Users.Any(u => u.Username == newUser.Username))
            return Conflict("Username already exists");

        try
        {
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
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
        return Ok(new { message = "Logged out successfully" });
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "https://localhost/auth",
            audience: "https://localhost/api",
            claims: claims,
            expires: DateTime.Now.AddMinutes(_jwtLifespan),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
