using AuthenticationAPI.DTOs;
using AuthenticationAPI.Entities;

namespace AuthenticationAPI.Services.Interfaces
{
    public interface IAuthService
    {
        Task<TokenResponseDto?> LoginAsync(UserDto request);
        Task<User?> RegisterAsync(UserDto request);
        Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request);
    }
}