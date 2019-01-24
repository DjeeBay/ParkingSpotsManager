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

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string test)
        {
            try {
                var user = await userModel.GetByLogin("thebestjb");
                if (user != null) {
                    var passMatched = PasswordService.VerifyHashedPassword(user.Password, test);
                    if (passMatched) {
                        return new OkObjectResult(user);
                    }
                }

                return new OkObjectResult(user);
            } catch (Exception e) { return new OkObjectResult(e.Message); }
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync(Authenticator authenticator)
        {
            try {
                var user = await userModel.GetByLogin(authenticator.Login);
                if (user != null) {
                    var passMatched = PasswordService.VerifyHashedPassword(user.Password, authenticator.Password);
                    if (passMatched) {
                        return new OkObjectResult(user);
                    }
                }
            } catch (Exception e) {
                //TODO badrequest
                return new OkObjectResult(e.Message);
            }
            //TODO badrequest
            return new OkObjectResult("no user");
        }
    }
}