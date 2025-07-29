using Microsoft.AspNetCore.Mvc;
using ReactForUI.Server.Data;
using ReactForUI.Server.Entities;
using MongoDB.Driver;
using static ReactForUI.Server.Entities.Cart;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace ReactForUI.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly MongoDbContext _context;
        public CartController(MongoDbContext context)
        {
            _context = context;
        }
        [HttpGet("{userId}")]
        public async Task<IActionResult> getCart(string userId)
        {
            var cart = await _context.Carts.Find(i => i.UserId == userId).FirstOrDefaultAsync();
            if (cart == null)
            {
                cart = new Cart();
                cart.UserId = userId;
                cart.Items = new List<CartItem>();
                await _context.Carts.InsertOneAsync(cart);
            }
            return Ok(cart);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCart(string id)
        {
            var result = await _context.Carts.DeleteOneAsync(c => c.Id == id);
            if (result.DeletedCount == 0)
            {
                return NotFound(new { message = $"Không tìm thấy cart với id: {id}" });
            }

            return Ok(new { message = $"Đã xóa cart với id: {id}" });
        }

        [HttpPost("addItem")]
        public async Task<IActionResult> addItemToCart([FromBody] CartItem newItem, [FromQuery] string userId)
        {
            var cart = await _context.Carts.Find(i => i.UserId == userId).FirstOrDefaultAsync();
            if (cart == null)
            {
                cart = new Cart();
                cart.UserId = userId;
                cart.Items = new List<CartItem>();
                await _context.Carts.InsertOneAsync(cart);
            }
            else
            {
                var existingItem = cart.Items.FirstOrDefault(i => i.BookId == newItem.BookId);
                if (existingItem != null)
                {
                    return Conflict("Sản phẩm đã có trong giỏ hàng. Hãy dùng API cập nhật số lượng.");
                }
                else
                {
                    cart.Items.Add(newItem);
                }
                var filter = Builders<Cart>.Filter.Eq(c => c.Id, cart.Id);
                var update = Builders<Cart>.Update.Set(c => c.Items, cart.Items);
                await _context.Carts.UpdateOneAsync(filter, update);
            }
            return Ok(cart);
        }
        [HttpDelete("removeItem")]
        public async Task<IActionResult> removeItemToCart([FromQuery] string bookId, string userId)
        {
            var cart = await _context.Carts.Find(i => i.UserId == userId).FirstOrDefaultAsync();
            if (cart == null)
            {
                return NotFound("Giỏ hàng không tồn tại");
            }
            var filter = Builders<Cart>.Filter.Eq(cart => cart.Id, cart.Id);
            var update = Builders<Cart>.Update.PullFilter(c => c.Items, i => i.BookId == bookId);
            var res = await _context.Carts.UpdateOneAsync(filter, update);
            if (res.ModifiedCount == 0)
            {
                return NotFound("Không tìm thấy sản phẩm để xóa");
            }
            return Ok("Đã xóa sản phẩm đó");
        }

        [HttpPut("updateQuantity")]
        public async Task<IActionResult> updateItemQuantity([FromQuery] string userId, [FromQuery] string bookId, int newQuantity)
        {
            var cart = await _context.Carts.Find(i => i.UserId == userId).FirstOrDefaultAsync();
            if (cart == null)
            {
                return NotFound("Giõ hàng không tồn tại");
            }
            var filter = Builders<Cart>.Filter.And(
                Builders<Cart>.Filter.Eq(c => c.Id, cart.Id),
                Builders<Cart>.Filter.ElemMatch(c => c.Items, i => i.BookId == bookId)
            );
            var update = Builders<Cart>.Update.Set("Items.$.Quantity", newQuantity);
            var result = await _context.Carts.UpdateOneAsync(filter, update);

            if (result.MatchedCount == 0)
            {
                return NotFound("Không tìm thấy sản phẩm cần cập nhật.");
            }
            return Ok("Đã cập nhật số lượng sản phẩm");
        }
    } 
}