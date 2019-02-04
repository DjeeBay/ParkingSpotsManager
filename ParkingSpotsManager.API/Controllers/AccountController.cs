using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        private User userModel;

        public AccountController()
        {
            userModel = new User();
        }
        
        [HttpGet]
        public IActionResult Get() {
            return new OkObjectResult(User.Identity.Name);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Test() {
            try
            {
                var user = await userModel.GetByLoginAsync("thebestjb");
                if (user != null && user.Id != 0)
                {
                    var passMatched = PasswordService.VerifyHashedPassword(user.Password, "admin");
                    if (passMatched)
                    {
                        user.Password = null;
                        user.AuthToken = TokenService.Get(user);

                        return user != null && user.Id != 0 ? new OkObjectResult(user) : new OkObjectResult("{\"success\":\"false\",\"reason\":\"Auth failed, try later.\"}");
                    }
                }

                return new OkObjectResult("{\"success\":\"false\",\"reason\":\"Auth failed, bad credentials.\"}");
            }
            catch (Exception e)
            {
                return new OkObjectResult(e.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult ValidToken() {
            return new OkObjectResult(HttpContext.User.Identity.IsAuthenticated);
        }

        [AllowAnonymous]
        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] Authenticator authenticator)
        {
            try {
                var user = await userModel.GetByLoginAsync(authenticator.Login);
                if (user != null && user.Id != 0) {
                    var passMatched = PasswordService.VerifyHashedPassword(user.Password, authenticator.Password);
                    if (passMatched) {
                        user.Password = null;
                        user.AuthToken = TokenService.Get(user);

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
                var userNameExists = await userModel.GetByLoginAsync(user.Username);
                if (userNameExists != null) {
                    return new OkObjectResult("{\"success\":\"false\",\"reason\":\"Username already exists.\"}");
                }
                //TODO verify email validity
                var userEmailExists = await userModel.GetByEmailAsync(user.Email);
                if (userEmailExists != null) {
                    return new OkObjectResult("{\"success\":\"false\",\"reason\":\"Email already used.\"}");
                }
                if (user.Password == null || user.Password.Length < 5) {
                    return new OkObjectResult("{\"success\":\"false\",\"reason\":\"Password is too small, 5 caracters min.\"}");
                }

                user.Password = PasswordService.HashPassword(user.Password);
                var createdUser = await userModel.CreateAsync(user);
                createdUser.Password = null;
                createdUser.AuthToken = TokenService.Get(createdUser);

                return new OkObjectResult(createdUser);
            } catch (Exception e) { return new OkObjectResult(e.Message); }
        }
    }
}