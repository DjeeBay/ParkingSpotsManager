using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            } catch (Exception) {
                return new BadRequestResult();
            }

            return new BadRequestResult();
        }
    }
}