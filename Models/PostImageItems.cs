using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PolaroidAPI.Models
{
    public class PostImageItems
    {
        public int UserID { get; set; }
        public IFormFile Image { get; set; }
        public string Caption { get; set; }
    }
}
