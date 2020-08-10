using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpiceApp.Models;
using SpiceApp.Services;
using SpiceApp.Utility;
using SpiceApp.ViewModels;
using Stripe;

namespace SpiceApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly IShoppingCartService shoppingCartService;
        private readonly IMenuItemService menuItemService;
        private readonly ICouponService couponService;
        private readonly IUserService userService;
        private readonly IOrderService orderService;

        public CartController(IShoppingCartService shoppingCartService
                                ,IMenuItemService menuItemService
                                ,ICouponService couponService
                                ,IUserService userService
                                ,IOrderService orderService)
        {
            this.shoppingCartService = shoppingCartService;
            this.menuItemService = menuItemService;
            this.couponService = couponService;
            this.userService = userService;
            this.orderService = orderService;
        }
        [BindProperty]
        public OrderDetailsCart DetailsCart{ get; set; }

        public async Task<IActionResult> Index()
        {
            DetailsCart = new OrderDetailsCart()
            {
                OrderHeader = new OrderHeader()
                {
                    OrderTotal = 0
                }
            };

            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var cart = await shoppingCartService.GetShoppingCartsByUserId(claim.Value);
            if (cart != null)
            {
                DetailsCart.listCart = cart.ToList();
            }
            //var total = DetailsCart.listCart.Select(s => new { total = s.MenuItem.Price * s.Count });
            foreach (var list in DetailsCart.listCart)
            {
                list.MenuItem = await menuItemService.GetMenuItemById(list.MenuItemId);
                DetailsCart.OrderHeader.OrderTotal = DetailsCart.OrderHeader.OrderTotal + (list.MenuItem.Price * list.Count);
                list.MenuItem.Description = SD.ConvertToRawHtml(list.MenuItem.Description);
                if (list.MenuItem.Description.Length > 100)
                {
                    list.MenuItem.Description = list.MenuItem.Description.Substring(0, 99) + "...";
                }
            }
            DetailsCart.OrderHeader.OrderTotalOriginal = DetailsCart.OrderHeader.OrderTotal;

            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                DetailsCart.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponFromDb = await couponService.GetCouponByName(DetailsCart.OrderHeader.CouponCode);
                DetailsCart.OrderHeader.OrderTotal = SD.DiscountedPrice(couponFromDb, DetailsCart.OrderHeader.OrderTotalOriginal);
            }

            return View(DetailsCart);
        }

        public async Task<IActionResult> Summary()
        {
            DetailsCart = new OrderDetailsCart()
            {
                OrderHeader = new OrderHeader()
                {
                    OrderTotal = 0
                }
            };

            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var applicationUser = await userService.GetUserById(claim.Value);
            var cart = await shoppingCartService.GetShoppingCartsByUserId(claim.Value);
            if (cart != null)
            {
                DetailsCart.listCart = cart.ToList();
            }
            //var total = DetailsCart.listCart.Select(s => new { total = s.MenuItem.Price * s.Count });
            foreach (var list in DetailsCart.listCart)
            {
                list.MenuItem = await menuItemService.GetMenuItemById(list.MenuItemId);
                DetailsCart.OrderHeader.OrderTotal = DetailsCart.OrderHeader.OrderTotal + (list.MenuItem.Price * list.Count);
                
            }

            DetailsCart.OrderHeader.OrderTotalOriginal = DetailsCart.OrderHeader.OrderTotal;
            DetailsCart.OrderHeader.PickUpDate =DateTime.Now;
            DetailsCart.OrderHeader.PhoneNumber =  applicationUser.PhoneNumber;
            DetailsCart.OrderHeader.PickupName = applicationUser.Name;

            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                DetailsCart.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponFromDb = await couponService.GetCouponByName(DetailsCart.OrderHeader.CouponCode);
                DetailsCart.OrderHeader.OrderTotal = SD.DiscountedPrice(couponFromDb, DetailsCart.OrderHeader.OrderTotalOriginal);
            }

            return View(DetailsCart);
        }

        

        [ValidateAntiForgeryToken]
        [HttpPost,ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(string stripeToken)
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var applicationUser = await userService.GetUserById(claim.Value);
            DetailsCart.listCart = ( await shoppingCartService.GetShoppingCartsByUserId(claim.Value) ).ToList();

            DetailsCart.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            DetailsCart.OrderHeader.Status = SD.PaymentStatusPending;
            DetailsCart.OrderHeader.UserId = claim.Value;
            DetailsCart.OrderHeader.PickUpTime= 
                        Convert.ToDateTime(DetailsCart.OrderHeader.PickUpDate.ToShortDateString() 
                        + " " + DetailsCart.OrderHeader.PickUpTime.ToShortTimeString());
            DetailsCart.OrderHeader.OrderDate=DateTime.Now;

            await orderService.AddOrderHeaderAsync(DetailsCart.OrderHeader);
            DetailsCart.OrderHeader.OrderTotalOriginal = 0;

            List<OrderDetails> orderDetailsList = new List<OrderDetails>();

            foreach (var item in DetailsCart.listCart)
            {
                item.MenuItem = await menuItemService.GetMenuItemById(item.MenuItemId);
                OrderDetails orderDetails = new OrderDetails
                {
                    MenuItemId = item.MenuItemId,
                    OrderId = DetailsCart.OrderHeader.Id,
                    Description = item.MenuItem.Description,
                    Name = item.MenuItem.Name,
                    Price = item.MenuItem.Price,
                    Count = item.Count
                };
                DetailsCart.OrderHeader.OrderTotalOriginal += orderDetails.Count * orderDetails.Price;
                orderDetailsList.Add(orderDetails);
            }
            await orderService.AddOrdersDetailsAsync(orderDetailsList);

            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                DetailsCart.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponFromDb = await couponService.GetCouponByName(DetailsCart.OrderHeader.CouponCode.ToLower());
                DetailsCart.OrderHeader.OrderTotal = SD.DiscountedPrice(couponFromDb, DetailsCart.OrderHeader.OrderTotalOriginal);
            }
            else
            {
                DetailsCart.OrderHeader.OrderTotal = DetailsCart.OrderHeader.OrderTotalOriginal;
            }
            DetailsCart.OrderHeader.CouponCodeDiscount = DetailsCart.OrderHeader.OrderTotalOriginal - DetailsCart.OrderHeader.OrderTotal;
            await orderService.CommitAsync();

            await shoppingCartService.RemoveShoppingCarts(DetailsCart.listCart);
            HttpContext.Session.SetInt32(SD.ssShoppingCartCount, 0);
            var options = new ChargeCreateOptions()
            {
                Amount = Convert.ToInt32(DetailsCart.OrderHeader.OrderTotal * 100),
                Currency = "usd",
                Description = "Order Id : " + DetailsCart.OrderHeader.Id,
                Source = stripeToken
            };
            var service = new ChargeService();
            Charge charge = await service.CreateAsync(options);
            if (charge.BalanceTransactionId==null)
            {
                DetailsCart.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;
            }
            else
            {
                DetailsCart.OrderHeader.TransactionId = charge.BalanceTransactionId;
            }
            if(charge.Status.ToLower()=="succeeded")
            {
                DetailsCart.OrderHeader.PaymentStatus = SD.PaymentStatusApproved;
                DetailsCart.OrderHeader.Status= SD.StatusSubmitted;
            }
            else
            {
                DetailsCart.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;
            }
            await orderService.CommitAsync();

            return RedirectToAction("Index", "Home");
        }

        public IActionResult AddCoupon()
        {
            if (DetailsCart.OrderHeader.CouponCode == null)
            {
                DetailsCart.OrderHeader.CouponCode = "";
            }
            HttpContext.Session.SetString(SD.ssCouponCode, DetailsCart.OrderHeader.CouponCode);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveCoupon()
        {

            HttpContext.Session.SetString(SD.ssCouponCode, string.Empty);

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Plus(int? cartId)
        {
            if (cartId ==null)
            {
                return NotFound();
            }
            await shoppingCartService.PlusShoppingCart(cartId.Value);
            return RedirectToAction(nameof(Index));
            
        }

        public async Task<IActionResult> Minus(int? cartId)
        {
            if (cartId == null)
            {
                return NotFound();
            }
            await shoppingCartService.MinusShoppingCart(cartId.Value);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Remove(int? cartId)
        {
            if (cartId ==null)
            {
                return NotFound();
            }
            await shoppingCartService.RemoveShoppingCart(cartId.Value);
            return RedirectToAction(nameof(Index));
        }
    }
}
