using Microsoft.AspNetCore.Mvc;
using ReactForUI.Server.Entities;
using ReactForUI.Server.Data;
using MongoDB.Driver;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Bson;
using ZstdSharp.Unsafe;

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

        //[Authorize]
        [HttpPost("addBooks")]
        public async Task<IActionResult> AddBooks([FromBody] Book book)
        {
            book.Id = "";
            // Nếu Id rỗng hoặc null thì để mặc định cho MongoDB tự sinh
            if (string.IsNullOrEmpty(book.Id))
            {
                book.Id = ObjectId.GenerateNewId().ToString();
            }
            await _context.Books.InsertOneAsync(book);  
            return Ok(book);
        }

        [HttpDelete("books/{id}")]
        public async Task<IActionResult> DeleteBooks([FromRoute(Name = "id")] string bookId)
        {
            //var objectId = new ObjectId(bookId);
            //var res = await _context.Books.DeleteOneAsync(i => i.Id == objectId);

            // xem bookId là tên
            var res = await _context.Books.DeleteOneAsync(i => i.Id == bookId);
            if(res.DeletedCount == 0)
            {
                NotFound("Không tìm thấy sách bạn cần xóa");
            }
            return Ok("Đã xóa thành công");
        }
        [HttpPut("updatePrice/{id}")]
        public async Task<IActionResult> UpdatePrice([FromRoute(Name = "id")] string bookId, [FromBody] double nPrice)
        {
            var filter = Builders<Book>.Filter.Eq(b => b.Id, bookId);
            var update = Builders<Book>.Update.Set(b => b.Price, nPrice);

            var res = await _context.Books.UpdateOneAsync(filter, update);
            if(res.ModifiedCount == 0)
            {
                NotFound("Không tìm thấy sách để cập nhật lại giá");
            }
            return Ok("Đã cập nhật giá thành công");
        }
    }
}
