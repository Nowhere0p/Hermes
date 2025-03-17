using System;
using System.Collections.Generic;
using Hermes.DbCore;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Hermes.src.Models
{
    public class PostModel : IMongoDbRecord
    {
        [BsonElement("id")]
        public string Id { get; set; }

        [BsonElement("partitionKey")]
        public string PartitionKey { get; set; }

        [BsonElement("textContent")]
        public Content Content { get; set; }

        [BsonElement("upVote")]
        public int UpVote { get; set; }

        [BsonElement("downVote")]
        public int DownVote { get; set; }

        [BsonElement("commentsCount")]
        public int CommentsCount { get; set; }

        [BsonElement("authorUserId")]
        
        public string AuthorUserId { get; set; }

        [BsonElement("allComments")]
        public List<Comment> AllComments { get; set; }

        [BsonElement("postTags")]
        public List<string> PostTags { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("lastUpdatedAt")]
        public DateTime LastUpdatedAt { get; set; }

        [BsonElement("isApproved")]
        public bool IsApproved { get; set; }

        public PostModel()
        {
            Id = Guid.NewGuid().ToString();
            PartitionKey = DateTime.UtcNow.ToString("MM-yyyy");
            CreatedAt = DateTime.UtcNow;
            IsApproved = false;
        }

        public object GetPartitionKey()
        {
            return this.PartitionKey;
        }
    }

    public class Content
    {
        [BsonElement("title")]
        public string Title { get; set; }
        [BsonElement("textContent")]
        public string? TextContent { get; set; }
        [BsonElement("mediaContent")]   
        public string? MediaContent { get; set; }
    }

    public class Comment
    {
        [BsonElement("id")]
        public string Id { get; set; }

        [BsonElement("userId")]
        public string UserId { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("commentId")]
        public string CommentId { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("isDeleted")]
        public bool IsDeleted { get; set; }

        [BsonElement("upVoteCount")]
        public int UpVoteCount { get; set; }

        [BsonElement("downVoteCount")]
        public int? DownVoteCount { get; set; }

        [BsonElement("replies")]
        public List<string> Replies { get; set; } = new List<string>();

        [BsonElement("postId")]
        public string PostId { get; set; }

        public Comment()
        {
            Id = Guid.NewGuid().ToString();
            CreatedAt = DateTime.UtcNow;
            IsDeleted = false;
            UpVoteCount = 0;
            DownVoteCount = 0;
            CommentId = Guid.NewGuid().ToString();
        }
    }

    public class PostInteraction
    {
        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("textContent")]
        public string? TextContent { get; set; }

        [BsonElement("mediaContent")]
        public string? MediaContent { get; set; }

        [BsonElement("postTags")]
        public List<string>? PostTags { get; set; } = new List<string>();

        [BsonElement("communityId")]
        public string CommunityId { get; set; }
    }
}
