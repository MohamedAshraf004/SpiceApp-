using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using SpiceApp.Models;
using SpiceApp.Services;
using SpiceApp.Utility;
using SpiceApp.ViewModels;
using Stripe;

namespace SpiceApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService orderService;
        private readonly Services.IEmailSender emailSender;
        private readonly IUserService userService;
        private int PageSize = 2;
        public OrderController(IOrderService orderService, Services.IEmailSender emailSender,
                                IUserService userService)
        {
            this.orderService = orderService;
            this.emailSender = emailSender;
            this.userService = userService;
        }
        public async Task<IActionResult> Confirm(int id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            OrderDetailsViewModel orderDetailsViewModel = new OrderDetailsViewModel()
            {
                OrderHeader = await orderService.GetOrderHeaderByUserIdAsync(claim.Value),
                OrderDetails = await orderService.GetOrderDetailsByOrderHeaderIdAsync(id)
            };
            orderDetailsViewModel.OrderHeader.Status = SD.StatusSubmitted;
            await orderService.CommitAsync();
            return View(orderDetailsViewModel);
        }

        public async Task<IActionResult> OrderHistory(int productPage = 1)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            OrderListViewModel orderListVM = new OrderListViewModel()
            {
                Orders = new List<OrderDetailsViewModel>()
            };

            List<OrderHeader> orderHeaderList=await orderService.GetAllOrderHeaderByUserIdAsync(claim.Value);
            foreach (var item in orderHeaderList)
            {
                OrderDetailsViewModel model = new OrderDetailsViewModel()
                {
                    OrderHeader = item,
                    OrderDetails = await orderService.GetOrderDetailsByOrderHeaderIdAsync(item.Id)
                };
                orderListVM.Orders.Add(model);
            }

            var count = orderListVM.Orders.Count;
            orderListVM.Orders = orderListVM.Orders.OrderByDescending(p => p.OrderHeader.Id)
                                 .Skip((productPage - 1) * PageSize)
                                 .Take(PageSize).ToList();

            orderListVM.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = PageSize,
                TotalItem = count,
                urlParam = "/Customer/Order/OrderHistory?productPage=:"
            };

            return View(orderListVM);
        }

        public async Task<IActionResult> GetOrderDetails(int Id)
        {
            OrderDetailsViewModel orderDetailsViewModel = new OrderDetailsViewModel()
            {
                OrderHeader = await orderService.GetOrderHeaderById(Id),
                OrderDetails = await orderService.GetOrderDetailsByOrderHeaderIdAsync(Id)
            };
            //orderDetailsViewModel.OrderHeader.ApplicationUser = await _db.ApplicationUser.FirstOrDefaultAsync(u => u.Id == orderDetailsViewModel.OrderHeader.UserId);

            return PartialView("_IndividualOrderDetails", orderDetailsViewModel);
        }
        [Authorize(Roles =SD.ManagerUser +","+SD.KitchenUser)]
        public async Task<IActionResult> ManageOrder()
        {
            List<OrderDetailsViewModel> orderDetailsViewModel = new List<OrderDetailsViewModel>();
            List<OrderHeader> orderHeaders = await orderService.GetOrderHeadersWhichSubmittedOrInProecess();

            foreach (var item in orderHeaders)
            {
                OrderDetailsViewModel model = new OrderDetailsViewModel()
                {
                    OrderHeader = item,
                    OrderDetails = await orderService.GetOrderDetailsByOrderHeaderIdAsync(item.Id)
                };
                orderDetailsViewModel.Add(model);
            }
            
            return View(orderDetailsViewModel.OrderBy(o=>o.OrderHeader.PickUpTime));
        }

        public async Task<IActionResult> GetOrderStatus(int Id)
        {
            return PartialView("_OrderStatus",(await orderService.GetOrderHeaderById(Id)).Status);

        }

        [Authorize(Roles = SD.KitchenUser + "," + SD.ManagerUser)]
        public async Task<IActionResult> OrderPrepare(int OrderId)
        {
            await orderService.OrderPrepare(OrderId);
            return RedirectToAction("ManageOrder", "Order");
        }

        [Authorize(Roles = SD.KitchenUser + "," + SD.ManagerUser)]
        public async Task<IActionResult> OrderReady(int OrderId)
        {
            await orderService.OrderReady(OrderId);

            //Email logic to notify user that order is ready for pickup
            var order = await orderService.GetOrderHeaderById(OrderId);
            var subject = "Spicy - Order Ready " + OrderId.ToString();
            var msg = "The Order Has been Ready for pichup.";
            var applicationUser = await userService.GetUserById(order.UserId);
            await emailSender.SendEmailAsync(applicationUser.Email, subject, msg);


            return RedirectToAction("ManageOrder", "Order");
        }

        [Authorize(Roles = SD.KitchenUser + "," + SD.ManagerUser)]
        public async Task<IActionResult> OrderCancel(int OrderId)
        {
            await orderService.OrderCancelled(OrderId);
            var order= await orderService.GetOrderHeaderById(OrderId);
            var subject = "Spicy - Order Cancelled " + OrderId.ToString();
            var msg = "The Order Has been Cancelled successfully";
            var applicationUser = await userService.GetUserById(order.UserId);
            await emailSender.SendEmailAsync(applicationUser.Email, subject, msg);

            return RedirectToAction("ManageOrder", "Order");
        }


        public async Task<IActionResult> OrderPickup(int productPage = 1, string searchEmail = null, 
                                                    string searchPhone = null, string searchName = null)
        {

            OrderListViewModel orderListVM = new OrderListViewModel()
            {
                Orders = new List<OrderDetailsViewModel>()
            };

            StringBuilder param = new StringBuilder();
            param.Append("/Customer/Order/OrderPickup?productPage=:");
            param.Append("&searchName=");
            if (searchName != null)
            {
                param.Append(searchName);
            }
            param.Append("&searchEmail=");
            if (searchEmail != null)
            {
                param.Append(searchEmail);
            }
            param.Append("&searchPhone=");
            if (searchPhone != null)
            {
                param.Append(searchPhone);
            }


            List<OrderHeader> orderHeaderList = new List<OrderHeader>();

            if (searchName !=null)
            {
                orderHeaderList = await orderService.GetOrderHeaderByName(searchName);
            }
            else if (searchEmail != null)
            {
                orderHeaderList = await orderService.GetOrderHeaderByPhoneNumber(searchPhone);
            }
            else if(searchPhone != null)
            {
                orderHeaderList = await orderService.GetOrderHeaderByEmail(searchEmail);
            }
            else
            {
                orderHeaderList = await orderService.GetOrderHeaderWhichStatusIsReadyAsync();
            }

            foreach (var item in orderHeaderList)
            {
                OrderDetailsViewModel model = new OrderDetailsViewModel()
                {
                    OrderHeader = item,
                    OrderDetails = await orderService.GetOrderDetailsByOrderHeaderIdAsync(item.Id)
                };
                orderListVM.Orders.Add(model);
            }

            var count = orderListVM.Orders.Count;
            orderListVM.Orders = orderListVM.Orders.OrderByDescending(p => p.OrderHeader.Id)
                                 .Skip((productPage - 1) * PageSize)
                                 .Take(PageSize).ToList();

            orderListVM.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = PageSize,
                TotalItem = count,
                urlParam = param.ToString()
            };

            return View(orderListVM);
        }

        [Authorize(Roles = SD.FrontDeskUser + "," + SD.ManagerUser)]
        [HttpPost]
        [ActionName("OrderPickup")]
        public async Task<IActionResult> OrderPickupPost(int orderId)
        {
            OrderHeader orderHeader = await orderService.GetOrderHeaderById(orderId);
            orderHeader.Status = SD.StatusCompleted;
            await orderService.CommitAsync();

            var order = await orderService.GetOrderHeaderById(orderId);
            var subject = "Spicy - Order Completed " + orderId.ToString();
            var msg = "The Order Has been Completed successfully";
            var applicationUser = await userService.GetUserById(order.UserId);
            await emailSender.SendEmailAsync(applicationUser.Email, subject, msg);

            return RedirectToAction("OrderPickup", "Order");
        }

        public IActionResult Index()
        {
            return View();
        }

    }
}
