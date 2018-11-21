using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PolaroidAPI.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new PolaroidAPIContext(
                serviceProvider.GetRequiredService<DbContextOptions<PolaroidAPIContext>>()))
            {
                // Look for any movies.
                if (context.PostItem.Count() > 0 && context.UserItem.Count() > 0)
                {
                    return;   // DB has been seeded
                }

                context.PostItem.AddRange(
                    new PostItem
                    {
                        UserID = 1,
                        ImageURL = "https://pbs.twimg.com/media/DOXI0IEXkAAkokm.jpg",
                        Caption = "Moving the community!",
                        Uploaded = DateTime.Now,
                        Likes = 12,
                    }
                );
                context.UserItem.AddRange(
                    new UserItem
                    {
                        Username = "seed",
                        Name = "Pineapple Seed",
                        Email = "demo@preetpatel.com",
                        AvatarURL = "https://pbs.twimg.com/media/DOXI0IEXkAAkokm.jpg",
                        Bio = "A test account"
                    });
                context.Relationships.AddRange(
                    new Relationships
                    {
                        Person = 1,
                        Follows = 2
                        
                    });
                context.SaveChanges();
            }
        }
    }
}
