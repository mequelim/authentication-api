using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AuthenticationAPI.DTOs;
using AuthenticationAPI.Entities;
using AuthenticationAPI.Infrastructure.Database;
using AuthenticationAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationAPI.Services
{
    public class AuthService(AppDbContext databaseContext, IConfiguration configuration) : IAuthService
    {
        private string CreateToken(User userData)
        {
            List<Claim> claims =
            [
                new(ClaimTypes.Name, userData.Username),
                new(ClaimTypes.NameIdentifier, userData.UserId.ToString()),
                new(ClaimTypes.Role, userData.Role)
            ];

            SymmetricSecurityKey key = new(
                Encoding.UTF8.GetBytes(
                    configuration.GetValue<string>("AppSettings:Token")!
                )
            );

            SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha512);

            JwtSecurityToken tokenDescriptor = new(
                issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                audience: configuration.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        private static string GenerateRefreshToken()
        {
            byte[] randomNumber = new byte[32];
            using RandomNumberGenerator randomNumberGenerated = RandomNumberGenerator.Create();

            randomNumberGenerated.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }

        private async Task<string> GenerateAndSaveRefreshTokenAsync(User user)
        {
            string refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await databaseContext.SaveChangesAsync();

            return refreshToken;
        }

        private async Task<User?> ValidateRefreshTokenAsync(Guid userId, string refreshToken)
        {
            User? user = await databaseContext.Users.FindAsync(userId);

            if((user is null) || (user.RefreshToken != refreshToken) || (user.RefreshTokenExpiryTime <= DateTime.UtcNow)) return null;

            return user;
        }

        private async Task<TokenResponseDto> CreateTokenResponseAsync(User user)
        {
            return new TokenResponseDto()
            {
                AccessToken = CreateToken(user),
                RefreshToken = await GenerateAndSaveRefreshTokenAsync(user)
            };
        }

        public async Task<TokenResponseDto?> LoginAsync(UserDto request)
        {
            User? user = await databaseContext.Users
                .FirstOrDefaultAsync((u) => u.Username.Equals(request.Username));

            if(user is null) return null;
            if(new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password).Equals(PasswordVerificationResult.Failed))
            {
                return null;
            }

            return await CreateTokenResponseAsync(user);
        }

        public async Task<User?> RegisterAsync(UserDto request)
        {
            if(await databaseContext.Users.AnyAsync((u) => u.Username.Equals(request.Username))) return null;

            User user = new();
            string hashedPassword = new PasswordHasher<User>()
                .HashPassword(user, request.Password);

            user.Username = request.Username;
            user.PasswordHash = hashedPassword;

            databaseContext.Users.Add(user);
            await databaseContext.SaveChangesAsync();

            return user;
        }

        public async Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            User? user = await ValidateRefreshTokenAsync(request.UserId, request.RefreshToken);

            if(user is null) return null;

            return await CreateTokenResponseAsync(user);
        }
    }
}