using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PolaroidAPI.Models;

namespace PolaroidAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserItemsController : ControllerBase
    {
        private readonly PolaroidAPIContext _context;
        private IConfiguration _configuration;

        public UserItemsController(PolaroidAPIContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: api/UserItems
        [HttpGet]
        public IEnumerable<UserItem> GetUserItem()
        {
            return _context.UserItem;
        }

        // GET: api/UserItems/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserItem([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userItem = await _context.UserItem.FindAsync(id);

            if (userItem == null)
            {
                return NotFound();
            }

            return Ok(userItem);
        }

        // PUT: api/UserItems/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserItem([FromRoute] int id, [FromBody] UserItem userItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != userItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(userItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserItemExists(id))
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

        // POST: api/UserItems
        [HttpPost]
        public async Task<IActionResult> PostUserItem([FromBody] UserItem userItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (_context.UserItem.Where(p => p.Email.Equals(userItem.Email)).Count() != 0)
            {
                return Conflict();
            }

            if (_context.UserItem.Where(p => p.Username.Equals(userItem.Username)).Count() != 0)
            {
                return Conflict();
            }

            _context.UserItem.Add(userItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserItem", new { id = userItem.Id }, userItem);
        }

        // DELETE: api/UserItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserItem([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userItem = await _context.UserItem.FindAsync(id);
            if (userItem == null)
            {
                return NotFound();
            }

            _context.UserItem.Remove(userItem);
            await _context.SaveChangesAsync();

            return Ok(userItem);
        }

        private bool UserItemExists(int id)
        {
            return _context.UserItem.Any(e => e.Id == id);
        }
    }
}