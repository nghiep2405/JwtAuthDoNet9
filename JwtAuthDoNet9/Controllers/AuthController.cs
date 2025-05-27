using JwtAuthDoNet9.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using JwtAuthDoNet9.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using JwtAuthDoNet9.Services;
using Microsoft.AspNetCore.Authorization;

namespace JwtAuthDoNet9.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            var user = await authService.RegisterAsync(request);
            if(user is null)
            {
                return BadRequest("User already exists");
            }
            return Ok(user);

        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDto request)
        {
            var token = await authService.LoginAsync(request);
            if(token is null)
            {
                return BadRequest("Invalid username or password");   
            }
            return Ok(token);
        }

        [Authorize]
        [HttpGet]
        public IActionResult AuthenticatedOnlyendpoint()
        {
            return Ok("You are authenticated!");
        }
    }
}
