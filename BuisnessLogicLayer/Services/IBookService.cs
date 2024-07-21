using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLogicLayer.Services
{
    public interface IBookService
    {
        IQueryable<Book> Books { get; }
        bool CheckName(int id, string bookName);
        bool CheckNameinCategory(int categoryId, string bookName);

        IEnumerable<Book> GetBookByShelf(int shelfId);
        IEnumerable<Book>? GetBookByName(int id, string bookName);
        IEnumerable<Book>? GetBookByCategory(int id, string category);
        IEnumerable<Book> GetBooksByCategoryId(int categoryId);
        IEnumerable<Book>? GetBookByNameInCategory(int categoryId, string bookName);

        Shelf GetShelfById(int id);
        IEnumerable<int> GetBooksID(IEnumerable<Book> books);
        IEnumerable<int> GetAllShelfID();
        IEnumerable<int> GetAllCategoryID();
        List<int> NumofBookinCategories();
        string GetCurrentBookName(int bookId);
        Book GetBookById(int bookId);

        bool Add(int shelfId, string bookName, int categoryId);
        bool Delete(int bookId);
        bool Update(Book book);
        bool Save();

    }
}
