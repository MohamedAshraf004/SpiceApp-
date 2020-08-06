using SpiceApp.Models;
using SpiceApp.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpiceApp.Services
{
    public interface ISubCategoryService
    {
        Task<bool> AddSubCategory(SubCategory subCategory);
        Task<bool> DeleteSubCategory(int id);
        Task<IEnumerable<SubCategory>> GetAllSubCategories();
        Task<List<SubCategory>> GetSubCategoryInCategory(int categoryId);
        Task<SubCategory> GetSubCategoryById(int id);
        Task<SubCategory> DoesSubCategoryExists(SubCategoryAndCategoryViewModel model);
        Task<bool> UpdateSubCategory(SubCategory updatedSubCategory);
    }
}