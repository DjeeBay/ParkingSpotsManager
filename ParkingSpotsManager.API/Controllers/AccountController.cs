using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkingSpotsManager.Shared.Libraries;
using ParkingSpotsManager.Shared.Models;
using ParkingSpotsManager.Shared.Services;

namespace ParkingSpotsManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private Shared.Models.User userModel;

        public AccountController()
        {
            userModel = new Shared.Models.User();
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync([FromBody] Authenticator authenticator)
        {
            try {
                var user = await userModel.GetByLoginAsync(authenticator.Login);
                if (user != null) {
                    var passMatched = PasswordService.VerifyHashedPassword(user.Password, authenticator.Password);
                    if (passMatched) {
                        user.Password = null;
                        return new OkObjectResult(user);
                    }
                }
            } catch (Exception e) {
                return new OkObjectResult(e.Message);
            }

            return new BadRequestResult();
        }

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

                return new OkObjectResult(createdUser);
            } catch (Exception e) { return new OkObjectResult(e.Message); }
        }
    }
}