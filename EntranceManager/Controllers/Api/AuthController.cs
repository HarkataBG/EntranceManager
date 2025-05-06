using EntranceManager.Models.Mappers;
using EntranceManager.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto.Username, dto.Password);
        if (!result) return BadRequest("User already exists.");
        return Ok("Registered successfully.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var token = await _authService.LoginAsync(dto.Username, dto.Password);
        if (token == null) return Unauthorized("Invalid credentials.");

        return Ok(new { token });
    }
}