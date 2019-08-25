using Microsoft.AspNetCore.Mvc.Filters;
using ParkingSpotsManager.Shared.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ParkingSpotsManager.API.Filters
{
    public class AuthoringFilter : IAsyncActionFilter
    {
        private DataContext _dbContext;

        public AuthoringFilter(DataContext dataContext)
        {
            _dbContext = dataContext;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var claimsIdentity = (ClaimsIdentity)context.HttpContext.User.Identity;
            if (claimsIdentity != null && claimsIdentity.Name != null) {
                _dbContext.UserId = int.Parse(claimsIdentity.Name);
            }

            _ = await next();
        }
    }
}
