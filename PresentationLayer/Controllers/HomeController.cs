using BuisnessLogicLayer.Services;
using BuisnessLogicLayer.ViewModels;
using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using PresentationLayer.Models;
using System;
using System.Diagnostics;

namespace PresentationLayer.Controllers
{
    public class HomeController : Controller
    {

        public readonly UserManager<AppUser> _userManager;
        public readonly SignInManager<AppUser> _signinManager;
        public readonly ApplicationDbContext _context;

        private IBookService _bookService;
        private ICategoryService _categoryService;
        private readonly IWebHostEnvironment _hostingEnvironment;


        public HomeController(UserManager<AppUser> userManager, SignInManager<AppUser> SigninManger, ApplicationDbContext context, IBookService bookService, ICategoryService categoryService , IWebHostEnvironment hostingEnvironment)
        {
            _userManager = userManager;
            _signinManager = SigninManger;
            _context = context;
            _bookService = bookService;
            _categoryService = categoryService;
            _hostingEnvironment = hostingEnvironment;
                  
    }
        public IActionResult Login()
        {
            var response = new LogInViewModel(); //in case of reload the page it will save the inputs
            return View(response);
        }
       
        [HttpPost]
        public async Task<IActionResult> Login(LogInViewModel loginVM)
        {
            if (!ModelState.IsValid)
                return View(loginVM);

            var user = await _userManager.FindByEmailAsync(loginVM.EmailAddress);

            if (user != null)
            {
                var passwordCheck = await _userManager.CheckPasswordAsync(user, loginVM.Password);
                if (passwordCheck == true)
                {
                    var result = await _signinManager.PasswordSignInAsync(user, loginVM.Password, false, false);
                    if (result.Succeeded)
                    {
                        if(await _userManager.IsInRoleAsync(user,"user"))
                            return RedirectToAction("Welcome", "Home");
                        if (await _userManager.IsInRoleAsync(user, "admin"))
                            return RedirectToAction("Index", "Dashboard");
                        
                    }
                }
                TempData["Error"] = "Wrong Credentials. Please Try Again";
                return View(loginVM);
            }
            TempData["Error"] = "Wrong credentials. Please try again";
            return View(loginVM);
        }

        public IActionResult Welcome()
        {
            IEnumerable<Book> books = _bookService.Books.OrderByDescending(book => book.RatingValue).Take(5); 

            var result = _bookService.Books
                        .Join(
                            _categoryService.Categories,
                            book => book.CategoryId,
                            category => category.Id,
                            (book, category) => new
                            {
                                CategoryId = book.CategoryId,
                                CategoryName = category.CategoryName
                            })
                        .ToList();
            ViewBag.showCategory = result;

            var imageFileNamesDictionary = new Dictionary<int, string>();

            foreach (var book in books)
            {
                string folderPath = Path.Combine(_hostingEnvironment.WebRootPath, "Images", book.Id.ToString());
                if (Directory.Exists(folderPath))
                {
                    string[] imageFileNames = Directory.GetFiles(folderPath);
                    string imageFileName = (imageFileNames != null && imageFileNames.Length > 0) ? Path.GetFileName(imageFileNames[0]) : null;
                    imageFileNamesDictionary.Add(book.Id, imageFileName);
                }
                else
                {
                    string imageFileName = null;
                    imageFileNamesDictionary.Add(book.Id, imageFileName);
                }
            }
            ViewBag.ImageFileNamesDictionary = imageFileNamesDictionary;

            return View(books);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
