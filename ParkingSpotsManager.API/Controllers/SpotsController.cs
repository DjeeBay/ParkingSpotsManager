using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkingSpotsManager.Shared.Database;
using ParkingSpotsManager.Shared.Models;

namespace ParkingSpotsManager.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SpotsController : ControllerBase
    {
        private readonly DataContext _context;

        public SpotsController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Spots
        [HttpGet]
        public IEnumerable<Spot> GetSpots()
        {
            return _context.Spots;
        }

        // GET: api/Spots/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSpot([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var spot = await _context.Spots.FindAsync(id);

            if (spot == null)
            {
                return NotFound();
            }

            return Ok(spot);
        }

        // PUT: api/Spots/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSpot([FromRoute] int id, [FromBody] Spot spot)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != spot.Id)
            {
                return BadRequest();
            }

            _context.Entry(spot).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SpotExists(id))
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

        // POST: api/Spots
        [HttpPost]
        public async Task<IActionResult> PostSpot([FromBody] Spot spot)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Spots.Add(spot);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSpot", new { id = spot.Id }, spot);
        }

        // DELETE: api/Spots/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSpot([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var spot = await _context.Spots.FindAsync(id);
            if (spot == null)
            {
                return NotFound();
            }

            _context.Spots.Remove(spot);
            await _context.SaveChangesAsync();

            return Ok(spot);
        }

        private bool SpotExists(int id)
        {
            return _context.Spots.Any(e => e.Id == id);
        }
    }
}