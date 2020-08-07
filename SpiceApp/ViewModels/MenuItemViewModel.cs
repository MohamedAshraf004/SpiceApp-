using Microsoft.AspNetCore.Http;
using SpiceApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiceApp.ViewModels
{
    public class MenuItemViewModel
    {

        public MenuItem MenuItem { get; set; }
        public IFormFile Img{ get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<SubCategory> SubCategories { get; set; }

    }
}
