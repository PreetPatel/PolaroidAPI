using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PolaroidPostsApi.Models
{
    public class Postimageitem
    {
        public string Username { get; set; }
        public IFormFile Image { get; set; }
        public string Caption { get; set; }
        public string Email { get; set; }
        public string Avatar { get; set; }
    }
}
