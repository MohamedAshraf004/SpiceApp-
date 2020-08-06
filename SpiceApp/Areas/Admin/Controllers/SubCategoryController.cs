using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SpiceApp.Models;
using SpiceApp.Services;
using SpiceApp.ViewModels;

namespace SpiceApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SubCategoryController : Controller
    {
        private readonly ISubCategoryService subCategoryService;
        private readonly ICategoryService categoryService;
        [TempData]
        public string StatusMessage { get; set; }
        public SubCategoryController(ISubCategoryService subCategoryService,ICategoryService categoryService)
        {
            this.subCategoryService = subCategoryService;
            this.categoryService = categoryService;
        }
        public async Task<IActionResult> Index()
        {
            return View(await subCategoryService.GetAllSubCategories());
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || id.Value == 0)
                return NotFound();
            var subCategory = await subCategoryService.GetSubCategoryById(id.Value);
            if (subCategory != null)
            {
                return View(subCategory);
            }
            return NotFound();
        }

        public async Task<IActionResult> Create()
        {
            var subcategoriesList = await subCategoryService.GetAllSubCategories();
            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await categoryService.GetAllCategories(),
                SubCategory = new SubCategory(),
                SubCategoryList = subcategoriesList.OrderBy(n => n.Name).Distinct().Select(s => s.Name).ToList()
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var subcategory = await subCategoryService.DoesSubCategoryExists(model);
                if (subcategory != null)
                {
                    //Error
                    StatusMessage = "Error : Sub Category exists under " + subcategory.Category.Name + 
                        " category. Please use another name.";
                }
                else
                {
                    await subCategoryService.AddSubCategory(model.SubCategory);
                    return RedirectToAction(nameof(Index));                    
                }               
            }
            var subcategoriesList = await subCategoryService.GetAllSubCategories();
              var modelMV = new SubCategoryAndCategoryViewModel()
              {
                  CategoryList = await categoryService.GetAllCategories(),
                  SubCategory = new SubCategory(),
                  SubCategoryList = subcategoriesList.OrderBy(n => n.Name).Distinct().Select(s => s.Name).ToList(),
                  StatusMessage=StatusMessage
              };
              return View(modelMV);           
        }

        [ActionName("GetSubCategory")]
        public async Task<IActionResult> GetSubCategory(int id)
        {
            if (id != 0)
            {
                List<SubCategory> subCategories = new List<SubCategory>();
                subCategories = await subCategoryService.GetSubCategoryInCategory(id);
                return Json(new SelectList(subCategories, "Id", "Name"));
            }
            return NotFound();
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id==null)
            {
                return NotFound();
            }
            var subCategory = await subCategoryService.GetSubCategoryById(id.Value);
            if (subCategory==null)
            {
                return NotFound();
            }
            var subcategoriesList = await subCategoryService.GetAllSubCategories();
            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await categoryService.GetAllCategories(),
                SubCategory = subCategory,
                SubCategoryList = subcategoriesList.OrderBy(n => n.Name).Distinct().Select(s => s.Name).ToList()
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var subcategory = await subCategoryService.DoesSubCategoryExists(model);
                if (subcategory != null)
                {
                    //Error
                    StatusMessage = "Error : Sub Category exists under " + subcategory.Category.Name +
                        " category. Please use another name.";
                }
                else
                {
                    await subCategoryService.UpdateSubCategory(model.SubCategory);
                    return RedirectToAction(nameof(Index));
                }
            }
            var subcategoriesList = await subCategoryService.GetAllSubCategories();
            var modelMV = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await categoryService.GetAllCategories(),
                SubCategory = new SubCategory(),
                SubCategoryList = subcategoriesList.OrderBy(n => n.Name).Distinct().Select(s => s.Name).ToList(),
                StatusMessage = StatusMessage
            };
            return View(modelMV);
        }

        public async Task<IActionResult> Delete(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }
            var subCategory = await subCategoryService.GetSubCategoryById(id.Value);
            if (subCategory == null)
            {
                return NotFound();
            }
            return View(subCategory);
        }
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }
            if (await subCategoryService.DeleteSubCategory(id))
                return RedirectToAction(nameof(Index));
            return View();
        }
    }
}
