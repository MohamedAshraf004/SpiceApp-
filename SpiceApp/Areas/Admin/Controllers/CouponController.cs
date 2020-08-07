using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SpiceApp.Services;

namespace SpiceApp.Areas.Admin.Controllers
{
    public class CouponController : Controller
    {
        private readonly ICouponService couponService;

        public CouponController(ICouponService couponService)
        {
            this.couponService = couponService;
        }
        public async Task<IActionResult> Index()
        {
            return View(await couponService.GetAllCoupons());
        }
    }
}
