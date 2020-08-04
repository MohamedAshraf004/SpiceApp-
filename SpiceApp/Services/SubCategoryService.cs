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
            return await _dbContext.SubCategories.ToListAsync();
        }

        public async Task<SubCategory> GetSubCategoryById(int id)
        {
            return await _dbContext.SubCategories.FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<bool> DeleteCategory(int id)
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
