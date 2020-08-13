using Microsoft.EntityFrameworkCore;
using SpiceApp.Data;
using SpiceApp.Models;
using SpiceApp.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiceApp.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _dbContext;

        public OrderService(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task AddOrderHeaderAsync(OrderHeader orderHeader)
        {
            await _dbContext.OrderHeaders.AddAsync(orderHeader);
            await _dbContext.SaveChangesAsync();
        }
        public async Task AddOrderDetailsAsync(OrderDetails orderDetails)
        {
            await _dbContext.OrderDetails.AddAsync(orderDetails);
            await _dbContext.SaveChangesAsync();

        }
        public async Task AddOrdersDetailsAsync(List<OrderDetails> listOrdersDetails)
        {
            await _dbContext.OrderDetails.AddRangeAsync(listOrdersDetails);
            await _dbContext.SaveChangesAsync();

        }
        public async Task CommitAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task<OrderHeader> GetOrderHeaderByUserIdAsync(string ApplicationUserId)
        {
            return await _dbContext.OrderHeaders.Include(u => u.ApplicationUser)
                .FirstOrDefaultAsync(u => u.UserId == ApplicationUserId);
        }

        public async Task<List<OrderHeader>> GetAllOrderHeaderByUserIdAsync(string ApplicationUserId)
        {
            return await _dbContext.OrderHeaders.Include(u => u.ApplicationUser)
                .Where(u => u.UserId == ApplicationUserId).ToListAsync();
        }

        public async Task<List<OrderHeader>> GetOrderHeaderWhichStatusIsReadyAsync()
        {
            return await _dbContext.OrderHeaders.Include(u => u.ApplicationUser)
                .Where(u => u.Status==SD.StatusReady).ToListAsync();
        }

        public async Task<List<OrderDetails>> GetOrderDetailsByOrderHeaderIdAsync(int orderHeaderId)
        {
            return await _dbContext.OrderDetails.Where(u => u.OrderId == orderHeaderId).ToListAsync();
        }

        public async Task<OrderHeader> GetOrderHeaderById(int orderHeaderId)
        {
            return await _dbContext.OrderHeaders.Include(u=>u.ApplicationUser)
                .FirstOrDefaultAsync(o => o.Id == orderHeaderId);
        }
        public async Task OrderPrepare(int orderId)
        {
             (await _dbContext.OrderHeaders
                .FirstOrDefaultAsync(o => o.Id == orderId)).Status=SD.StatusInProcess;
            await _dbContext.SaveChangesAsync();
        }
         public async Task OrderReady(int orderId)
        {
             (await _dbContext.OrderHeaders
                .FirstOrDefaultAsync(o => o.Id == orderId)).Status=SD.StatusReady;
            await _dbContext.SaveChangesAsync();
        }
         public async Task OrderCancelled(int orderId)
        {
             (await _dbContext.OrderHeaders
                .FirstOrDefaultAsync(o => o.Id == orderId)).Status=SD.StatusCancelled;
            await _dbContext.SaveChangesAsync();
        }
        

        public async Task<List<OrderHeader>> GetOrderHeadersWhichSubmittedOrInProecess()
        {
            return await _dbContext.OrderHeaders
                .Where(o => o.Status == SD.StatusInProcess || o.Status == SD.StatusSubmitted)
                .OrderByDescending(o=>o.PickUpTime).ToListAsync();
        }

        public async Task<List<OrderHeader>> GetOrderHeaderByName(string name)
        {
            return await _dbContext.OrderHeaders.Include(u => u.ApplicationUser)
                .Where(o => o.PickupName.Trim().ToLower().Contains(name.Trim().ToLower()))
                .OrderByDescending(o => o.OrderDate).ToListAsync();
        }

        public async Task<List<OrderHeader>> GetOrderHeaderByPhoneNumber(string phoneNumber)
        {
            return await _dbContext.OrderHeaders.Include(u=>u.ApplicationUser)
                .Where(o => o.PhoneNumber.Trim().Contains(phoneNumber.Trim()))
                .OrderByDescending(o => o.OrderDate).ToListAsync();

        }

        public async Task<List<OrderHeader>> GetOrderHeaderByEmail(string email)
        {
            var user = await _dbContext.ApplicationUsers.Where(u => u.Email.Trim().ToLower().Contains(email.Trim().ToLower())).FirstOrDefaultAsync();
            return await _dbContext.OrderHeaders.Include(u => u.ApplicationUser)
                            .Where(o => o.UserId == user.Id).OrderByDescending(o => o.OrderDate).ToListAsync();
        }
    }
}
