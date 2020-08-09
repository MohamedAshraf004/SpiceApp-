using Microsoft.EntityFrameworkCore;
using SpiceApp.Data;
using SpiceApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiceApp.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly ApplicationDbContext _dbContext;

        public ShoppingCartService(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }
        public async Task<ShoppingCart>
            GetShoppingCartByUserIdAndItemIdAsync(string applicationUsertId, int menuItemId)
        {
            return await _dbContext.shoppingCarts
                .FirstOrDefaultAsync(u => u.ApplicationUserId == applicationUsertId && u.MenuItemId == menuItemId);
        }
        public async Task<IEnumerable<ShoppingCart>>
            GetShoppingCartsByUserId(string applicationUsertId)
        {
            return await _dbContext.shoppingCarts
                .Where(u => u.ApplicationUserId == applicationUsertId).ToListAsync();
        }
        public async Task<bool> AddShoppingCartAsync(ShoppingCart shoppingCart)
        {
            var shoppingCartFromDb =
                await GetShoppingCartByUserIdAndItemIdAsync(shoppingCart.ApplicationUserId, shoppingCart.MenuItemId);
            if (shoppingCartFromDb == null)
            {
                await _dbContext.shoppingCarts
                .AddAsync(shoppingCart);
            }
            else
            {
                shoppingCartFromDb.Count += shoppingCart.Count;
            }

            return await _dbContext.SaveChangesAsync() > 0 ? true : false;
        }
        public async Task<int> GetCountAsync(string applicationUsertId)
        {
            return await _dbContext.shoppingCarts
                .Where(u => u.ApplicationUserId == applicationUsertId).CountAsync();
        }

    }
}
