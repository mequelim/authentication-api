using AuthenticationAPI.Data;
using AuthenticationAPI.Entities;
using AuthenticationAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        // GET:
        [Authorize(Roles = "Admin")]
        [HttpGet("admin-only")]
        public IActionResult AdminOnlyEndpoint()
        {
            return Ok("You are authenticated!");
        }

        // POST:
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto userDto)
        {
            User? user = await authService.RegisterAsync(userDto);

            if(user is null) return BadRequest("This username already exists!");

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login(UserDto userDto)
        {
            TokenResponseDto? result = await authService.LoginAsync(userDto);

            if(result is null) return BadRequest("Invalid username or password!");

            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenRequestDto refreshToken)
        {
            TokenResponseDto? result = await authService.RefreshTokenAsync(refreshToken);

            if(result is null) return Unauthorized("Invalid response!");

            return Ok(result);
        }
    }
}