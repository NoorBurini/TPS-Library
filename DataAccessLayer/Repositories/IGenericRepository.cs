using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public interface IGenericRepository
    {
        IQueryable<Book> Books { get; }
        bool Add(int shelfId, string bookName);
        bool Delete(int bookId);
        IQueryable<Shelf> Shelfs { get; }
        bool Add(string shelfName);
        bool Delete(Shelf shelf);
        bool Update(object obj);
        bool Save();
    }
}
