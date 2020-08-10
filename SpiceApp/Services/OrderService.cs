using SpiceApp.Data;
using SpiceApp.Models;
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
    }
}
