using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PolaroidAPI.Models
{
    public class PostItem
    {
        public int Id { get; set; }
        public int UserID { get; set; }
        public string ImageURL { get; set; }
        public string Caption { get; set; }
        public DateTime Uploaded { get; set; }
        public int Likes { get; set; }
       
    }
}
