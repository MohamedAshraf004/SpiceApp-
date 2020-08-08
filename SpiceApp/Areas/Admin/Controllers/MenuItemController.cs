using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SpiceApp.Services;
using SpiceApp.Utility;
using SpiceApp.ViewModels;

namespace SpiceApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.ManagerUser)]
    public class MenuItemController : Controller
    {
        private readonly IWebHostEnvironment hostEnvironment;
        private readonly IMenuItemService menuItemService;
        private readonly ICategoryService categoryService;
        private readonly ISubCategoryService subCategoryService;

        [BindProperty]
        public MenuItemViewModel model { get; set; }

        public MenuItemController(IWebHostEnvironment hostEnvironment, IMenuItemService menuItemService,
            ICategoryService categoryService, ISubCategoryService subCategoryService)
        {
            this.hostEnvironment = hostEnvironment;
            this.menuItemService = menuItemService;
            this.categoryService = categoryService;
            this.subCategoryService = subCategoryService;
            this.model = new MenuItemViewModel()
            {
                Categories = categoryService.GetAllCategories().Result.ToList(),
                MenuItem = new Models.MenuItem(),
            };
        }
        public async Task<IActionResult> Index()
        {
            return View(await menuItemService.GetAllMenuItems());
        }
        public IActionResult Create()
        {
            return View(model);
        }
        public async Task<IActionResult> Details(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }
            model = new MenuItemViewModel()
            {
                MenuItem = await menuItemService.GetMenuItemById(id.Value),
                Categories= categoryService.GetAllCategories().Result.ToList()
            };
            if (model.MenuItem == null)
            {
                return NotFound();
            }
            return View(model);
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost()
        {
            model.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.Img != null)
            {
                await menuItemService.AddMenuItem(model.MenuItem);
                model.MenuItem.Image = ProcessUploadedFile(model);
            }
            else
            {
                await menuItemService.AddMenuItem(model.MenuItem);
                var menuItemFromDb = await menuItemService.GetMenuItemById(model.MenuItem.Id);
                string webRootPath = hostEnvironment.WebRootPath;
                var uploads = Path.Combine(webRootPath, @"images\" + SD.DefaultFoodImage);
                System.IO.File.Copy(uploads, webRootPath + @"\images\MenuItem\" + model.MenuItem.Id + ".png");
                menuItemFromDb.Image = @"\images\MenuItem\" + model.MenuItem.Id + ".png";
            }
            await menuItemService.Commit();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            model.MenuItem = await menuItemService.GetMenuItemById(id.Value);
            model.SubCategories = await subCategoryService.GetAllSubCategories();
            if (model.MenuItem == null)
            {
                return NotFound();
            }
            return View(model);
        }
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            model.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());
            if (id == null)
            {
                model.SubCategories = await subCategoryService.GetAllSubCategories();
                return NotFound();
            }
            if (model.MenuItem == null)
            {
                model.SubCategories = await subCategoryService.GetAllSubCategories();
                return NotFound();
            }
            if (model.Img != null)
            {
                await menuItemService.UpdateMenuItem(model.MenuItem);
                if (model.MenuItem.Image != null)
                {
                    string webRootPath = hostEnvironment.WebRootPath;
                    var imgPath = Path.Combine(webRootPath, model.MenuItem.Image.TrimStart('\\'));
                    var imgName = imgPath.Split("MenuItem\\");
                    if (imgName.Length > 1)
                    {
                        System.IO.File.Delete(imgName[1]);
                    }
                }

                model.MenuItem.Image = ProcessUploadedFile(model);
            }
            await menuItemService.Commit();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }
            model = new MenuItemViewModel()
            {
                MenuItem = await menuItemService.GetMenuItemById(id.Value),
                Categories = categoryService.GetAllCategories().Result.ToList()
            };
            if (model.MenuItem == null)
            {
                return NotFound();
            }
            return View(model);
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
            var MenuItem = await menuItemService.GetMenuItemById(id);
            if (MenuItem.Image != null)
            {
                string webRootPath = hostEnvironment.WebRootPath;
                var imgPath = Path.Combine(webRootPath, MenuItem.Image.TrimStart('\\'));
                var imgName = imgPath.Split("MenuItem\\");
                if (imgName.Length > 1)
                {
                    System.IO.File.Delete(imgName[1]);
                }
            }
            if (await menuItemService.DeleteMenuItem(id))
                return RedirectToAction(nameof(Index));
            return View();
        }
        private string ProcessUploadedFile(MenuItemViewModel model)
        {
            string path = hostEnvironment.WebRootPath + "\\images\\MenuItem\\";
            var extension = Path.GetExtension(model.Img.FileName);
            string uniqueFileName = model.MenuItem.Id.ToString() + extension;
            if (model.Img.Length > 0)
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                using (FileStream filestream = System.IO.File.Create(path + uniqueFileName))
                {
                    model.Img.CopyTo(filestream);
                    filestream.Flush();
                }
                uniqueFileName = "MenuItem\\" + uniqueFileName;
                return uniqueFileName;
            }
            return null;
        }
    }


}

