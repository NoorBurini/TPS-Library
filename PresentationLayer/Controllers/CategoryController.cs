using BuisnessLogicLayer.Services;
using BuisnessLogicLayer.ViewModels;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;


        public CategoryController(ICategoryService service)
        {
            _categoryService = service;
        }

        [HttpGet]
        public IActionResult Index(IEnumerable<int>? c = null, int? C_id = null)
        {
            if (c.Any() && c != null)
            {
                IEnumerable<Category> categories = _categoryService.Categories.Where(row => c.Contains(row.Id)).ToList();
                return View(categories);
            }
            else
            {
                if (TempData["CategoryNotFound"] != null)
                {
                    ViewBag.CategoryNotFound = TempData["CategoryNotFound"];
                }
                else if (TempData["Delete1"] != null)
                {
                    ViewBag.Delete1 = TempData["Delete1"];
                    ViewBag.Delete2 = TempData["Delete2"];

                }
                else if (C_id != null)//the delete is Done
                {
                    ViewBag.Deleted = "Category is deleted successfully";
                }
            }
            return View(_categoryService.Categories);
        }

        [HttpPost]
        public IActionResult Index(string categoryName)
        {
            bool exist = _categoryService.CheckCategoryName(categoryName);
            if (!string.IsNullOrEmpty(categoryName))
            {
                exist = _categoryService.CheckCategoryName(categoryName);
            }

            if (!string.IsNullOrEmpty(categoryName) && exist == false)
            {
                _categoryService.Add(categoryName);
                ViewBag.addedCategory = categoryName + " Category is Added Successfully";
            }
            else if (!string.IsNullOrEmpty(categoryName) && exist == true)
            {
                ViewBag.existCategory = categoryName + " Category is Already Exist";
            }

            return View(_categoryService.Categories);
        }


        [HttpGet]
        public IActionResult EditCategory(int id)
        {
            if (TempData["existCategory"] != null)
            {
                ViewBag.existCategory = TempData["existCategory"];
            }
            if (id == null)
            {
                return NotFound();
            }

            Category? category = _categoryService.Categories.SingleOrDefault(m => m.Id == id);

            if (category == null)
            {
                return NotFound();
            }
            var newCategory = new EditCategoryViewModel
            {
                Id = category.Id,
                CategoryName = category.CategoryName,

            };

            return View(newCategory);
        }

        [HttpPost]
        public IActionResult EditCategory(int id, EditCategoryViewModel newCategory)
        {

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit category");
                return View("EditCategory", newCategory);
            }
            Category? category = _categoryService.Categories.SingleOrDefault(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            bool exist = _categoryService.CheckCategoryName(newCategory.CategoryName);
            if (exist == true)
            {
                TempData["existCategory"] = newCategory.CategoryName + " is Already Exist";
                return RedirectToAction("EditCategory");
            }


            category.Id = id;
            category.CategoryName = newCategory.CategoryName;
            

            _categoryService.Update(category);
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult DeleteCategory(int categoryId)
        {
            if (categoryId == null)
            {
                return NotFound();
            }

            Category? category = _categoryService.Categories.SingleOrDefault(m => m.Id == categoryId);

            if (category == null)
            {
                return NotFound();
            }
            TempData["Delete1"] = "Are you sure you want to delete this category ? " + category.CategoryName;
            TempData["Delete2"] = "Be Aware That All Related Books Will Be Deleted Too";
            TempData["categoryId"] = category.Id;

            return RedirectToAction("Index");

        }

        [HttpPost, ActionName("DeleteCategory")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed()
        {
            int categoryId;
            categoryId = (int)TempData["categoryId"];
            Category category = _categoryService.Categories.SingleOrDefault(m => m.Id == categoryId);
            _categoryService.Delete(category);
            _categoryService.DeleteRelatedBooks(category.Id);
            return RedirectToAction("Index", new { C_id = category.Id });
        }

        [HttpPost]
        public IActionResult SearchCategory(string categoryName)
        {
            IEnumerable<Category>? categories = _categoryService.GetCategoryByName(categoryName);

            if (categories.Any())
            {
                IEnumerable<int> CategoriesIdList = _categoryService.GetCategoriesID(categories);
                return RedirectToAction("Index", new { c = CategoriesIdList });
            }
            else
            {
                TempData["CategoryNotFound"] = "No Categories Found";
                return RedirectToAction("Index");
            }
        }

    }
}
