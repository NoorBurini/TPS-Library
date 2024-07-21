using BuisnessLogicLayer.Services;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IShelfService _shelfService;
        private readonly IBookService _bookService;
        private readonly ICategoryService _categoryService;


        public DashboardController(IShelfService shelfService, IBookService bookService, ICategoryService categoryService)
        {
            _shelfService = shelfService;
            _bookService = bookService;
            _categoryService = categoryService;
        }
        public IActionResult Index()
        {
            
            List<int> bookCounts = _shelfService.NumofBookinShelfs();
            ViewBag.bookCounts= bookCounts;

            IQueryable<string> categories = _categoryService.GetAllCategories();
            ViewBag.categories = categories;

            List<int> categoryCounts = _bookService.NumofBookinCategories();
            ViewBag.categoryCounts = categoryCounts;


            IQueryable<Book> books = _bookService.Books;
            ViewBag.bookCount = books.Count();


            return View(_shelfService.Shelfs);
           
        }
    }
}
