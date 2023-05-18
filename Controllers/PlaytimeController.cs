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
    // All of these routes will be at the base URL:     /api/Playtime
    // That is what "api/[controller]" means below. It uses the name of the controller
    // in this case PlaytimeController to determine the URL
    [Route("api/[controller]")]
    [ApiController]
    public class PlaytimeController : ControllerBase
    {
        // This is the variable you use to have access to your database
        private readonly DatabaseContext _context;

        // Constructor that receives a reference to your database context
        // and stores it in _context for you to use in your API methods
        public PlaytimeController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/Playtime
        //
        // Returns a list of all your Playtimes
        //
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Playtime>>> GetPlaytimes()
        {
            // Uses the database context in `_context` to request all of the Playtimes, sort
            // them by row id and return them as a JSON array.
            return await _context.Playtimes.OrderBy(row => row.Id).ToListAsync();
        }

        // GET: api/Playtime/5
        //
        // Fetches and returns a specific playtime by finding it by id. The id is specified in the
        // URL. In the sample URL above it is the `5`.  The "{id}" in the [HttpGet("{id}")] is what tells dotnet
        // to grab the id from the URL. It is then made available to us as the `id` argument to the method.
        //
        [HttpGet("{id}")]
        public async Task<ActionResult<Playtime>> GetPlaytime(int id)
        {
            // Find the playtime in the database using `FindAsync` to look it up by id
            var playtime = await _context.Playtimes.FindAsync(id);

            // If we didn't find anything, we receive a `null` in return
            if (playtime == null)
            {
                // Return a `404` response to the client indicating we could not find a playtime with this id
                return NotFound();
            }

            // Return the playtime as a JSON object.
            return playtime;
        }

        // PUT: api/Playtime/5
        //
        // Update an individual playtime with the requested id. The id is specified in the URL
        // In the sample URL above it is the `5`. The "{id} in the [HttpPut("{id}")] is what tells dotnet
        // to grab the id from the URL. It is then made available to us as the `id` argument to the method.
        //
        // In addition the `body` of the request is parsed and then made available to us as a Playtime
        // variable named playtime. The controller matches the keys of the JSON object the client
        // supplies to the names of the attributes of our Playtime POCO class. This represents the
        // new values for the record.
        //
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlaytime(int id, Playtime playtime)
        {
            // If the ID in the URL does not match the ID in the supplied request body, return a bad request
            if (id != playtime.Id)
            {
                return BadRequest();
            }

            // Tell the database to consider everything in playtime to be _updated_ values. When
            // the save happens the database will _replace_ the values in the database with the ones from playtime
            _context.Entry(playtime).State = EntityState.Modified;

            try
            {
                // Try to save these changes.
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Ooops, looks like there was an error, so check to see if the record we were
                // updating no longer exists.
                if (!PlaytimeExists(id))
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
            return Ok(playtime);
        }

        // POST: api/Playtime
        //
        // Creates a new playtime in the database.
        //
        // The `body` of the request is parsed and then made available to us as a Playtime
        // variable named playtime. The controller matches the keys of the JSON object the client
        // supplies to the names of the attributes of our Playtime POCO class. This represents the
        // new values for the record.
        //
        [HttpPost]
        public async Task<ActionResult<Playtime>> PostPlaytime(Playtime playtime)
        {
            // Indicate to the database context we want to add this new record
            _context.Playtimes.Add(playtime);
            await _context.SaveChangesAsync();

            // Return a response that indicates the object was created (status code `201`) and some additional
            // headers with details of the newly created object.
            return CreatedAtAction("GetPlaytime", new { id = playtime.Id }, playtime);
        }

        // DELETE: api/Playtime/5
        //
        // Deletes an individual playtime with the requested id. The id is specified in the URL
        // In the sample URL above it is the `5`. The "{id} in the [HttpDelete("{id}")] is what tells dotnet
        // to grab the id from the URL. It is then made available to us as the `id` argument to the method.
        //
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlaytime(int id)
        {
            // Find this playtime by looking for the specific id
            var playtime = await _context.Playtimes.FindAsync(id);
            if (playtime == null)
            {
                // There wasn't a playtime with that id so return a `404` not found
                return NotFound();
            }

            // Tell the database we want to remove this record
            _context.Playtimes.Remove(playtime);

            // Tell the database to perform the deletion
            await _context.SaveChangesAsync();

            // Return a copy of the deleted data
            return Ok(playtime);
        }

        // Private helper method that looks up an existing playtime by the supplied id
        private bool PlaytimeExists(int id)
        {
            return _context.Playtimes.Any(playtime => playtime.Id == id);
        }
    }
}
