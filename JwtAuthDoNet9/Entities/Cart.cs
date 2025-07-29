using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace ReactForUI.Server.Entities
{
    public class Cart
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string UserId { get; set; } // Liên kết với người dùng
        public List<CartItem> Items { get; set; } = new();
        public class CartItem {
            public string BookId { get; set; }
            public int Quantity { get; set; }
        }

    }
}
