using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using PolaroidAPI.Helpers;
using PolaroidAPI.Models;

namespace PolaroidAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostItemsController : ControllerBase
    {
        private readonly PolaroidAPIContext _context;
        private IConfiguration _configuration;

        public PostItemsController(PolaroidAPIContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: api/PostItems
        [HttpGet]
        public IEnumerable<PostItem> GetPostItem()
        {
            return _context.PostItem;
        }

        // GET: api/PostItems/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPostItem([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var postItem = await _context.PostItem.FindAsync(id);

            if (postItem == null)
            {
                return NotFound();
            }

            return Ok(postItem);
        }

        // PUT: api/PostItems/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPostItem([FromRoute] int id, [FromBody] PostItem postItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != postItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(postItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/PostItems
        [HttpPost]
        public async Task<IActionResult> PostPostItem([FromBody] PostItem postItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.PostItem.Add(postItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPostItem", new { id = postItem.Id }, postItem);
        }

        // DELETE: api/PostItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePostItem([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var postItem = await _context.PostItem.FindAsync(id);
            if (postItem == null)
            {
                return NotFound();
            }

            _context.PostItem.Remove(postItem);
            await _context.SaveChangesAsync();

            return Ok(postItem);
        }

        private bool PostItemExists(int id)
        {
            return _context.PostItem.Any(e => e.Id == id);
        }

        // GET: api/postitems/userID
        [HttpGet("filter/{UserID}")]
        public IEnumerable<PostItem> GetPostByEmail([FromRoute] int userID)
        {

            return _context.PostItem.Where(p => p.UserID == userID);
        }

        // GET: api/postitems/userID[]
        [HttpGet("filterall/")]
        public IEnumerable<PostItem> GetMultiplePosts([FromQuery] int[] UserID)
        {
            List<PostItem> returnList = new List<PostItem>();

            foreach (int i in UserID)
            {
                
                IEnumerable<PostItem> individualUserPosts = _context.PostItem.Where(p => p.UserID == i);

                foreach (PostItem pi in individualUserPosts)
                {
                    returnList.Add(pi);
                }

            }
            returnList = returnList.OrderByDescending(d => d.Uploaded).ToList();
            return returnList;
        }

        [HttpPost, Route("upload")]
        public async Task<IActionResult> UploadFile([FromForm]PostImageItems polaroidImage)
        {
            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest($"Expected a multipart request, but got {Request.ContentType}");
            }
            try
            {
                using (var stream = polaroidImage.Image.OpenReadStream())
                {
                    var cloudBlock = await UploadToBlob(polaroidImage.Image.FileName, null, stream);

                    //// Retrieve the filename of the file you have uploaded
                    //var filename = provider.FileData.FirstOrDefault()?.LocalFileName;
                    if (string.IsNullOrEmpty(cloudBlock.StorageUri.ToString()))
                    {
                        return BadRequest("An error has occured while uploading your file. Please try again.");
                    }

                    PostItem postItem = new PostItem();
                    postItem.UserID = polaroidImage.UserID;
                    postItem.Caption = polaroidImage.Caption;
        
                    System.Drawing.Image image = System.Drawing.Image.FromStream(stream);
                    postItem.ImageURL = cloudBlock.SnapshotQualifiedUri.AbsoluteUri;
                    postItem.Uploaded = DateTime.Now;
                    postItem.Likes = 0;

                    _context.PostItem.Add(postItem);
                    await _context.SaveChangesAsync();

                    return Ok($"Post has successfully uploaded");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"An error has occured. Details: {ex.Message}");
            }
        }

        private async Task<CloudBlockBlob> UploadToBlob(string filename, byte[] imageBuffer = null, System.IO.Stream stream = null)
        {

            var accountName = _configuration["AzureBlob:name"];
            var accountKey = _configuration["AzureBlob:key"]; ;
            var storageAccount = new CloudStorageAccount(new StorageCredentials(accountName, accountKey), true);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer imagesContainer = blobClient.GetContainerReference("images");

            string storageConnectionString = _configuration["AzureBlob:connectionString"];

            // Check whether the connection string can be parsed.
            if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
            {
                try
                {
                    // Generate a new filename for every new blob
                    var fileName = Guid.NewGuid().ToString();
                    fileName += GetFileExtention(filename);

                    // Get a reference to the blob address, then upload the file to the blob.
                    CloudBlockBlob cloudBlockBlob = imagesContainer.GetBlockBlobReference(fileName);

                    if (stream != null)
                    {
                        await cloudBlockBlob.UploadFromStreamAsync(stream);
                    }
                    else
                    {
                        return new CloudBlockBlob(new Uri(""));
                    }

                    return cloudBlockBlob;
                }
                catch (StorageException ex)
                {
                    return new CloudBlockBlob(new Uri(""));
                }
            }
            else
            {
                return new CloudBlockBlob(new Uri(""));
            }

        }

        private string GetFileExtention(string fileName)
        {
            if (!fileName.Contains("."))
                return ""; //no extension
            else
            {
                var extentionList = fileName.Split('.');
                return "." + extentionList.Last(); //assumes last item is the extension 
            }
        }

    }
}