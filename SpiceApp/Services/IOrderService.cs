using SpiceApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpiceApp.Services
{
    public interface IOrderService
    {
        Task AddOrderDetailsAsync(OrderDetails orderDetails);
        Task AddOrderHeaderAsync(OrderHeader orderHeader);
        Task AddOrdersDetailsAsync(List<OrderDetails> listOrdersDetails);
        Task CommitAsync();
    }
}