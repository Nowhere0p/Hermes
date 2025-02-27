using System.Text.Json.Serialization;
using Hermes.DbCore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Hermes.src.Models;

public class User : IMongoDbRecord
{
    [BsonElement("firstName")]
    public string FirstName { get; set; }

    [BsonElement("lastName")]
    public string LastName { get; set; }

    [BsonElement("email")]
    public string Email { get; set; }

    [BsonElement("password")]
    public string Password { get; set; }

    [BsonElement("partitionKey")]
    public string PartitionKey { get; set; }

    public object GetPartitionKey()
    {
        return PartitionKey;
    }

    public User GetRedactedUser()
    {
        // Redact password for security
        Password = "[RedactedPassword]";
        return this;
    }
}

public class RegistrationInteraction {
 
    [JsonPropertyName("firstName")]
    public required string FirstName { get; set; }
    
    [JsonPropertyName("lastName")]
    public required string LastName { get; set; }
    
    [JsonPropertyName("email")]
    public required string Email { get; set; }
    
    [JsonPropertyName("password")]
    public required string Password { get; set; }
    
    [JsonPropertyName("gender")]
    public Gender Gender { get; set; }
    
    [JsonPropertyName("country")]
    public Country Country { get; set; }
}