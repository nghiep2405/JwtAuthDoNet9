using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using JwtAuthDoNet9.Data;
using JwtAuthDoNet9.Entities;
using JwtAuthDoNet9.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace JwtAuthDoNet9.Services
{
    public class AuthService(UserDbContext context, IConfiguration configuration) : IAuthService
    {
        public async Task<string?> LoginAsync(UserDto request)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName);
            if (user is null)
            {
                return null;
            }
            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
            {
                return null;
            }

            //token
            return CreateToken(user);
        }
        private string CreateToken(User user)
        {
            var claim = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role)
            }; 

            var tmpK = configuration.GetValue<string>("AppSettings:Token");
            // avoidance Token in appSettings is null
            if (tmpK == null)
                tmpK = "hellooooooooooooooooooooooooooooooooooooooooooooooooooooo1235555555555566666666666666666777777777777777777777777777777777777999999999999999999999911111111111110000000000000000000";
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(tmpK));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
            var tokenDescriptor = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                audience: configuration.GetValue<string>("AppSettings:Audience"),
                claims: claim,
                expires: DateTime.Now.AddDays(2),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

        }

        public async Task<User?> RegisterAsync(UserDto request)
        {
            if(await context.Users.AnyAsync(u => u.UserName == request.UserName))
            {
                return null;
            }
            var user = new User();
            var hashedPassword = new PasswordHasher<User>().HashPassword(user, request.Password);
            user.UserName = request.UserName;
            user.PasswordHash = hashedPassword;
            context.Users.Add(user);
            await context.SaveChangesAsync();
            return user;
        }
    }
}
