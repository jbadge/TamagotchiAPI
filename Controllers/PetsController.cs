using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
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
        private readonly string _adminVisitorId;

        private string _visitorId;
        private bool? _isAdmin;

        private string VisitorId => _visitorId ??= Request.Headers["x-visitor-id"].FirstOrDefault();
        private bool IsAdmin => _isAdmin ??= VisitorId == _adminVisitorId;

        // Constructor that receives a reference to your database context
        // and stores it in _context for you to use in your API methods
        public PetsController(DatabaseContext context, IConfiguration config)
        {
            _context = context;
            _adminVisitorId = config["AdminVisitorId"];
        }

        // GET: api/Pets
        //
        // Returns a list of all your Pets
        //
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pet>>> GetPets(
            bool isDead = false
            )
        {
            if (string.IsNullOrEmpty(VisitorId) && !IsAdmin)
            {
                return Unauthorized();
            }

            await using var transaction = await SetVisitorContextAsync();

            var allPets = await _context.Pets
            .Include(pet => pet.Playtimes)
            .Include(pet => pet.Feedings)
            .Include(pet => pet.Scoldings)
            .Where(pet => IsAdmin || pet.VisitorId == VisitorId || pet.VisitorId == null)
            .ToListAsync();

            foreach (var item in allPets)
            {
                if (item.LastInteractedWithDate != DateTime.MinValue)
                {
                    item.IsDeadMethod();
                }
            }

            await transaction.CommitAsync();

            if (IsAdmin)
            {
                // Admin gets all pets
                return allPets
                .OrderBy(pets => pets.Id)
                .ToList();
            }

            // Uses the database context in `_context` to request all of the Pets, sort
            // them by row id and return them as a JSON array.
            return allPets
            .Where(pet => pet.VisitorId == VisitorId || pet.VisitorId == null)
            .OrderBy(row => row.Id)
            .ToList();
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
            if (string.IsNullOrEmpty(VisitorId) && !IsAdmin)
            {
                return Unauthorized();
            }

            await using var transaction = await SetVisitorContextAsync();

            var pet = await _context.Pets
                .Where(pet => (IsAdmin || pet.VisitorId == VisitorId || pet.VisitorId == null) && pet.Id == id)
                .Include(pet => pet.Playtimes)
                .Include(pet => pet.Feedings)
                .Include(pet => pet.Scoldings)
                .FirstOrDefaultAsync();

            if (pet == null)
            {
                return NotFound();
            }

            await transaction.CommitAsync();

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
            if (string.IsNullOrEmpty(VisitorId) && !IsAdmin)
            {
                return Unauthorized();
            }

            await using var transaction = await SetVisitorContextAsync();

            if (id != pet.Id)
            {
                return BadRequest();
            }

            var existingPet = await _context.Pets.FirstOrDefaultAsync(p => p.Id == id && (IsAdmin || p.VisitorId == VisitorId));

            if (existingPet == null)
            {
                return NotFound();
            }

            existingPet.Name = pet.Name;
            existingPet.HungerLevel = pet.HungerLevel;
            existingPet.HappinessLevel = pet.HappinessLevel;
            existingPet.LastInteractedWithDate = pet.LastInteractedWithDate;

            try
            {
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PetExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

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
            if (string.IsNullOrEmpty(VisitorId) && !IsAdmin)
            {
                return Unauthorized();
            }

            await using var transaction = await SetVisitorContextAsync();


            pet.VisitorId = VisitorId;

            _context.Pets.Add(pet);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

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
            if (string.IsNullOrEmpty(VisitorId) && !IsAdmin)
            {
                return Unauthorized();
            }

            await using var transaction = await SetVisitorContextAsync();

            // Find this pet by looking for the specific id
            var pet = await _context.Pets.FirstOrDefaultAsync(pet => pet.Id == id && (IsAdmin || pet.VisitorId == VisitorId));

            if (pet == null)
            {
                // There wasn't a pet with that id so return a `404` not found
                return NotFound();
            }

            if (pet.VisitorId == null)
            {
                // Global pet - do not allow edits or deletion
                return Forbid();
            }

            // Tell the database we want to remove this record
            _context.Pets.Remove(pet);

            // Tell the database to perform the deletion
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            // Return a copy of the deleted data
            return Ok(pet);
        }

        // Add playtimes to a pet
        // POST: /api/Pets/5/Playtimes
        [HttpPost("{id}/Playtimes")]
        public async Task<ActionResult<Playtime>> CreatePlaytimeForPet(int id)
        {
            if (string.IsNullOrEmpty(VisitorId) && !IsAdmin)
            {
                return Unauthorized();
            }

            await using var transaction = await SetVisitorContextAsync();

            var pet = await _context.Pets.FirstOrDefaultAsync(pet => pet.Id == id && (pet.VisitorId == VisitorId || pet.VisitorId == null));

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
            _context.Entry(pet).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            // Return the new playtime to the response of the API
            return Ok(playtime);
        }

        // Add feedings to a pet
        // POST: /api/Pets/5/Feedings
        [HttpPost("{id}/Feedings")]
        public async Task<ActionResult<Feeding>> CreateFeedingForPet(int id)
        {
            if (string.IsNullOrEmpty(VisitorId) && !IsAdmin)
            {
                return Unauthorized();
            }

            await using var transaction = await SetVisitorContextAsync();

            var pet = await _context.Pets.FirstOrDefaultAsync(pet => pet.Id == id && (pet.VisitorId == VisitorId || pet.VisitorId == null));

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
            _context.Entry(pet).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return Ok(feeding);
        }

        // Add scoldings to a pet
        // POST: /api/Pets/5/Scoldings
        [HttpPost("{id}/Scoldings")]
        public async Task<ActionResult<Scolding>> CreateScoldingForPet(int id)
        {
            if (string.IsNullOrEmpty(VisitorId) && !IsAdmin)
            {
                return Unauthorized();
            }

            await using var transaction = await SetVisitorContextAsync();

            var pet = await _context.Pets.FirstOrDefaultAsync(pet => pet.Id == id && (pet.VisitorId == VisitorId || pet.VisitorId == null));

            if (pet == null)
            {
                return NotFound();
            }

            var scolding = new Scolding();
            scolding.PetId = pet.Id;
            pet.HappinessLevel -= 5;
            pet.LastInteractedWithDate = DateTime.UtcNow;

            _context.Scoldings.Add(scolding);
            _context.Entry(pet).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return Ok(scolding);
        }

        // Method to allow pinging to keep from winding down
        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok("OK");
        }

        private async Task<IDbContextTransaction> SetVisitorContextAsync()
        {
            var conn = _context.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open)
            {
                await conn.OpenAsync();
            }

            var transaction = await _context.Database.BeginTransactionAsync();

            if (!IsAdmin && !string.IsNullOrEmpty(VisitorId))
            {
                await _context.Database.ExecuteSqlRawAsync("SET ROLE visitor_role");
                var sql = $"SET LOCAL \"request.jwt.claim.sub\" = '{VisitorId.Replace("'", "''")}'";
                await _context.Database.ExecuteSqlRawAsync(sql);
            }
            else if (IsAdmin)
            {
                await _context.Database.ExecuteSqlRawAsync("SET ROLE admin_role");
            }

            return transaction;
        }

        // Private helper method that looks up an existing pet by the supplied id
        private bool PetExists(int id)
        {
            return _context.Pets.Any(pet => pet.Id == id && (IsAdmin || pet.VisitorId == VisitorId));
        }

    }
}
