using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SpiceApp.Models;
using SpiceApp.Services;
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

        public HomeController(ILogger<HomeController> logger,ICategoryService categoryService
                   ,IMenuItemService menuItemService,ICouponService couponService)
        {
            _logger = logger;
            this.categoryService = categoryService;
            this.menuItemService = menuItemService;
            this.couponService = couponService;
        }

        public async Task<IActionResult> Index()
        {
            IndexViewModel model = new IndexViewModel()
            {
                Categories= (await categoryService.GetAllCategories()),
                MenuItems=(await menuItemService.GetAllMenuItems()),
                Coupons=(await couponService.GetAllCoupons()).Where(a=>a.IsActive).ToList()
            };
            return View(model);
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
