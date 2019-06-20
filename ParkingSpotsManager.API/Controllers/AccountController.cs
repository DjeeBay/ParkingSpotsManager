using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkingSpotsManager.API.Helpers;
using ParkingSpotsManager.Shared.Database;
using ParkingSpotsManager.Shared.Libraries;
using ParkingSpotsManager.Shared.Models;
using ParkingSpotsManager.Shared.Services;

namespace ParkingSpotsManager.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly int _passwordMinLength = 5;

        public AccountController(DataContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public IActionResult Get() {
            return new OkObjectResult(User.Identity.Name);
        }

        [AllowAnonymous]
        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] Authenticator authenticator)
        {
            try {
                var user = await _context.Users.Where(u => u.Username.ToLowerInvariant() == authenticator.Login.ToLowerInvariant()).FirstOrDefaultAsync();
                if (user != null && user.Id != 0) {
                    var passMatched = PasswordService.VerifyHashedPassword(user.Password, authenticator.Password);
                    if (passMatched) {
                        user.Password = null;
                        user.AuthToken = TokenService.Get(user, Secrets.TokenSecretKey);

                        return user != null && user.Id != 0 ? new OkObjectResult(user) : new OkObjectResult("{\"success\":\"false\",\"reason\":\"Auth failed, try later.\"}");
                    }
                }

                return new OkObjectResult("{\"success\":\"false\",\"reason\":\"Auth failed, bad credentials.\"}");
            } catch (Exception) {
                return new BadRequestResult();
            }
        }

        [AllowAnonymous]
        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            try {
                if (_context.Users.Where(u => u.Username.ToLowerInvariant() == user.Username.ToLowerInvariant()).FirstOrDefault() != null) {
                    return BadRequest(new { Username = new string[] { "Username already exists." } });
                } else if (_context.Users.Where(u => u.Email == user.Email).FirstOrDefault() != null) {
                    return BadRequest(new { Email = new string[] { "Email already used." } });
                }

                user.Password = PasswordService.HashPassword(user.Password);
                var entityEntry = await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                var createdUser = entityEntry.Entity;
                createdUser.Password = null;
                createdUser.AuthToken = TokenService.Get(createdUser, Secrets.TokenSecretKey);

                return new OkObjectResult(createdUser);
            } catch (Exception e) {
                return NotFound(e.InnerException.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> UpdateUser([FromBody] UserAccount user)
        {
            if (int.Parse(User.Identity.Name) == user.Id) {
                if (_context.Users.Where(u => u.Email == user.Email && u.Id != user.Id).FirstOrDefault() != null) {
                    return BadRequest(new { Email = new string[] { "Email already used." } });
                }
                if (user.IsPasswordSet && (string.IsNullOrEmpty(user.Password) || user.Password.Length < _passwordMinLength)) {
                    return BadRequest(new { Password = new string[] { "Password length must be 5 min." } });
                }
                var storedUser = _context.Users.Where(u => u.Id == user.Id).FirstOrDefault();
                storedUser.Firstname = user.Firstname;
                storedUser.Lastname = user.Lastname;
                storedUser.Email = user.Email;
                if (!string.IsNullOrEmpty(user.Password)) {
                    storedUser.Password = PasswordService.HashPassword(user.Password);
                } else {
                    storedUser.Password = storedUser.Password;
                }

                await _context.SaveChangesAsync();

                return Ok();
            }

            return NotFound();
        }
    }
}