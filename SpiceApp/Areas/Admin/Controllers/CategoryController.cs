using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SpiceApp.Models;
using SpiceApp.Services;

namespace SpiceApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }
        public async Task<IActionResult> Index()
        {
            return View(await categoryService.GetAllCategories());
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || id.Value == 0)
                return NotFound();
            var category = await categoryService.GetCategoryById(id.Value);
            if (category !=null)
            {
                return View(category);
            }
            return NotFound();
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
                return View(category);
            if (await categoryService.AddCategory(category))
            {
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        public async Task<IActionResult> Edit(int? id)
        {
            
            if (id == null)
            {
                return NotFound();
            }
            var category = await categoryService.GetCategoryById(id.Value);
            if (category==null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }
            if (await categoryService.UpdateCategory(category))
                return RedirectToAction(nameof(Index));
            return View(category);
        }

        public async Task<IActionResult> Delete(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }
            var category = await categoryService.GetCategoryById(id.Value);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (id ==0)
            {
                return NotFound();
            }
            if (await categoryService.DeleteCategory(id))
                return RedirectToAction(nameof(Index));
            return View();
        }
    }
}
