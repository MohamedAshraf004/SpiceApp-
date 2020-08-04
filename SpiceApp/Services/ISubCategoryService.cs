using SpiceApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpiceApp.Services
{
    public interface ISubCategoryService
    {
        Task<bool> AddSubCategory(SubCategory subCategory);
        Task<bool> DeleteCategory(int id);
        Task<IEnumerable<SubCategory>> GetAllSubCategories();
        Task<SubCategory> GetSubCategoryById(int id);
        Task<bool> UpdateSubCategory(SubCategory updatedSubCategory);
    }
}