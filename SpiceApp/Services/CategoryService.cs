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
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _dbContext;

        public CategoryService(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<bool> AddCategory(Category category)
        {
            await _dbContext.Categories.AddAsync(category);
            return await _dbContext.SaveChangesAsync() > 0 ? true : false;
        }

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            return await _dbContext.Categories.ToListAsync();
        }

        public async Task<Category> GetCategoryById(int id)
        {
            return await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<bool> DeleteCategory(int id)
        {
            var category = await _dbContext.Categories.FindAsync(id);
             _dbContext.Categories.Remove(category);
            return await _dbContext.SaveChangesAsync() > 0 ? true : false;
        }

        public async Task<bool> UpdateCategory(Category updatedCategory)
        {
            var category = _dbContext.Categories.Attach(updatedCategory);
            category.State = EntityState.Modified;
            return await _dbContext.SaveChangesAsync() > 0 ? true : false;
        }
    }
}
