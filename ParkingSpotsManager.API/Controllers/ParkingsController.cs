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
    public class ParkingsController : ControllerBase
    {
        private readonly DataContext _context;

        public ParkingsController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Parkings
        [HttpGet]
        public IEnumerable<Parking> GetParkings()
        {
            return _context.Parkings;
        }
        
        // GET: api/Parkings/GetUserParkings
        [HttpGet]
        [Route("[action]")]
        public IEnumerable<Parking> GetUserParkings()
        {
            var userID = User.Identity.Name;
            var userParkingsList = _context.UsersParkings.Where(up => up.UserId == int.Parse(userID)).ToList();
            var parkingsList = _context.Parkings.Where(p => userParkingsList.Any(up => up.ParkingId == p.Id)).ToList();
            parkingsList.ForEach(p => {
                p.IsCurrentUserAdmin = userParkingsList.Where(up => up.ParkingId == p.Id).SingleOrDefault().IsAdmin == 1;
            });
            return parkingsList;
        }

        // GET: api/Parkings/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetParking([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var parking = await _context.Parkings.FindAsync(id);

            if (parking == null)
            {
                return NotFound();
            }

            parking.IsCurrentUserAdmin = _context.UsersParkings.Where(up => up.ParkingId == parking.Id && up.UserId == int.Parse(User.Identity.Name)).SingleOrDefault().IsAdmin == 1;

            return Ok(parking);
        }

        // PUT: api/Parkings/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutParking([FromRoute] int id, [FromBody] Parking parking)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != parking.Id)
            {
                return BadRequest();
            }

            _context.Entry(parking).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ParkingExists(id))
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

        // POST: api/Parkings
        [HttpPost]
        public async Task<IActionResult> PostParking([FromBody] Parking parking)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Parkings.Add(parking);
            await _context.SaveChangesAsync();
            _context.UsersParkings.Add(new UserParking { UserId = int.Parse(User.Identity.Name), ParkingId = parking.Id, IsAdmin = 1 });
            await _context.SaveChangesAsync();
            parking.IsCurrentUserAdmin = true;

            return CreatedAtAction("GetParking", new { id = parking.Id }, parking);
        }

        // DELETE: api/Parkings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteParking([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var parking = await _context.Parkings.FindAsync(id);
            if (parking == null)
            {
                return NotFound();
            }

            _context.Parkings.Remove(parking);
            await _context.SaveChangesAsync();

            return Ok(parking);
        }

        private bool ParkingExists(int id)
        {
            return _context.Parkings.Any(e => e.Id == id);
        }
    }
}