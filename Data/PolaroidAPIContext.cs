using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PolaroidAPI.Models;

namespace PolaroidAPI.Models
{
    public class PolaroidAPIContext : DbContext
    {
        public PolaroidAPIContext (DbContextOptions<PolaroidAPIContext> options)
            : base(options)
        {
        }

        public DbSet<PolaroidAPI.Models.UserItem> UserItem { get; set; }

        public DbSet<PolaroidAPI.Models.PostItem> PostItem { get; set; }

        public DbSet<PolaroidAPI.Models.Relationships> Relationships { get; set; }
    }
}
