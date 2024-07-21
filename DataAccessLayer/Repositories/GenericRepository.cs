using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    internal class GenericRepository : IGenericRepository
    {
        private ApplicationDbContext context;

        public GenericRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }
        public IQueryable<Book> Books => context.Books;

        public IQueryable<Shelf> Shelfs => context.Shelfs;

        public bool Add(int shelfId, string bookName)
        {
            throw new NotImplementedException();
        }

        public bool Add(string shelfName)
        {
            throw new NotImplementedException();
        }

        public bool Delete(int bookId)
        {
            Book book = context.Books.SingleOrDefault(b => b.Id == bookId);
            context.Remove(book);
            return Save();
        }

        public bool Delete(Shelf shelf)
        {
            context.Remove(shelf);
            return Save();
        }

        public bool Update(object obj)
        {
            context.Update(obj);
            return Save();
        }
        public bool Save()
        {
            var saved = context.SaveChanges();
            return saved > 0;
        }
    }
}
