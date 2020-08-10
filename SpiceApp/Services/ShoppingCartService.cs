using Microsoft.AspNetCore.Http;
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
    public class ShoppingCartService : IShoppingCartService


    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor httpContextAccessor;

        public ShoppingCartService(ApplicationDbContext dbContext,IHttpContextAccessor httpContextAccessor)
        {
            this._dbContext = dbContext;
            this.httpContextAccessor = httpContextAccessor;
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
        public async Task<ShoppingCart> GetShoppingCartById(int cartId)
        {
            return await _dbContext.shoppingCarts.FirstOrDefaultAsync(u => u.Id == cartId);
        }
        public async Task<IEnumerable<ShoppingCart>>
            GetShoppingCartsIncludedMenuItemByUserId(string applicationUsertId)
        {
            
            var r = await _dbContext.shoppingCarts
                .Where(u => u.ApplicationUserId == applicationUsertId).Include(m => m.MenuItem).ToListAsync();
            return r;
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
        public async Task<bool> PlusShoppingCart(int cartId)
        {
             (await _dbContext.shoppingCarts.FirstOrDefaultAsync(u => u.Id == cartId)).Count++;
            return await _dbContext.SaveChangesAsync() > 0 ? true : false;
        }
        public async Task<bool> MinusShoppingCart(int cartId)
        {
             var cartDb=await _dbContext.shoppingCarts.FirstOrDefaultAsync(u => u.Id == cartId);
            if (cartDb.Count==1)
            {
                _dbContext.shoppingCarts.Remove(cartDb);
                await _dbContext.SaveChangesAsync();
                var count = (await _dbContext.shoppingCarts.Where(s => s.ApplicationUserId == cartDb.ApplicationUserId).ToListAsync()).Count;
                httpContextAccessor.HttpContext.Session.SetInt32(SD.ssShoppingCartCount, count);
            }
            else
            {
                cartDb.Count -= 1;
            }
            return await _dbContext.SaveChangesAsync() > 0 ? true : false;
        }
        public async Task<bool> RemoveShoppingCart(int cartId)
        {
             var cartDb=await _dbContext.shoppingCarts.FirstOrDefaultAsync(u => u.Id == cartId);
             _dbContext.shoppingCarts.Remove(cartDb);
             await _dbContext.SaveChangesAsync();
             var count = (await _dbContext.shoppingCarts.Where(s => s.ApplicationUserId == cartDb.ApplicationUserId).ToListAsync()).Count;
             httpContextAccessor.HttpContext.Session.SetInt32(SD.ssShoppingCartCount, count);
            return await _dbContext.SaveChangesAsync() > 0 ? true : false;
        }
        public async Task RemoveShoppingCarts(List<ShoppingCart> carts)
        {
             _dbContext.shoppingCarts.RemoveRange(carts);
             await _dbContext.SaveChangesAsync();
             
        }

    }
}
