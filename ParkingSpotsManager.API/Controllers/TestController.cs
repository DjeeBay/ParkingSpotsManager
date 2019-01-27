using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ParkingSpotsManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        // POST api/test
        [HttpPost]
        public ActionResult<User> Post([FromBody] Authenticator authenticator)
        {
            if (authenticator != null && authenticator.Login != null) {
                return new OkObjectResult(new User() { Name = "toto", Token = "token" });
            }

            return new BadRequestResult();
        }
    }

    public class Authenticator
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }

    public class User
    {
        public string Name { get; set; }
        public string Token { get; set; }
    }
}
