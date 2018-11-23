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
    public class RelationshipsController : ControllerBase
    {
        private readonly PolaroidAPIContext _context;
        private IConfiguration _configuration;

        public RelationshipsController(PolaroidAPIContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: api/Relationships
        [HttpGet]
        public IEnumerable<Relationships> GetRelationships()
        {
            return _context.Relationships;
        }

        // GET: api/Relationships/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRelationships([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var relationships = await _context.Relationships.FindAsync(id);

            if (relationships == null)
            {
                return NotFound();
            }

            return Ok(relationships);
        }

        // PUT: api/Relationships/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRelationships([FromRoute] int id, [FromBody] Relationships relationships)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != relationships.Id)
            {
                return BadRequest();
            }

            _context.Entry(relationships).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RelationshipsExists(id))
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

        // POST: api/Relationships
        [HttpPost]
        public async Task<IActionResult> PostRelationships([FromForm] RelationshipSingleItem relationships)
        {

            // Same person cant follow himself
            if (relationships.Follows == relationships.Person)
            {
                return Conflict();
            }

            // Cannot follow someone again
            IQueryable<Relationships> initialDB = _context.Relationships.Where(p => p.Person == relationships.Person);
            IQueryable<Relationships> resultDB = initialDB.Where(p => p.Follows == relationships.Follows);
            if (resultDB.Count() != 0)
            {
                return Conflict();
            }

            Relationships submitData = new Relationships();

            submitData.Person = relationships.Person;
            submitData.Follows = relationships.Follows;

            _context.Relationships.Add(submitData);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRelationships", new { id = submitData.Id }, relationships);
        }

        // DELETE: api/Relationships/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRelationships([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var relationships = await _context.Relationships.FindAsync(id);
            if (relationships == null)
            {
                return NotFound();
            }

            _context.Relationships.Remove(relationships);
            await _context.SaveChangesAsync();

            return Ok(relationships);
        }

        private bool RelationshipsExists(int id)
        {
            return _context.Relationships.Any(e => e.Id == id);
        }

        // GET: api/relationships/person
        [HttpGet("following/{PersonID}")]
        public IEnumerable<Relationships> GetFollowing([FromRoute] int PersonID)
        {
            return _context.Relationships.Where(p => p.Person.Equals(PersonID));
        }

        // GET: api/relationships/person
        [HttpGet("followers/{PersonID}")]
        public IEnumerable<Relationships> GetFollowers([FromRoute] int PersonID)
        {
            return _context.Relationships.Where(p => p.Follows.Equals(PersonID));
        }

        // GET: api/relationships/person/{PersonID}/follows/{FollowerID}
        [HttpGet("person/{PersonID}/follows/{FollowerID}")]
        public IEnumerable<Relationships> CheckRelationship([FromRoute] int PersonID, int FollowerID)
        {
            IQueryable<Relationships> initialDB = _context.Relationships.Where(p => p.Person == PersonID);
            IQueryable<Relationships> resultDB = initialDB.Where(p => p.Follows == FollowerID);

            if (resultDB.Count() > 0)
            {
                return resultDB;
            }
            else
            {

                return new List<Relationships>();
            }
        }
    }
}