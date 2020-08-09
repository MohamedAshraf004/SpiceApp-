using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SpiceApp.Models;
using SpiceApp.Services;
using SpiceApp.Utility;
using SpiceApp.ViewModels;

namespace SpiceApp.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICategoryService categoryService;
        private readonly IMenuItemService menuItemService;
        private readonly ICouponService couponService;
        private readonly IShoppingCartService shoppingCartService;

        public HomeController(ILogger<HomeController> logger,ICategoryService categoryService
                   ,IMenuItemService menuItemService,ICouponService couponService
                    ,IShoppingCartService shoppingCartService)
        {
            _logger = logger;
            this.categoryService = categoryService;
            this.menuItemService = menuItemService;
            this.couponService = couponService;
            this.shoppingCartService = shoppingCartService;
        }

        public async Task<IActionResult> Index()
        {
            IndexViewModel model = new IndexViewModel()
            {
                Categories= (await categoryService.GetAllCategories()),
                MenuItems=(await menuItemService.GetAllMenuItems()),
                Coupons=(await couponService.GetAllCoupons()).Where(a=>a.IsActive).ToList()
            };
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim !=null)
            {
                IEnumerable<ShoppingCart> shoppingCarts = await shoppingCartService.GetShoppingCartsByUserId(claim.Value);
                HttpContext.Session.SetInt32(SD.ssShoppingCartCount, shoppingCarts.ToList().Count);

            }
            return View(model);
        }
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var menuItem=await menuItemService.GetMenuItemById(id);
            ShoppingCart model = new ShoppingCart()
            {
                MenuItem=menuItem,
                MenuItemId=menuItem.Id
            };
            return View(model);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Details(ShoppingCart shoppingCart)
        {
            shoppingCart.Id = 0;
            if (ModelState.IsValid)
            {
                var claimsIdentity = ((ClaimsIdentity)this.User.Identity);
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                shoppingCart.ApplicationUserId = claim.Value;
                await shoppingCartService.AddShoppingCartAsync(shoppingCart);

                var count = await shoppingCartService.GetCountAsync(shoppingCart.ApplicationUserId);
                HttpContext.Session.SetInt32(SD.ssShoppingCartCount, count);

                return RedirectToAction("Index");

            }
            else
            {
                var menuItem = await menuItemService.GetMenuItemById(shoppingCart.MenuItemId);
                ShoppingCart model = new ShoppingCart()
                {
                    MenuItem = menuItem,
                    MenuItemId = menuItem.Id
                };
                return View(model);
            }

            
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
