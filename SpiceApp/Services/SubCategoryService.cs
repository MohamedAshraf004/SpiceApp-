using Microsoft.EntityFrameworkCore;
using SpiceApp.Data;
using SpiceApp.Models;
using SpiceApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiceApp.Services
{
    public class SubCategoryService : ISubCategoryService
    {
        private readonly ApplicationDbContext _dbContext;

        public SubCategoryService(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }
        public async Task<bool> AddSubCategory(SubCategory subCategory)
        {
            await _dbContext.SubCategories.AddAsync(subCategory);
            return await _dbContext.SaveChangesAsync() > 0 ? true : false;
        }

        public async Task<IEnumerable<SubCategory>> GetAllSubCategories()
        {
            return await _dbContext.SubCategories.Include(c=>c.Category).ToListAsync();
        }

        public async Task<SubCategory> DoesSubCategoryExists(SubCategoryAndCategoryViewModel model)
        {
            return await _dbContext.SubCategories.Include(c => c.Category)
                .Where(s=>s.Name==model.SubCategory.Name && s.Category.Id==model.SubCategory.CategoryId)
                .FirstOrDefaultAsync();
        }
        public async Task<List<SubCategory>> GetSubCategoryInCategory(int categoryId)
        {
            return  await _dbContext.SubCategories.Where(c => c.CategoryId == categoryId).ToListAsync();
        }
        public async Task<SubCategory> GetSubCategoryById(int id)
        {
            return  await _dbContext.SubCategories.Include(c => c.Category).FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<bool> DeleteSubCategory(int id)
        {
            var subCategory = await _dbContext.SubCategories.FindAsync(id);
            _dbContext.SubCategories.Remove(subCategory);
            return await _dbContext.SaveChangesAsync() > 0 ? true : false;
        }

        public async Task<bool> UpdateSubCategory(SubCategory updatedSubCategory)
        {
            var subCategory = _dbContext.SubCategories.Attach(updatedSubCategory);
            subCategory.State = EntityState.Modified;
            return await _dbContext.SaveChangesAsync() > 0 ? true : false;
        }
    }
}
