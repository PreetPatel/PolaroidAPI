using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PolaroidPostsApi.Models
{
    public class PolaroidPostsApiContext : DbContext
    {
        public PolaroidPostsApiContext (DbContextOptions<PolaroidPostsApiContext> options)
            : base(options)
        {
        }

        public DbSet<PolaroidPostsApi.Models.PostItem> PostItem { get; set; }
    }
}
