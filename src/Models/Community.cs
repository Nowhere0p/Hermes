using System;
using System.Collections.Generic;

namespace Hermes.Models
{
    public class Community
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        
        /// <summary>
        /// stores id of posts
        /// </summary>
        public List<string> Posts { get; set; } 
        List<string> Moderators { get; set; } 
        
        List<string> Admins { get; set; }
        
        /// <summary>
        /// stores id of members
        /// </summary>
        public List<string> Members { get; set; }
        public List<string> Tags { get; set; } 
        
        public DateTime CreatedAt { get; set; }

        public Community()
        {
            Id = Guid.NewGuid().ToString();
            CreatedAt = DateTime.UtcNow;
        }
    }
}