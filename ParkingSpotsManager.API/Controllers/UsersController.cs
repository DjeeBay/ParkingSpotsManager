using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public class UsersController : ControllerBase
    {
        private readonly DataContext _context;

        public UsersController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Users
        //[HttpGet]
        //public IEnumerable<User> GetUsers()
        //{
        //    return _context.Users;
        //}

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }
            CleanUserPassword(ref user);

            return Ok(user);
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser([FromRoute] int id, [FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // POST: api/Users
        [HttpPost]
        public async Task<IActionResult> PostUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        [HttpGet]
        [Route("[action]/{parkingID}/{search}")]
        public async Task<IActionResult> GetInvitableUsers([FromRoute] int parkingID, [FromRoute] string search)
        {
            if (search != null && search.Length >= 3) {
                var parking = _context.Parkings.FirstOrDefault(p => p.Id == parkingID);
                if (parking != null) {
                    var userList = await _context.Users.Where(u => u.Id != int.Parse(User.Identity.Name) && u.Username.Contains(search) ).ToListAsync();
                    CleanUsersPassword(ref userList);
                    //TODO do not return users already associated with the parking (use relationship to query it correctly)

                    return Ok(userList);
                }
            }

            return BadRequest();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        private void CleanUsersPassword(ref List<User> users)
        {
            var nbUsers = users.Count;
            for (var i = 0; i < nbUsers; i++) {
                users[i].Password = null;
            }
        }

        private void CleanUserPassword(ref User user)
        {
            user.Password = null;
        }
    }
}