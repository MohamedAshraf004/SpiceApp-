using SpiceApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpiceApp.Services
{
    public interface IMenuItemService
    {
        Task<bool> AddMenuItem(MenuItem menuItem);
        Task Commit();
        Task<bool> DeleteMenuItem(int id);
        Task<IEnumerable<MenuItem>> GetAllMenuItems();
        Task<MenuItem> GetMenuItemById(int id);
        Task<bool> UpdateMenuItem(MenuItem updatedMenuItem);
    }
}