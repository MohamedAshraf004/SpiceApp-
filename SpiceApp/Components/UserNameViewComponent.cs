using Microsoft.AspNetCore.Mvc;
using SpiceApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SpiceApp.Components
{
    public class UserNameViewComponent : ViewComponent
    {
        private readonly IUserService userService;

        public UserNameViewComponent(IUserService userService)
        {
            this.userService = userService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var user = await userService.GetUserById(claims.Value);
            return View("Default",user.Name);
        }

    }
}
