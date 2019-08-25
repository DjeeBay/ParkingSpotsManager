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

        //GET: api/spots/GetParkingSpots/11
        [Route("[action]/{parkingID}")]
        [HttpGet("{parkingID}")]
        public async Task<IActionResult> GetParkingSpots([FromRoute] int parkingID)
        {
            var parking = await _context.Parkings.Include(p => p.Spots).Where(p => p.Id == parkingID).FirstOrDefaultAsync();
            if (parking != null && IsParkingUser(parking)) {
                parking.IsCurrentUserAdmin = IsParkingAdmin(parking);
                foreach (var spot in parking.Spots) {
                    spot.IsCurrentUserAdmin = IsParkingAdmin(spot);
                    if (spot.OccupiedBy != null) {
                        spot.Occupier = _context.Users.Find(spot.OccupiedBy);
                    }
                }

                return Ok(parking.Spots);
            }

            return BadRequest();
        }

        // GET: api/Spots/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSpot([FromRoute] int id)
        {
            //TODO if user is linked
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var spot = _context.Spots.Where(s => s.Id == id).FirstOrDefault();

            if (spot == null) {
                return NotFound();
            }

            return Ok(spot);
        }

        //GET: api/Spots/SetDefaultOccupier/5/2
        [HttpGet("[action]/{spotID}/{userID}")]
        public async Task<IActionResult> SetDefaultOccupier([FromRoute] int spotID, [FromRoute] int userID)
        {
            var spot = _context.Spots.Where(s => s.Id == spotID).FirstOrDefault();
            var user = _context.Users.AsNoTracking().Where(u => u.Id == userID).FirstOrDefault();

            if (spot != null && user != null) {
                spot.OccupiedByDefaultBy = userID;
                spot.OccupiedBy = userID;
                spot.IsOccupiedByDefault = true;
                await _context.SaveChangesAsync();
                //TODO remove password

                return Ok(_context.Spots.Include("Occupier").Where(s => s.Id == spot.Id).FirstOrDefault());
            }

            return BadRequest();
        }
        
        //GET: api/Spots/GetDefaultOccupier/2
        [HttpGet("[action]/{spotID}")]
        public async Task<IActionResult> GetDefaultOccupier([FromRoute] int spotID)
        {
            var spot = _context.Spots.Where(s => s.Id == spotID).FirstOrDefault();
            if (spot != null && spot.OccupiedByDefaultBy != null) {
                var user = _context.Users.AsNoTracking().Where(u => u.Id == spot.OccupiedByDefaultBy).FirstOrDefault();
                if (user != null) {
                    user.Password = null;

                    return Ok(user);
                }
            }

            return BadRequest();
        }

        // PUT: api/Spots/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSpot([FromRoute] int id, [FromBody] Spot spot)
        {
            if (!IsParkingAdmin(spot)) {
                return BadRequest();
            }
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            if (id != spot.Id) {
                return BadRequest();
            }
            _context.Entry(spot).State = EntityState.Modified;

            try {
                if (!spot.IsOccupiedByDefault) {
                    spot.OccupiedByDefaultBy = null;
                }
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) {
                if (!SpotExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Spots
        [HttpPost]
        public async Task<IActionResult> PostSpot([FromBody] Spot spot)
        {
            if (!IsParkingAdmin(spot.ParkingId)) {
                return BadRequest();
            }

            if (!ModelState.IsValid) {
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

            if (!IsParkingAdmin(spot)) {
                return BadRequest();
            }

            var storedSpot = _context.Spots.Where(s => s.Id == spot.Id).FirstOrDefault();

            storedSpot.IsDeleted = true;
            storedSpot.DeletedBy = _context.UserId;
            await _context.SaveChangesAsync();

            return Ok(storedSpot);
        }

        // PUT: api/Spots/5/ChangeStatus
        [HttpPut]
        [Route("{id}/[action]")]
        public async Task<IActionResult> ChangeStatus([FromBody] Spot spot)
        {
            if (!IsParkingUser(spot)) {
                return BadRequest();
            }

            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (CanChangeStatus(spot)) {
                var storedSpot = _context.Spots.FirstOrDefault(s => s.Id == spot.Id);
                if (storedSpot != null) {
                    try {
                        if (spot.OccupiedAt == null) {
                            if (storedSpot.OccupiedAt != null && storedSpot.OccupiedBy != null) {
                                storedSpot.ReleasedAt = DateTime.Now;
                            }
                            storedSpot.OccupiedBy = null;
                            storedSpot.OccupiedAt = null;
                        } else {
                            storedSpot.OccupiedBy = int.Parse(User.Identity.Name);
                            storedSpot.OccupiedAt = DateTime.Now;
                            storedSpot.ReleasedAt = null;
                        }
                        var entries = await _context.SaveChangesAsync();
                        var spots = _context.Spots.Where(s => s.ParkingId == storedSpot.ParkingId).ToList();
                        foreach (var singleSpot in spots) {
                            singleSpot.Occupier = _context.Users.FirstOrDefault(u => u.Id == singleSpot.OccupiedBy);
                            singleSpot.IsCurrentUserAdmin = IsParkingAdmin(singleSpot);
                        }
                        return Ok(spots);
                    } catch (DbUpdateConcurrencyException) {
                        if (!SpotExists(spot.Id)) {
                            return NotFound();
                        } else {
                            throw;
                        }
                    }
                }
            }

            return NotFound();
        }

        private bool SpotExists(int id)
        {
            return _context.Spots.Any(e => e.Id == id);
        }

        private bool IsParkingAdmin(Spot spot)
        {
            var userID = User.Identity.Name;
            var storedSpot = _context.Spots.AsNoTracking().Where(s => s.Id == spot.Id).FirstOrDefault();
            var parking = _context.Parkings.Where(p => p.Id == storedSpot.ParkingId).FirstOrDefault();
            var userParking = _context.UsersParkings.Where(up => up.ParkingId == parking.Id && up.UserId == int.Parse(userID) && up.IsAdmin == 1).FirstOrDefault();

            return userParking != null;
        }

        private bool IsParkingAdmin(Parking parking)
        {
            var userID = User.Identity.Name;
            var storedParking = _context.Parkings.Find(parking.Id);
            var userParking = _context.UsersParkings.Where(up => up.ParkingId == storedParking.Id && up.UserId == int.Parse(userID) && up.IsAdmin == 1).FirstOrDefault();

            return userParking != null;
        }

        private bool IsParkingAdmin(int parkingID)
        {
            var storedParking = _context.Parkings.Find(parkingID);
            if (storedParking != null) {
                return IsParkingAdmin(storedParking);
            }

            return false;
        }

        private bool IsParkingUser(Spot spot)
        {
            var userID = User.Identity.Name;
            var storedSpot = _context.Spots.Find(spot.Id);
            var userParking = _context.UsersParkings.Where(up => up.ParkingId == storedSpot.ParkingId && up.UserId == int.Parse(userID)).FirstOrDefault();

            return userParking != null;
        }

        private bool IsParkingUser(Parking parking)
        {
            var userID = User.Identity.Name;
            var storedParking = _context.Parkings.Find(parking.Id);
            var userParking = _context.UsersParkings.Where(up => up.ParkingId == storedParking.Id && up.UserId == int.Parse(userID)).FirstOrDefault();

            return userParking != null;
        }

        private bool CanChangeStatus(Spot spot)
        {
            var storedSpot = _context.Spots.AsNoTracking().Where(s => s.Id == spot.Id).FirstOrDefault();
            if (storedSpot != null) {
                if (IsParkingAdmin(storedSpot) || (IsParkingUser(storedSpot) && storedSpot.OccupiedBy == null)) {
                    return true;
                }

                var userID = User.Identity.Name;
                if (storedSpot.OccupiedBy == int.Parse(userID) && IsParkingUser(storedSpot)) {
                    return true;
                }
            }

            return false;
        }
    }
}