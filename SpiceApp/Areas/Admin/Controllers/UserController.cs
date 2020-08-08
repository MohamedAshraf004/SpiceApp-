using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpiceApp.Services;
using SpiceApp.Utility;

namespace SpiceApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.ManagerUser)]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            this._userService = userService;
        }
        public async Task<IActionResult> Index()
        {
            var claimsIdentity = ((ClaimsIdentity)this.User.Identity);
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            return View(await _userService.GetUsersExceptCurrentUser(claim));
        }
        public async Task<IActionResult> Lock(string id)
        {
            if (id==null)
            {
                return NotFound();
            }
            if (await _userService.UserLock(id))
            {
                return RedirectToAction(nameof(Index));
            }

            return NotFound();
        }
        public async Task<IActionResult> UnLock(string id)
        {
            if (id==null)
            {
                return NotFound();
            }
            if (await _userService.UserUnLock(id))
            {
                return RedirectToAction(nameof(Index));
            }

            return NotFound();
        }

    }
}
