using BuisnessLogicLayer.Services;
using BuisnessLogicLayer.ViewModels;
using DataAccessLayer.Entities;
using DataAccessLayer.Migrations;
using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using static System.Reflection.Metadata.BlobBuilder;

namespace PresentationLayer.Controllers
{
    public class BookController : Controller
    {
        private IBookService _bookService;
        private ICategoryService _categoryService;
        private IShelfService _shelfService;
        private IRatingService _ratingService;
        private readonly IWebHostEnvironment _hostingEnvironment;


        public BookController(IBookService service, ICategoryService categoryservice, IShelfService shelfservice, IRatingService ratingService, IWebHostEnvironment hostingEnvironment)
        {
            _bookService = service;
            _categoryService = categoryservice;
            _shelfService = shelfservice;
            _ratingService = ratingService;
            _hostingEnvironment = hostingEnvironment;
        }


        [HttpGet]

        public IActionResult ShowBooks(int id, IEnumerable<int>? b = null, int? B_id = null) //id for the shelf
        {
            Shelf shelf = _bookService.GetShelfById(id);
            ViewBag.ShelfName = shelf.ShelfName;
            ViewBag.ShelfId = shelf.Id;

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

            IEnumerable<Book> books = _bookService.GetBookByShelf(id);
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
                    string imageFileName =null;
                    imageFileNamesDictionary.Add(book.Id, imageFileName);
                }
            }
            ViewBag.ImageFileNamesDictionary = imageFileNamesDictionary;

            if (b.Any() && b != null)
            {
                IEnumerable<Book> BooksAfterSearch = _bookService.Books.Where(row => b.Contains(row.Id)).ToList();
                IQueryable<string> categories = _categoryService.GetAllCategories();
                ViewBag.CategoryList = categories;
                return View(BooksAfterSearch);
            }
            else
            {
                if (TempData["ErrorMessage"] != null)//Book not found
                {
                    ViewBag.ErrorMessage = TempData["ErrorMessage"];
                }
                else if (TempData["SearchString"] != null)
                {
                    ViewBag.SearchString = TempData["SearchString"];
                }
                else if (TempData["Delete"] != null && B_id == null)
                {
                    ViewBag.Delete = TempData["Delete"];
                }
                else if (B_id != null)//the delete is Done
                {
                    ViewBag.Deleted = "Book is deleted successfully";
                }
                else if (TempData["addedCategory"] != null)
                {
                    ViewBag.addedCategory = TempData["addedCategory"];
                } 
                else if (TempData["existCategory"] != null)
                {
                    ViewBag.existCategory = TempData["existCategory"];
                }
                else if (TempData["rated"] != null)
                {
                    ViewBag.rated = TempData["rated"];
                }
                else if (TempData["addedRating"] != null)
                {
                    ViewBag.addedRating = TempData["addedRating"];
                }
                else if (TempData["uploaded"] != null)
                {
                    ViewBag.uploaded = TempData["uploaded"];
                }
                else if (TempData["BookNotUploaded"] != null)
                {
                    ViewBag.BookNotUploaded = TempData["BookNotUploaded"];
                }
                

                IQueryable<string> categories = _categoryService.GetAllCategories();
                ViewBag.CategoryList = categories;

                return View(_bookService.GetBookByShelf(id));
            }
        }

        [HttpPost]
        public IActionResult ShowBooks(int id, string? bookName = null, string? categoryName = null) //id for the shelf
        {
            Shelf shelf = _bookService.GetShelfById(id);
            ViewBag.ShelfName = shelf.ShelfName;
            ViewBag.ShelfId = id;

            bool exist = false;
            if (!string.IsNullOrEmpty(bookName))
            {
                exist = _bookService.CheckName(id, bookName);
            }
            

            if (!string.IsNullOrEmpty(bookName) && !string.IsNullOrEmpty(categoryName) && exist == false)
            {
                int categoryId = _categoryService.GetIdByName(categoryName);
                _bookService.Add(id, bookName, categoryId);
                ViewBag.Added = bookName + " is Added Successfully";
            }
            else if (!string.IsNullOrEmpty(bookName) && exist == true)
            {
                ViewBag.Exist = bookName + " is Already Exist in this Shelf";
            }

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

            IEnumerable<Book> books = _bookService.GetBookByShelf(id);
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

            IQueryable<string> categories = _categoryService.GetAllCategories();
            ViewBag.CategoryList = categories;

            return View(_bookService.GetBookByShelf(id));
        }

        [HttpGet]
        public IActionResult DeleteBook(int bookId)
        {
            if (bookId == null)
            {
                return NotFound();
            }

            Book? book = _bookService.Books.SingleOrDefault(m => m.Id == bookId);

            if (book == null)
            {
                return NotFound();
            }

            TempData["Delete"] = "Are you sure you want to delete this book ? " + book.BookName;
            TempData["BookId"] = book.Id;

            //return RedirectToAction("ShowBooks", new { id = book.ShelfId });

            int shelfId = book.ShelfId;
            int categoryId = book.CategoryId;
            string origin = TempData["Origin"] as string;
            switch (origin)
            {
                case "ShowBooks":
                    return RedirectToAction("ShowBooks", new { id = shelfId });
                case "ShowBooksInCategory":
                    return RedirectToAction("ShowBooksInCategory", new { id = categoryId });
                default:
                    return RedirectToAction("Login", "Home"); // Default redirection
            }
        }

        [HttpPost, ActionName("DeleteBook")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed()
        {
            int bookId;
            bookId = (int)TempData["BookId"];

            Book book = _bookService.Books.SingleOrDefault(m => m.Id == bookId);
            
            _bookService.Delete(bookId);//after delete the book we will delete its pdf and image 

            // Get the path to the folder in wwwroot based on the bookId
            string bookPath = Path.Combine(_hostingEnvironment.WebRootPath, "Books", bookId.ToString());
            string imgPath = Path.Combine(_hostingEnvironment.WebRootPath, "Images", bookId.ToString());

            if (Directory.Exists(bookPath))
            {
                // Delete all contents of the folder
                Directory.Delete(bookPath, true);
            }
            if (Directory.Exists(imgPath))
            {
                // Delete all contents of the folder
                Directory.Delete(imgPath, true);
            }

            //return RedirectToAction("ShowBooks", new { id = shelfId, B_id = bookId });
            int shelfId = book.ShelfId;
            int categoryId = book.CategoryId;
            string origin = TempData["Origin"] as string;
            switch (origin)
            {
                case "ShowBooks":
                    return RedirectToAction("ShowBooks", new { id = shelfId, B_id = bookId });
                case "ShowBooksInCategory":
                    return RedirectToAction("ShowBooksInCategory", new { id = categoryId, B_id = bookId });
                default:
                    return RedirectToAction("Login", "Home"); // Default redirection
            }
        }

        [HttpGet]
        public IActionResult EditBook(int bookId)
        {
            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
            }
            if (TempData["Exist"] != null)
            {
                ViewBag.Exist = TempData["Exist"];
            }
            if (TempData["ExistCategory"] != null)
            {
                ViewBag.ExistCategory = TempData["ExistCategory"];
            }
            if (bookId == null)
            {
                return NotFound();
            }

            Book? book = _bookService.Books.SingleOrDefault(m => m.Id == bookId);
            ViewBag.shelfId = book.ShelfId;
            ViewBag.categoryId = book.CategoryId;
            if (book == null)
            {
                return NotFound();
            }
            string categoryName = _categoryService.GetCategoryById(book.CategoryId);
            string shelfName = _shelfService.GetShelfNameById(book.ShelfId);

            var newBook = new EditBookViewModel
            {
                Id = book.Id,
                BookName = book.BookName,
                ShelfName = shelfName,
                CategoryName = categoryName
            };

            IEnumerable<string> Shelfs= _shelfService.GetAllShelfs();
            ViewBag.ShelfList = Shelfs;

            IQueryable<string> categories = _categoryService.GetAllCategories();
            ViewBag.CategoryList = categories;

            string origin = TempData["Origin"] as string;
            ViewBag.origin = origin;
            ViewBag.bookId = bookId;
            return View(newBook);
        }

        [HttpPost]
        public IActionResult EditBook(int bookId, EditBookViewModel newBook,string origin)
        {
            IEnumerable<string> Shelfs = _shelfService.GetAllShelfs();
            ViewBag.ShelfList = Shelfs;

            IQueryable<string> categories = _categoryService.GetAllCategories();
            ViewBag.CategoryList = categories;

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit book");
                
                return View("EditBook", newBook);
            }

            // Retrieve the existing book
            Book book = _bookService.GetBookById(bookId);
  
            if (book == null)
            {
                return NotFound();
            }
            int shelfId = _shelfService.GetIdByName(newBook.ShelfName);
            int categoryId = _categoryService.GetIdByName(newBook.CategoryName);
            // Check if the book name has changed and if the new name exists on the same shelf
            if (book.BookName != newBook.BookName)
            {
                bool nameExist_shelf = _bookService.CheckName(shelfId, newBook.BookName);
                bool nameExist_category = _bookService.CheckNameinCategory(categoryId, newBook.BookName);
                
                if (nameExist_shelf)
                {
                    TempData["Exist"] = newBook.BookName + " is already exist in this shelf";
                    
                    return RedirectToAction("EditBook", new { bookId = bookId }); // Ensure redirection is correct
                }
                if (nameExist_category)
                {
                    TempData["ExistCategory"] = newBook.BookName + " is already exist in this category";

                    return RedirectToAction("EditBook", new { bookId = bookId }); // Ensure redirection is correct
                }
                book.BookName = newBook.BookName;
                book.ShelfId = shelfId; // Update shelf if needed
            }
            else
            {
                if (book.ShelfId != shelfId)
                {
                    bool nameExists = _bookService.CheckName(shelfId, newBook.BookName);
                    if (nameExists)
                    {
                        TempData["Exist"] = newBook.BookName + " is already exist in this shelf";
                        return RedirectToAction("EditBook", new { bookId = bookId }); // Ensure redirection is correct
                    }
                    book.ShelfId = shelfId; // Update shelf if needed
                }
            }
            // Update category if changed
           
            if (book.CategoryId != categoryId)
            {
                book.CategoryId = categoryId;
            }

            _bookService.Update(book); 

            //return RedirectToAction("ShowBooks", new { id = book.ShelfId });

            switch (origin)
            {
                case "ShowBooks":
                    return RedirectToAction("ShowBooks", new { id = book.ShelfId });
                case "ShowBooksInCategory":
                    return RedirectToAction("ShowBooksInCategory", new { id = book.CategoryId });
                default:
                    return RedirectToAction("Login", "Home"); // Default redirection
            }
        }

        public IActionResult SearchBook(int shelfId, int categoryId, string searchString) //shelfId
        {
            IEnumerable<Book>? books1 = _bookService.GetBookByName(shelfId, searchString);
            IEnumerable<Book>? books2 = _bookService.GetBookByCategory(shelfId, searchString);
            TempData["SearchString"] = searchString;
            IEnumerable<Book>? books3 = _bookService.GetBookByNameInCategory(categoryId, searchString);

            IQueryable<string> shelfs = _shelfService.GetAllShelfs();
           
            IQueryable<string> categories = _categoryService.GetAllCategories();
           

            if (books1.Any())
            {
                IEnumerable<int> BooksIdList = _bookService.GetBooksID(books1);
                string origin = TempData["Origin"] as string;
                switch (origin)
                {
                    case "ShowBooks":
                        return RedirectToAction("ShowBooks", new { id = shelfId, b = BooksIdList });
                    case "ShowBooksInCategory":
                        return RedirectToAction("ShowBooksInCategory", new { id = categoryId, b = BooksIdList });
                    default:
                        return RedirectToAction("Login", "Home"); // Default redirection
                }
            }
            else if (books2.Any()) 
            {
                IEnumerable<int> BooksIdList = _bookService.GetBooksID(books2);
                string origin = TempData["Origin"] as string;
                switch (origin)
                {
                    case "ShowBooks":
                        return RedirectToAction("ShowBooks", new { id = shelfId, b = BooksIdList });
                    case "ShowBooksInCategory":
                        return RedirectToAction("ShowBooksInCategory", new { id = categoryId, b = BooksIdList });
                    default:
                        return RedirectToAction("Login", "Home"); // Default redirection
                }
            }
            else if (books3.Any())
            {
                IEnumerable<int> BooksIdList = _bookService.GetBooksID(books3);
                string origin = TempData["Origin"] as string;
                switch (origin)
                {
                    case "ShowBooks":
                        return RedirectToAction("ShowBooks", new { id = shelfId, b = BooksIdList });
                    case "ShowBooksInCategory":
                        return RedirectToAction("ShowBooksInCategory", new { id = categoryId, b = BooksIdList });
                    default:
                        return RedirectToAction("Login", "Home"); // Default redirection
                }
            }
            else
            {
                TempData["ErrorMessage"] = "No Books Found";
              
                string origin = TempData["Origin"] as string;
                switch (origin)
                {
                    case "ShowBooks":
                        return RedirectToAction("ShowBooks", new { id = shelfId });
                    case "ShowBooksInCategory":
                        return RedirectToAction("ShowBooksInCategory", new { id = categoryId});
                    default:
                        return RedirectToAction("Login", "Home"); // Default redirection
                }
            }
        }
        public IActionResult ViewBook(int bookId) 
        {
            string bookIdstr = bookId.ToString(); 
            var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath,"Books", bookIdstr); //bookIdstr here represents the folder name will be opened
            Book book = _bookService.GetBookById(bookId);
            int shelfId = book.ShelfId;
            int categoryId = book.CategoryId;

            if (!Directory.Exists(uploadsFolder))
            {
                TempData["BookNotUploaded"]  = "This Book is not uploaded yet";
                //return RedirectToAction("ShowBooks", new { id = shelfId }); 
                
                string origin = TempData["Origin"] as string;
                switch (origin)
                {
                    case "ShowBooks":
                        return RedirectToAction("ShowBooks", new { id = shelfId });
                    case "ShowBooksInCategory":
                        return RedirectToAction("ShowBooksInCategory", new { id = categoryId });
                    default:
                        return RedirectToAction("Login", "Home"); // Default redirection
                }
            }

            var pdfFiles = Directory.GetFiles(uploadsFolder, "*.pdf");

            if (pdfFiles.Length == 0)
            {
                return NotFound("No PDF files found in the folder."); // Or handle appropriately
            }
            else
            {
                var fileName = Path.GetFileName(pdfFiles[0]);
                var fileStream = new FileStream(pdfFiles[0], FileMode.Open, FileAccess.Read);
                Response.Headers["Content-Disposition"] = $"inline; filename=\"{fileName}\"";

                return File(fileStream, "application/pdf");             
            }
           
        }
        public IActionResult DownloadBook(int bookId)
        {
            string bookIdstr = bookId.ToString();
            var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "Books", bookIdstr); //bookIdstr here represents the folder name will be opened

            Book book = _bookService.GetBookById(bookId);
            int shelfId = book.ShelfId;
            int categoryId = book.CategoryId;

            if (!Directory.Exists(uploadsFolder))
            {
                TempData["BookNotUploaded"] = "This Book is not uploaded yet";
                //return RedirectToAction("ShowBooks", new { id = shelfId });
                string origin = TempData["Origin"] as string;
                switch (origin)
                {
                    case "ShowBooks":
                        return RedirectToAction("ShowBooks", new { id = shelfId });
                    case "ShowBooksInCategory":
                        return RedirectToAction("ShowBooksInCategory", new { id = categoryId });
                    default:
                        return RedirectToAction("Login", "Home"); // Default redirection
                }
            }

            var pdfFiles = Directory.GetFiles(uploadsFolder, "*.pdf");

            if (pdfFiles.Length == 0)
            {
                return NotFound("No PDF files found in the folder.");
            }
            else 
            {
                var fileName = Path.GetFileName(pdfFiles[0]);
                var fileStream = new FileStream(pdfFiles[0], FileMode.Open, FileAccess.Read);
                return File(fileStream, "application/pdf", fileName);
            }
           
        }
        
        [HttpPost]
        public IActionResult AddRate(int shelfId, int bookId, int ratingValue, string userId)
        {
            bool exist = false;
            if (!string.IsNullOrEmpty(userId))
            {
                exist = _ratingService.CheckUserId(userId, bookId);
            }

            if (ratingValue == null)
            {
                TempData["rated"] = "Please add rating value";
            }

            if (ratingValue != null && exist == false)
            {
                _ratingService.Add(bookId, ratingValue, userId);
                TempData["addedRating"] = "Your rating is saved successfully";
            }
            else if (exist == true)
            {
                TempData["rated"] = "You already rated this book";
            }
            

            List<int> ratings = _ratingService.BookRatings(bookId);
            Book book = _bookService.GetBookById(bookId);

            if (ratings != null)
            {
                float count = 0;
                float sum = 0;
                foreach (var x in ratings)
                {
                    sum += x;
                    count++;
                }
                book.RatingValue = (float)Math.Round(sum / count, 2);
            }
            else
            {
                book.RatingValue = 0;
            }
            _bookService.Update(book);

            int categoryId = book.CategoryId;

            string origin = TempData["Origin"] as string;
            switch (origin)
            {
                case "ShowBooks":
                    return RedirectToAction("ShowBooks", new { id = shelfId });
                case "ShowBooksInCategory":
                    return RedirectToAction("ShowBooksInCategory", new { id = categoryId });
                default:
                    return RedirectToAction("Login", "Home"); // Default redirection
            }
           
        }

        [HttpGet]
        public IActionResult UploadPDF(int bookId,int shelfId) 
        {
            string origin = TempData["Origin"] as string;
            ViewBag.origin = origin;
            ViewBag.bookId = bookId;
            ViewBag.shelfId = shelfId;
            Book book = _bookService.GetBookById(bookId);
            ViewBag.categoryId = book.CategoryId;
            if (TempData["InvalidFile"] != null)
            {
                ViewBag.InvalidFile = TempData["InvalidFile"];
                return View();
            }
            return View();        
        }

        [HttpPost]
        public async Task<IActionResult> UploadPDF(IFormFile file,int bookId,int shelfId,string origin)
        {
            if (file == null || file.Length == 0)
            {
                TempData["InvalidFile"] = "Invalid file";
                TempData["Origin"] = origin;
                return RedirectToAction("UploadPDF", new { bookId, shelfId });
            }

            var extension = Path.GetExtension(file.FileName).ToLower();
         

            if (extension != ".pdf")
            {
                TempData["InvalidFile"] = "Invalid file type. Please upload a PDF file.";
                TempData["Origin"] = origin;
                return RedirectToAction("UploadPDF", new { bookId, shelfId });
            }

            string bookIdstr = bookId.ToString();

            var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath,"Books", bookIdstr); //bookId here represents the folder name 

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            
            if (Directory.Exists(uploadsFolder))
            {
                // Delete all existing files in the folder
                foreach (var f in Directory.GetFiles(uploadsFolder))
                {
                    System.IO.File.Delete(f);
                }
                
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            TempData["uploaded"]= "File is uploaded successfully";
            //return RedirectToAction("ShowBooks", new { id = shelfId }); 

            Book book = _bookService.GetBookById(bookId);
            int categoryId = book.CategoryId;

            switch (origin)
            {
                case "ShowBooks":
                    return RedirectToAction("ShowBooks", new { id = shelfId });
                case "ShowBooksInCategory":
                    return RedirectToAction("ShowBooksInCategory", new { id = categoryId });
                default:
                    return RedirectToAction("Login", "Home"); // Default redirection
            }
        }
        [HttpGet]
        public IActionResult UploadImage(int bookId, int shelfId)
        {
            string origin = TempData["Origin"] as string;
            ViewBag.origin = origin;
            ViewBag.bookId = bookId;
            ViewBag.shelfId = shelfId;
            Book book = _bookService.GetBookById(bookId);
            ViewBag.categoryId = book.CategoryId;
            if (TempData["InvalidFile"] != null)
            {
                ViewBag.InvalidFile = TempData["InvalidFile"];
                return View();
            }
           
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file, int bookId, int shelfId, string origin)
        {
            if (file == null || file.Length == 0)
            {
                TempData["InvalidFile"] = "Invalid file";
                TempData["Origin"] = origin;
                return RedirectToAction("UploadImage", new { bookId, shelfId });
            }

            var validImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };

            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!validImageExtensions.Contains(extension))
            {
                TempData["InvalidFile"] = "Invalid file type. Please upload an image file (jpg, jpeg, png, gif, bmp).";
                TempData["Origin"] = origin;
                return RedirectToAction("UploadImage", new { bookId, shelfId });
            }

            string bookIdStr = bookId.ToString();

            var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "Images", bookIdStr); // BookId here represents the folder name 

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            if (Directory.Exists(uploadsFolder))
            {
                // Delete all existing files in the folder
                foreach (var f in Directory.GetFiles(uploadsFolder))
                {
                    System.IO.File.Delete(f);
                }

            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            TempData["uploaded"] = "File is uploaded successfully";
            //return RedirectToAction("ShowBooks", new { id = shelfId });

            Book book = _bookService.GetBookById(bookId);
            int categoryId = book.CategoryId;
           
            switch (origin)
            {
                case "ShowBooks":
                    return RedirectToAction("ShowBooks", new { id = shelfId });
                case "ShowBooksInCategory":
                    return RedirectToAction("ShowBooksInCategory", new { id = categoryId });
                default:
                    return RedirectToAction("Login", "Home"); // Default redirection
            }
        }

        [HttpGet]

        public IActionResult ShowBooksInCategory(int id, IEnumerable<int>? b = null, int? B_id = null) //id for category
        {
            string categoryName = _categoryService.GetCategoryById(id);

            ViewBag.CategoryName = categoryName;
            ViewBag.CategoryId = id;

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

            IEnumerable<Book> books = _bookService.GetBooksByCategoryId(id);
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

            if (b.Any() && b != null)
            {
                IEnumerable<Book> BooksAfterSearch = _bookService.Books.Where(row => b.Contains(row.Id)).ToList();
                IQueryable<string> shelfs = _shelfService.GetAllShelfs();
                ViewBag.ShelfList = shelfs;
                return View(BooksAfterSearch);
            }
            else
            {
                if (TempData["ErrorMessage"] != null)//Book not found
                {
                    ViewBag.ErrorMessage = TempData["ErrorMessage"];
                }
                else if (TempData["SearchString"] != null)
                {
                    ViewBag.SearchString = TempData["SearchString"];
                }
                else if (TempData["Delete"] != null && B_id == null)
                {
                    ViewBag.Delete = TempData["Delete"];
                }
                else if (B_id != null)//the delete is Done
                {
                    ViewBag.Deleted = "Book is deleted successfully";
                }
                else if (TempData["addedCategory"] != null)
                {
                    ViewBag.addedCategory = TempData["addedCategory"];
                }
                else if (TempData["existCategory"] != null)
                {
                    ViewBag.existCategory = TempData["existCategory"];
                }
                else if (TempData["rated"] != null)
                {
                    ViewBag.rated = TempData["rated"];
                }
                else if (TempData["addedRating"] != null)
                {
                    ViewBag.addedRating = TempData["addedRating"];
                }
                else if (TempData["uploaded"] != null)
                {
                    ViewBag.uploaded = TempData["uploaded"];
                }
                else if (TempData["BookNotUploaded"] != null)
                {
                    ViewBag.BookNotUploaded = TempData["BookNotUploaded"];
                }
                


                IQueryable<string> shelfs = _shelfService.GetAllShelfs();
                ViewBag.ShelfList = shelfs;

                return View(_bookService.GetBooksByCategoryId(id));
            }
        }

            [HttpPost]
            public IActionResult ShowBooksInCategory(int id, string? bookName = null, string? shelfName = null) //id for category
            {

                Category category = _categoryService.GetCategoryById_(id);
                ViewBag.CategoryName = category.CategoryName;
                ViewBag.CategoryId = id;

                bool exist = false;
                if (!string.IsNullOrEmpty(bookName))
                {
                    //Book book = _bookService.GetBookByName(,bookName);
                    //int shelfId = book.ShelfId;
                    exist = _bookService.CheckNameinCategory(id, bookName);
                    //exist = _bookService.CheckName(shelfId, bookName);
                }


                if (!string.IsNullOrEmpty(bookName) && !string.IsNullOrEmpty(shelfName) && exist == false)
                {
                    int shelfId = _shelfService.GetIdByName(shelfName);
                    _bookService.Add(shelfId, bookName, id);
                    ViewBag.Added = bookName + " is Added Successfully";
                }
                else if (!string.IsNullOrEmpty(bookName) && exist == true)
                {
                    ViewBag.Exist = bookName + " is Already Exist in this Category";
                }

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

            IEnumerable<Book> books = _bookService.GetBooksByCategoryId(id);
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

                IQueryable<string> shelfs = _shelfService.GetAllShelfs();
                ViewBag.ShelfList = shelfs;

                return View(_bookService.GetBooksByCategoryId(id));
            }



        }
}
