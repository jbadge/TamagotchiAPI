using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TamagotchiAPI.Models;

namespace TamagotchiAPI.Controllers
{
    // All of these routes will be at the base URL:     /api/Scolding
    // That is what "api/[controller]" means below. It uses the name of the controller
    // in this case ScoldingController to determine the URL
    [Route("api/[controller]")]
    [ApiController]
    public class ScoldingController : ControllerBase
    {
        // This is the variable you use to have access to your database
        private readonly DatabaseContext _context;

        // Constructor that receives a reference to your database context
        // and stores it in _context for you to use in your API methods
        public ScoldingController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/Scolding
        //
        // Returns a list of all your Scoldings
        //
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Scolding>>> GetScoldings()
        {
            // Uses the database context in `_context` to request all of the Scoldings, sort
            // them by row id and return them as a JSON array.
            return await _context.Scoldings.OrderBy(row => row.Id).ToListAsync();
        }

        // GET: api/Scolding/5
        //
        // Fetches and returns a specific scolding by finding it by id. The id is specified in the
        // URL. In the sample URL above it is the `5`.  The "{id}" in the [HttpGet("{id}")] is what tells dotnet
        // to grab the id from the URL. It is then made available to us as the `id` argument to the method.
        //
        [HttpGet("{id}")]
        public async Task<ActionResult<Scolding>> GetScolding(int id)
        {
            // Find the scolding in the database using `FindAsync` to look it up by id
            var scolding = await _context.Scoldings.FindAsync(id);

            // If we didn't find anything, we receive a `null` in return
            if (scolding == null)
            {
                // Return a `404` response to the client indicating we could not find a scolding with this id
                return NotFound();
            }

            // Return the scolding as a JSON object.
            return scolding;
        }

        // PUT: api/Scolding/5
        //
        // Update an individual scolding with the requested id. The id is specified in the URL
        // In the sample URL above it is the `5`. The "{id} in the [HttpPut("{id}")] is what tells dotnet
        // to grab the id from the URL. It is then made available to us as the `id` argument to the method.
        //
        // In addition the `body` of the request is parsed and then made available to us as a Scolding
        // variable named scolding. The controller matches the keys of the JSON object the client
        // supplies to the names of the attributes of our Scolding POCO class. This represents the
        // new values for the record.
        //
        [HttpPut("{id}")]
        public async Task<IActionResult> PutScolding(int id, Scolding scolding)
        {
            // If the ID in the URL does not match the ID in the supplied request body, return a bad request
            if (id != scolding.Id)
            {
                return BadRequest();
            }

            // Tell the database to consider everything in scolding to be _updated_ values. When
            // the save happens the database will _replace_ the values in the database with the ones from scolding
            _context.Entry(scolding).State = EntityState.Modified;

            try
            {
                // Try to save these changes.
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Ooops, looks like there was an error, so check to see if the record we were
                // updating no longer exists.
                if (!ScoldingExists(id))
                {
                    // If the record we tried to update was already deleted by someone else,
                    // return a `404` not found
                    return NotFound();
                }
                else
                {
                    // Otherwise throw the error back, which will cause the request to fail
                    // and generate an error to the client.
                    throw;
                }
            }

            // Return a copy of the updated data
            return Ok(scolding);
        }

        // POST: api/Scolding
        //
        // Creates a new scolding in the database.
        //
        // The `body` of the request is parsed and then made available to us as a Scolding
        // variable named scolding. The controller matches the keys of the JSON object the client
        // supplies to the names of the attributes of our Scolding POCO class. This represents the
        // new values for the record.
        //
        [HttpPost]
        public async Task<ActionResult<Scolding>> PostScolding(Scolding scolding)
        {
            // Indicate to the database context we want to add this new record
            _context.Scoldings.Add(scolding);
            await _context.SaveChangesAsync();

            // Return a response that indicates the object was created (status code `201`) and some additional
            // headers with details of the newly created object.
            return CreatedAtAction("GetScolding", new { id = scolding.Id }, scolding);
        }

        // DELETE: api/Scolding/5
        //
        // Deletes an individual scolding with the requested id. The id is specified in the URL
        // In the sample URL above it is the `5`. The "{id} in the [HttpDelete("{id}")] is what tells dotnet
        // to grab the id from the URL. It is then made available to us as the `id` argument to the method.
        //
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteScolding(int id)
        {
            // Find this scolding by looking for the specific id
            var scolding = await _context.Scoldings.FindAsync(id);
            if (scolding == null)
            {
                // There wasn't a scolding with that id so return a `404` not found
                return NotFound();
            }

            // Tell the database we want to remove this record
            _context.Scoldings.Remove(scolding);

            // Tell the database to perform the deletion
            await _context.SaveChangesAsync();

            // Return a copy of the deleted data
            return Ok(scolding);
        }

        // Private helper method that looks up an existing scolding by the supplied id
        private bool ScoldingExists(int id)
        {
            return _context.Scoldings.Any(scolding => scolding.Id == id);
        }
    }
}
