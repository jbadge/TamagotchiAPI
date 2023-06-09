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
    // All of these routes will be at the base URL:     /api/Pets
    // That is what "api/[controller]" means below. It uses the name of the controller
    // in this case PetsController to determine the URL
    [Route("api/[controller]")]
    [ApiController]
    public class PetsController : ControllerBase
    {
        // This is the variable you use to have access to your database
        private readonly DatabaseContext _context;

        // Constructor that receives a reference to your database context
        // and stores it in _context for you to use in your API methods
        public PetsController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/Pets
        //
        // Returns a list of all your Pets
        //
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pet>>> GetPets(bool isDead = false)
        {

            foreach (var item in _context.Pets) { if (item.LastInteractedWithDate != DateTime.MinValue) { item.IsDeadMethod(); } }

            // Uses the database context in `_context` to request all of the Pets, sort
            // them by row id and return them as a JSON array.
            return await _context.Pets.Where(x => x.IsDead == isDead).OrderBy(row => row.Id).Include(pet => pet.Playtimes).Include(pet => pet.Feedings).Include(pet => pet.Scoldings).ToListAsync();
        }

        // GET: api/Pets/5
        //
        // Fetches and returns a specific pet by finding it by id. The id is specified in the
        // URL. In the sample URL above it is the `5`.  The "{id}" in the [HttpGet("{id}")] is what tells dotnet
        // to grab the id from the URL. It is then made available to us as the `id` argument to the method.
        //
        [HttpGet("{id}")]
        public async Task<ActionResult<Pet>> GetPet(int id)
        {
            // Find the pet in the database using `FindAsync` to look it up by id
            // var pet = await _context.Pets.FindAsync(id);
            var pet = await _context.Pets.Include(pet => pet.Playtimes).Include(pet => pet.Feedings).Include(pet => pet.Scoldings).FirstOrDefaultAsync(x => x.Id == id);

            // If we didn't find anything, we receive a `null` in return
            if (pet == null)
            {
                // Return a `404` response to the client indicating we could not find a pet with this id
                return NotFound();
            }

            // Return the pet as a JSON object.
            return pet;
        }

        // PUT: api/Pets/5
        //
        // Update an individual pet with the requested id. The id is specified in the URL
        // In the sample URL above it is the `5`. The "{id} in the [HttpPut("{id}")] is what tells dotnet
        // to grab the id from the URL. It is then made available to us as the `id` argument to the method.
        //
        // In addition the `body` of the request is parsed and then made available to us as a Pet
        // variable named pet. The controller matches the keys of the JSON object the client
        // supplies to the names of the attributes of our Pet POCO class. This represents the
        // new values for the record.
        //
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPet(int id, Pet pet)
        {
            // If the ID in the URL does not match the ID in the supplied request body, return a bad request
            if (id != pet.Id)
            {
                return BadRequest();
            }

            // Tell the database to consider everything in pet to be _updated_ values. When
            // the save happens the database will _replace_ the values in the database with the ones from pet
            _context.Entry(pet).State = EntityState.Modified;

            try
            {
                // Try to save these changes.
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Ooops, looks like there was an error, so check to see if the record we were
                // updating no longer exists.
                if (!PetExists(id))
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
            return Ok(pet);
        }

        // POST: api/Pets
        //
        // Creates a new pet in the database.
        //
        // The `body` of the request is parsed and then made available to us as a Pet
        // variable named pet. The controller matches the keys of the JSON object the client
        // supplies to the names of the attributes of our Pet POCO class. This represents the
        // new values for the record.
        //
        [HttpPost]
        public async Task<ActionResult<Pet>> PostPet(Pet pet)
        {
            // Indicate to the database context we want to add this new record
            _context.Pets.Add(pet);
            await _context.SaveChangesAsync();

            // Return a response that indicates the object was created (status code `201`) and some additional
            // headers with details of the newly created object.
            return CreatedAtAction("GetPet", new { id = pet.Id }, pet);
        }

        // DELETE: api/Pets/5
        //
        // Deletes an individual pet with the requested id. The id is specified in the URL
        // In the sample URL above it is the `5`. The "{id} in the [HttpDelete("{id}")] is what tells dotnet
        // to grab the id from the URL. It is then made available to us as the `id` argument to the method.
        //
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePet(int id)
        {
            // Find this pet by looking for the specific id
            var pet = await _context.Pets.FindAsync(id);
            if (pet == null)
            {
                // There wasn't a pet with that id so return a `404` not found
                return NotFound();
            }

            // Tell the database we want to remove this record
            _context.Pets.Remove(pet);

            // Tell the database to perform the deletion
            await _context.SaveChangesAsync();

            // Return a copy of the deleted data
            return Ok(pet);
        }

        // Add playtimes to a pet
        // POST: /api/Pets/5/Playtimes
        [HttpPost("{id}/Playtimes")]
        public async Task<ActionResult<Playtime>> CreatePlaytimeForPet(int id)
        {
            // First, lets find the pet (by using the ID)
            var pet = await _context.Pets.FindAsync(id);

            // If the pet doesn't exist: return a 404 Not Found.
            if (pet == null)
            {
                // Return a '404' response to the client indicating we could not find a pet with this id
                return NotFound();
            }

            var playtime = new Playtime();
            // Associate the playtime to the given pet.
            playtime.PetId = pet.Id;
            pet.HungerLevel += 3;
            pet.HappinessLevel += 5;
            pet.LastInteractedWithDate = DateTime.UtcNow;

            _context.Playtimes.Add(playtime);
            await _context.SaveChangesAsync();

            // Return the new playtime to the response of the API
            return Ok(playtime);
        }

        // Add feedings to a pet
        // POST: /api/Pet/5/Feedings
        [HttpPost("{id}/Feedings")]
        public async Task<ActionResult<Feeding>> CreateFeedingForPet(int id)
        {
            var pet = await _context.Pets.FindAsync(id);

            if (pet == null)
            {
                return NotFound();
            }

            var feeding = new Feeding();
            feeding.PetId = pet.Id;
            pet.HungerLevel -= 5;
            pet.HappinessLevel += 3;
            pet.LastInteractedWithDate = DateTime.UtcNow;

            _context.Feedings.Add(feeding);
            await _context.SaveChangesAsync();

            return Ok(feeding);
        }

        // Add scoldings to a pet
        // POST: /api/Pet/5/Scoldings
        [HttpPost("{id}/Scoldings")]
        public async Task<ActionResult<Scolding>> CreateScoldingForPet(int id)
        {
            var pet = await _context.Pets.FindAsync(id);

            if (pet == null)
            {
                return NotFound();
            }

            var scolding = new Scolding();
            scolding.PetId = pet.Id;
            pet.HappinessLevel -= 5;
            pet.LastInteractedWithDate = DateTime.UtcNow;

            _context.Scoldings.Add(scolding);
            await _context.SaveChangesAsync();

            return Ok(scolding);
        }

        // Private helper method that looks up an existing pet by the supplied id
        private bool PetExists(int id)
        {
            return _context.Pets.Any(pet => pet.Id == id);
        }
    }
}
