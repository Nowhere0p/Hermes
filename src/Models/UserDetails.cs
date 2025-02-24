using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Hermes.src.Models;

public class UserDetails : User
{
    [BsonElement("_id")]
    public string Id { get; set; }

    [BsonElement("publicUsername")]
    public string? PublicUsername { get; set; }

    [BsonElement("userId")]
    public string? UserId { get; set; }

    [BsonElement("createdAt")]
    public DateTime? CreatedAt { get; set; }

    [BsonElement("isVerified")]
    public bool IsVerified { get; set; } = false;

    [BsonElement("role")]
    public Role Role { get; set; } = Role.USER;

    [BsonElement("gender")]
    public Gender? Gender { get; set; }

    public UserDetails()
    {
        Id = Guid.NewGuid().ToString();
        base.PartitionKey = DateTime.UtcNow.ToString("MM-yyyy");
    }
}

public enum Role
{
    USER=0,
    ADMIN=1,
}

public enum Gender
{
    Male=100,
    Female=200,
    NonBinary=300,
    PreferNotToSay=400,
    Other=500,
}

public enum Country {
    INDIA=100,
    PAKISTAN=200,
}