using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PolaroidPostsApi.Models
{
    public class PostItem
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string ImageURL { get; set; }
        public string Caption { get; set; }
        public string Uploaded { get; set; }
        public int Likes { get; set; }
        public string Email { get; set; }
        public string AvatarURL { get; set; }
    }
}
