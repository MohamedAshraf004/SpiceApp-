using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SpiceApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SubCategoryController : Controller
    {
        public SubCategoryController()
        {

        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
