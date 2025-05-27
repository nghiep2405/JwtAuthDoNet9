using JwtAuthDoNet9.Entities;
using JwtAuthDoNet9.Models;

namespace JwtAuthDoNet9.Services
{
    public interface IAuthService 
    {
        Task<User?> RegisterAsync(UserDto request);
        Task<string?> LoginAsync(UserDto  request);
    }
}
