using SpiceApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpiceApp.Services
{
    public interface IShoppingCartService
    {
        Task<bool> AddShoppingCartAsync(ShoppingCart shoppingCart);
        Task<IEnumerable<ShoppingCart>> GetShoppingCartsByUserId(string applicationUsertId);
        Task<int> GetCountAsync(string applicationUsertId);
        Task<ShoppingCart> GetShoppingCartByUserIdAndItemIdAsync(string applicationUsertId, int menuItemId);
    }
}