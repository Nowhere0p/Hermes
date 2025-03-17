using System.Text.Json.Serialization;
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
    public Gender Gender { get; set; }

    [BsonElement("country")]
    public Country Country { get; set; }
    /// <summary>
    /// communities joined by user
    /// </summary>  
    List<string> JoinedCommunities{get; set;}=[];

    /// <summary>
    /// communities created by user
    /// </summary>
    List<string> CommunitiesCreated{get; set;}=[];
    
    [BsonElement("adminAt")]
    List<string> AdminAt{get; set;}=[];


    /// <summary>
    /// stores id of posts
    /// </summary>

    public UserDetails()
    {
        Id = Guid.NewGuid().ToString();
        base.PartitionKey = DateTime.UtcNow.ToString("MM-yyyy");
        CreatedAt=DateTime.UtcNow;
    }
}

public class LoginInteraction {
    [JsonPropertyName("emailOrUsername")]
    public required string EmailOrUsername { get; set; }
    [JsonPropertyName("password")]
    public required string Password { get; set; }
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

public static class CustomClaimTypes {
    public const string UserId = "userId";
}

public class AuthResponse {
    [JsonPropertyName("jwt")]
    public string? Jwt {get; set;}
}