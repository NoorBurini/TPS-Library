using DataAccessLayer.Entities;
using DataAccessLayer.Data;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class BookRepository : IBookRepository
    {
        private ApplicationDbContext context;

        public BookRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }
        public IQueryable<Book> Books => context.Books;

        public bool CheckName(int id, string bookName)
        {
            Book book = context.Books.SingleOrDefault(b => b.ShelfId == id && b.BookName == bookName);
            if (book == null) return false;
            else return true;
        }

        public bool CheckNameinCategory(int categoryId, string bookName)
        {
            Book book = context.Books.SingleOrDefault(b => b.CategoryId == categoryId && b.BookName == bookName);
            if (book == null) return false;
            else return true;
        }

        public IEnumerable<Book> GetBookByShelf(int shelfId)
        {
            return context.Books.Where(b => b.ShelfId == shelfId).ToList();
        }
        public IEnumerable<Book> GetBooksByCategoryId(int categoryId)
        {
            return context.Books.Where(b => b.CategoryId == categoryId).ToList();
        }
        public IEnumerable<Book>? GetBookByName(int id, string bookName)
        {
            return context.Books.Where(b => b.BookName.Contains(bookName) && b.ShelfId == id).ToList();
        }
        
        public IEnumerable<Book>? GetBookByCategory(int id, string category)
        {
            return context.Books.Join(context.Categories,book => book.CategoryId,category => category.Id,
              (book, category) => new { Book = book, Category = category })
                .Where(x => x.Category.CategoryName.Contains(category) && x.Book.ShelfId == id)
                .Select(x => x.Book)
                .ToList();      
    
        }


        public IEnumerable<Book>? GetBookByNameInCategory(int categoryId, string bookName)
        {
            return context.Books.Where(b => b.BookName.Contains(bookName) && b.CategoryId == categoryId).ToList();
        }

       

        public IEnumerable<int> GetBooksID(IEnumerable<Book> books)
        {
            return books.Select(book => book.Id).ToList();
        }

        public Shelf GetShelfById(int id)
        {
            //return context.Shelfs.Where(s => s.Id == id).Single();
            return context.Shelfs.SingleOrDefault(i => i.Id == id);
        }
        public IEnumerable<int> GetAllShelfID()
        {
            return context.Shelfs.Select(b => b.Id).ToList();
        }
        public IEnumerable<int> GetAllCategoryID()
        {
            return context.Categories.Select(b => b.Id).ToList();
        }
        public List<int> NumofBookinCategories() 
        {
            return context.Books.GroupBy(b => b.CategoryId).Select(b => b.Count()).ToList();
        }
        public string GetCurrentBookName(int bookId)
        { 
            return context.Books.SingleOrDefault(m => m.Id == bookId).BookName;
        }
        public Book GetBookById(int bookId)
        {
            return context.Books.SingleOrDefault(i => i.Id == bookId);
        }
        
        public bool Add(int shelfId, string bookName, int categoryId)
        {
            context.Add(new Book { ShelfId = shelfId, BookName = bookName , CategoryId = categoryId});
            return Save();
        }


        public bool Delete(int bookId)
        {
            Book book = context.Books.SingleOrDefault(b => b.Id == bookId);

            IEnumerable<Rating> ratings = context.Ratings.Where(x => x.BookId == bookId); //delete all ratings that related with this book
            context.RemoveRange(ratings);
            context.Remove(book);
            return Save();
        }
        public bool Update(Book book)
        {
            context.Update(book);
            return Save();
        }

        public bool Save()
        {
            var saved = context.SaveChanges();
            return saved > 0;
        }
    }
}
