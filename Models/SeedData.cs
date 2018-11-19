using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PolaroidPostsApi.Models
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new PolaroidPostsApiContext(
                serviceProvider.GetRequiredService<DbContextOptions<PolaroidPostsApiContext>>()))
            {
                // Look for any movies.
                if (context.PostItem.Count() > 0)
                {
                    return;   // DB has been seeded
                }

                context.PostItem.AddRange(
                    new PostItem
                    {
                        Username = "Chris",
                        ImageURL = "https://www.laravelnigeria.com/img/chris.jpg",
                        Caption = "Moving the community!",
                        Uploaded = "07-10-18 4:20T18:25:43.511Z",
                        Likes = 12
                    }


                );
                context.SaveChanges();
            }
        }
    }
}
