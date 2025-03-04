using System;
using System.Text.Json.Serialization;
using Hermes.DbCore;
using MongoDB.Bson.Serialization.Attributes;
namespace Hermes.src.Models;

    public class OtpVerification :IMongoDbRecord
    {
        [BsonElement("id")]
        public string Id { get; set; }

        [BsonElement("partitionKey")]
        public string PartitionKey { get; set; }

        [BsonElement("verificationCode")]
        public string verificationCode { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("generatedAt")]
        public DateTime GeneratedAt { get; set; }

        [BsonElement("expiresAt")]
        public DateTime ExpiresAt { get; set; }

        [BsonElement("isValid")]
        public bool IsValid { get; set; }

        public OtpVerification()
        {   
            Id=Guid.NewGuid().ToString();
            GeneratedAt = DateTime.UtcNow;
            ExpiresAt = GeneratedAt.AddMinutes(5); // OTP expires in 5 minutes
            IsValid = true;
            PartitionKey = DateTime.UtcNow.ToString("MM-yyyy");
        }

        public object GetPartitionKey()
        {
            return PartitionKey;
        }
    }
    public class OtpVerificationInteraction {
        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; }
        
    }
