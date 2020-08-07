using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SpiceApp.Models;
using SpiceApp.Services;
using SpiceApp.ViewModels;

namespace SpiceApp.Areas.Admin.Controllers
{
    [Area("Admin")]
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
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || id.Value == 0)
                return NotFound();
            var coupon = await couponService.GetCouponById(id.Value);
            if (coupon != null)
            {
                return View(coupon);
            }
            return NotFound();
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Coupon coupon)
        {
            if (!ModelState.IsValid)
                return View(coupon);
            var files = HttpContext.Request.Form.Files;
            if (files.Count>0)
            {
                byte[] p1 = null;
                using(var fs1 = files[0].OpenReadStream())
                {
                    using (var ms1 = new MemoryStream())
                    {
                        fs1.CopyTo(ms1);
                        p1 = ms1.ToArray();
                    }
                }
                coupon.Picture = p1;
            }
            if (await couponService.AddCoupon(coupon))
            {
                return RedirectToAction(nameof(Index));
            }
            return View(coupon);
        }
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }
            var coupon = await couponService.GetCouponById(id.Value);
            var model = new CouponAndPictureViewModel()
            {
                Coupon = coupon,
                ExistingPhoto = coupon.Picture
            };
            
            if (model.Coupon == null)
            {
                return NotFound();
            }
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CouponAndPictureViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var files = HttpContext.Request.Form.Files;
            if (files.Count > 0)
            {
                byte[] p1 = null;
                using (var fs1 = files[0].OpenReadStream())
                {
                    using (var ms1 = new MemoryStream())
                    {
                        fs1.CopyTo(ms1);
                        p1 = ms1.ToArray();
                    }
                }
                model.Coupon.Picture = p1;
            }
            else
            {
                model.Coupon.Picture = model.ExistingPhoto;
            }
            if (await couponService.UpdateCoupon(model.Coupon))
                return RedirectToAction(nameof(Index));
            return View(model.Coupon);
        }

        public async Task<IActionResult> Delete(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }
            var coupon = await couponService.GetCouponById(id.Value);
            if (coupon == null)
            {
                return NotFound();
            }
            return View(coupon);
        }
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }
            if (await couponService.DeleteCoupon(id))
                return RedirectToAction(nameof(Index));
            return View();
        }
    }
}
