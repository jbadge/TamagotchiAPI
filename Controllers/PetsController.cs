using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using TamagotchiAPI.Models;

namespace TamagotchiAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PetsController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly string _adminVisitorId;

        private string _visitorId;
        private bool? _isAdmin;

        private string VisitorId => _visitorId ??= Request.Headers["x-visitor-id"].FirstOrDefault();
        private bool IsAdmin => _isAdmin ??= VisitorId == _adminVisitorId;

        public PetsController(DatabaseContext context, IConfiguration config)
        {
            _context = context;
            _adminVisitorId = config["AdminVisitorId"];
        }

        // GET: api/Pets
        // Returns a list of all user's Pets
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

            return allPets
            .Where(pet => pet.VisitorId == VisitorId || pet.VisitorId == null)
            .OrderBy(row => row.Id)
            .ToList();
        }

        // GET: api/Pets/5
        // Fetches and returns a specific pet by finding it by id. 
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
        // Update an individual pet with the requested id. 
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
        // Creates a new pet in the database.
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
        // Deletes an individual pet with the requested id. 
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePet(int id)
        {
            if (string.IsNullOrEmpty(VisitorId) && !IsAdmin)
            {
                return Unauthorized();
            }

            await using var transaction = await SetVisitorContextAsync();

            var pet = await _context.Pets.FirstOrDefaultAsync(pet => pet.Id == id && (IsAdmin || pet.VisitorId == VisitorId));

            if (pet == null)
            {
                return NotFound();
            }

            if (pet.VisitorId == null)
            {
                // Global pet - do not allow edits or deletion
                return Forbid();
            }

            _context.Pets.Remove(pet);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

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

            if (pet == null)
            {
                return NotFound();
            }

            var playtime = new Playtime();
            playtime.PetId = pet.Id;

            pet.HungerLevel += 3;
            pet.HappinessLevel += 5;
            pet.LastInteractedWithDate = DateTime.UtcNow;

            _context.Playtimes.Add(playtime);
            _context.Entry(pet).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

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

        // Method to allow pinging to keep from spinning down
        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok("OK");
        }

        // Helper function that sets role to visitor or admin
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

        // Helper function to look up an existing pet by the supplied id
        private bool PetExists(int id)
        {
            return _context.Pets.Any(pet => pet.Id == id && (IsAdmin || pet.VisitorId == VisitorId));
        }

    }
}
