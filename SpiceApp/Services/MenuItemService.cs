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
    public class MenuItemService : IMenuItemService
    {
        private readonly ApplicationDbContext _dbContext;

        public MenuItemService(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<bool> AddMenuItem(MenuItem menuItem)
        {
            await _dbContext.MenuItems.AddAsync(menuItem);
            return await _dbContext.SaveChangesAsync() > 0 ? true : false;
        }
        public async Task Commit()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<MenuItem>> GetAllMenuItems()
        {
            return await _dbContext.MenuItems.Include(c => c.Category).Include(s => s.SubCategory).ToListAsync();
        }

        //public async Task<SubCategory> DoesSubCategoryExists(SubCategoryAndCategoryViewModel model)
        //{
        //    return await _dbContext.MenuItems.Include(c => c.Category)
        //        .Where(s => s.Name == model.SubCategory.Name && s.Category.Id == model.SubCategory.CategoryId)
        //        .FirstOrDefaultAsync();
        //}
        //public async Task<List<MenuItem>> GetSubCategoryInCategory(int categoryId)
        //{
        //    return await _dbContext.MenuItems.Where(c => c.CategoryId == categoryId).ToListAsync();
        //}
        public async Task<MenuItem> GetMenuItemById(int id)
        {
            return await _dbContext.MenuItems.Include(c => c.Category).Include(s => s.SubCategory).FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<bool> DeleteMenuItem(int id)
        {
            var subCategory = await _dbContext.MenuItems.FindAsync(id);
            _dbContext.MenuItems.Remove(subCategory);
            return await _dbContext.SaveChangesAsync() > 0 ? true : false;
        }

        public async Task<bool> UpdateMenuItem(MenuItem updatedMenuItem)
        {
            var menuItem = _dbContext.MenuItems.Attach(updatedMenuItem);
            menuItem.State = EntityState.Modified;
            return await _dbContext.SaveChangesAsync() > 0 ? true : false;
        }
    }
}
