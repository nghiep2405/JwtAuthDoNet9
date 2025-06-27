using Microsoft.AspNetCore.Mvc;
using ReactForUI.Server.Entities;
using ReactForUI.Server.Data;
using MongoDB.Driver;
using Microsoft.AspNetCore.Authorization;

namespace ReactForUI.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MongoController : ControllerBase
    {
        private readonly MongoDbContext _context;

        public MongoController(MongoDbContext context)
        {
            _context = context;
        }

        //[Authorize]
        [HttpGet("books")]
        public async Task<IActionResult> GetBooks()
        {
            var books = await _context.Books.Find(_ => true).ToListAsync();
            return Ok(books);
        }
    }
}
