using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace ReactForUI.Server.Entities
{
    public class Book
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        //public ObjectId Id { get; set; }  // Change to ObjectId
        [BsonElement("title")]
        public string Title { get; set; }
        [BsonElement("author")]
        public string Author { get; set; }
        [BsonElement("genres")]
        public List<string> Genres { get; set; }
        [BsonElement("price")]
        public double Price { get; set; }
        [BsonElement("publishedYear")]
        public int Year { get; set;}

    }
}
