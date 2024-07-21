using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using DataAccessLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLogicLayer.Services
{
    public class BookService : IBookService
    {

        private readonly IBookRepository _bookRepository;

        public BookService(IBookRepository rep)
        {
            _bookRepository = rep;
        }
        public IQueryable<Book> Books => _bookRepository.Books;

        public bool CheckName(int id, string bookName)
        {
            return _bookRepository.CheckName(id, bookName);
        }
        public bool CheckNameinCategory(int categoryId, string bookName)
        {
            return _bookRepository.CheckNameinCategory(categoryId,bookName);
        }

        public IEnumerable<Book> GetBookByShelf(int shelfId)
        {
            return _bookRepository.GetBookByShelf(shelfId);
        }
        public IEnumerable<Book>? GetBookByName(int id, string bookName)
        {
            return _bookRepository.GetBookByName(id, bookName);
        }
        public IEnumerable<Book> GetBooksByCategoryId(int categoryId)
        {
            return _bookRepository.GetBooksByCategoryId(categoryId);
        }
        public IEnumerable<Book>? GetBookByNameInCategory(int categoryId, string bookName)
        {
            return _bookRepository.GetBookByNameInCategory(categoryId, bookName);
        }

        public IEnumerable<Book>? GetBookByCategory(int id, string category)
        {
            return _bookRepository.GetBookByCategory(id, category);
        }
        public IEnumerable<int> GetBooksID(IEnumerable<Book> books)
        {
            return _bookRepository.GetBooksID(books);
        }

        public Shelf GetShelfById(int Shelfid)
        {
            return _bookRepository.GetShelfById(Shelfid);
        }
        public IEnumerable<int> GetAllShelfID()
        {
            return _bookRepository.GetAllShelfID();
        }
        public IEnumerable<int> GetAllCategoryID()
        {
            return _bookRepository.GetAllCategoryID();
        }
        public List<int> NumofBookinCategories()
        {
            return _bookRepository.NumofBookinCategories();
        }
        public string GetCurrentBookName(int bookId) 
        {
            return _bookRepository.GetCurrentBookName(bookId);
        }
        public Book GetBookById(int bookId)
        {
            return _bookRepository.GetBookById(bookId);
        }

        

        public bool Add(int shelfId, string bookName, int categoryId)
        {
            return _bookRepository.Add(shelfId, bookName, categoryId);
        }


        public bool Delete(int bookId)
        {
            return _bookRepository.Delete(bookId);
        }
        public bool Update(Book book)
        {
            return _bookRepository.Update(book);
        }

        public bool Save()
        {
            return _bookRepository.Save();
        }
    }
}
