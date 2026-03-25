using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly JwtTokenService _tokenService;

    public AuthController(JwtTokenService tokenService)
    {
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // TODO: Validate credentials against your database
        if (request.Username == "admin" && request.Password == "password123")
        {
            var token = _tokenService.GenerateToken(
                userId: "1",
                userName: request.Username,
                email: "admin@example.com",
                roles: new List<string> { "Admin" }
            );

            return Ok(new { token });
        }

        return Unauthorized("Invalid credentials");
    }

    [Authorize] // Requires any valid token
    [HttpGet("protected")]
    public IActionResult GetProtectedData()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Ok(new { message = "This is protected data", userId });
    }

    [Authorize(Roles = "Admin")] // Requires Admin role
    [HttpDelete("admin-only")]
    public IActionResult AdminOnly()
    {
        return Ok(new { message = "Admin access granted" });
    }
}

public class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}