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
        Task<OrderHeader> GetOrderHeaderByUserIdAsync(string ApplicationUserId);
        Task<List<OrderHeader>> GetAllOrderHeaderByUserIdAsync(string ApplicationUserId);
        Task<List<OrderHeader>> GetOrderHeadersWhichSubmittedOrInProecess();
        Task<List<OrderDetails>> GetOrderDetailsByOrderHeaderIdAsync(int orderHeaderId);
        Task<OrderHeader> GetOrderHeaderById(int orderHeaderId);
        Task<List<OrderHeader>> GetOrderHeaderWhichStatusIsReadyAsync();
        Task<List<OrderHeader>> GetOrderHeaderByName(string name);
        Task<List<OrderHeader>> GetOrderHeaderByPhoneNumber(string phoneNumber);
        Task<List<OrderHeader>> GetOrderHeaderByEmail(string email);
        Task OrderPrepare(int orderId);
        Task OrderReady(int orderId);
        Task OrderCancelled(int orderId);
        Task CommitAsync();
    }
}