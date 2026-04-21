using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace mongo_api.Model
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public required string name { get; set; }
        public required string email { get; set; }
        public int age { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
