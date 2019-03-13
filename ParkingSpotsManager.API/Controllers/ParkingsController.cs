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
            var parkingsList = _context.Parkings.Include(p => p.Spots).Where(p => userParkingsList.Any(up => up.ParkingId == p.Id)).ToList();
            parkingsList.ForEach(p => {
                p.IsCurrentUserAdmin = userParkingsList.Where(up => up.ParkingId == p.Id).SingleOrDefault().IsAdmin == 1;
                p.Spots.ForEach(s => s.Occupier = _context.Users.Find(s.OccupiedBy));
            });
            return parkingsList;
        }

        // GET: api/Parkings/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetParking([FromRoute] int id)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var parkings = await _context.Parkings.Include(p => p.Spots).Include(p => p.UserParkings).ThenInclude(up => up.User).ToListAsync();
            var parking = parkings.FirstOrDefault(p => p.Id == id);

            if (parking == null) {
                return NotFound();
            }

            foreach (var spot in parking.Spots) {
                spot.Occupier = _context.Users.Find(spot.OccupiedBy);
            }

            parking.IsCurrentUserAdmin = _context.UsersParkings.Where(up => up.ParkingId == parking.Id && up.UserId == int.Parse(User.Identity.Name)).SingleOrDefault().IsAdmin == 1;

            return Ok(parking);
        }

        // PUT: api/Parkings/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutParking([FromRoute] int id, [FromBody] Parking parking)
        {
            if (!IsAdmin(parking)) {
                return BadRequest();
            }

            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (id != parking.Id) {
                return BadRequest();
            }

            _context.Entry(parking).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!ParkingExists(id)) {
                    return NotFound();
                }
                else {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Parkings
        [HttpPost]
        public async Task<IActionResult> PostParking([FromBody] Parking parking)
        {
            if (!ModelState.IsValid) {
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
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var parking = await _context.Parkings.FindAsync(id);
            if (parking == null || !IsAdmin(parking)) {
                return NotFound();
            }

            _context.Parkings.Remove(parking);
            await _context.SaveChangesAsync();

            return Ok(parking);
        }

        [HttpPost]
        [Route("[action]/{parkingID}")]
        public async Task<IActionResult> ChangeUserRole([FromRoute] int parkingID, [FromBody] UserParking userParking)
        {
            var userID = User.Identity.Name;
            if (ParkingExists(parkingID) && IsAdmin(parkingID) && int.Parse(userID) != userParking.UserId) {
                var storedUserParking = _context.UsersParkings.AsNoTracking().Where(up => up.Id == userParking.Id).FirstOrDefault();
                if (storedUserParking != null) {
                    storedUserParking.IsAdmin = userParking.IsAdmin;
                    await _context.SaveChangesAsync();

                    return Ok(_context.UsersParkings.Include(up => up.User).ToList());
                }
            }

            return BadRequest();
        }

        [HttpGet]
        [Route("[action]/{parkingID}/{userID}")]
        public async Task<IActionResult> RemoveUser([FromRoute] int parkingID, [FromRoute] int userID)
        {
            var senderID = User.Identity.Name;
            if (ParkingExists(parkingID) && IsAdmin(parkingID) && int.Parse(senderID) != userID) {
                var storedUserParking = _context.UsersParkings.AsNoTracking().Where(up => up.UserId == userID && up.ParkingId == parkingID).FirstOrDefault();
                if (storedUserParking != null) {
                    _context.UsersParkings.Remove(storedUserParking);
                    await _context.SaveChangesAsync();

                    return Ok(_context.UsersParkings.Include(up => up.User).ToList());
                }
            }

            return BadRequest();
        }

        [HttpGet]
        [Route("[action]/{parkingID}/{userID}")]
        public async Task<IActionResult> SendInvitation([FromRoute] int parkingID, [FromRoute] int userID)
        {
            var parking = _context.Parkings.FirstOrDefault(p => p.Id == parkingID);
            var user = _context.Users.FirstOrDefault(u => u.Id == userID);
            if (parking == null || user == null) {
                return BadRequest();
            }

            var existingUserParking = _context.UsersParkings.Where(up => up.UserId == userID && up.ParkingId == parkingID).FirstOrDefault();
            if (existingUserParking != null) {
                return BadRequest();
            }

            try {
                var userParking = new UserParking {
                    UserId = userID,
                    ParkingId = parkingID,
                    IsAdmin = 0
                };
                _context.UsersParkings.Add(userParking);
                await _context.SaveChangesAsync();

                return Ok();
            } catch (DbUpdateConcurrencyException) {
                throw;
            }
        }

        private bool ParkingExists(int id)
        {
            return _context.Parkings.Any(e => e.Id == id);
        }

        private bool IsAdmin(Parking parking)
        {
            var userID = User.Identity.Name;
            var userParking = _context.UsersParkings.Where(up => up.ParkingId == parking.Id && up.UserId == int.Parse(userID) && up.IsAdmin == 1).FirstOrDefault();

            return userParking != null;
        }

        private bool IsAdmin(int parkingID)
        {
            var userID = User.Identity.Name;
            var userParking = _context.UsersParkings.AsNoTracking().Where(up => up.ParkingId == parkingID && up.UserId == int.Parse(userID) && up.IsAdmin == 1).FirstOrDefault();

            return userParking != null;
        }
    }
}