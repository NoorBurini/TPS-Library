using DataAccessLayer.Entities;
using DataAccessLayer.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Migrations;

namespace DataAccessLayer.Repositories
{
    public class ShelfRepository : IShelfRepository
    {
        private ApplicationDbContext context;

        public ShelfRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }
        public IQueryable<Shelf> Shelfs => context.Shelfs;
        public IQueryable<string> GetAllShelfs() 
        { 
            return context.Shelfs.Select(c => c.ShelfName);
        }
        public bool CheckName(string shelfName)
        {
            Shelf shelf = context.Shelfs.SingleOrDefault(s => s.ShelfName == shelfName);
            if (shelf == null) return false;
            else return true;
        }
        public string GetShelfNameById(int shelfId) {
            Shelf s = context.Shelfs.SingleOrDefault(i => i.Id == shelfId);
            return s.ShelfName;
        }
        public int GetIdByName(string shelfName)
        {
            Shelf s = context.Shelfs.SingleOrDefault(i => i.ShelfName == shelfName);
            return s.Id;
        }

        public IEnumerable<Shelf> GetShelfByName(string shelfName)
        {
            return context.Shelfs.Where(s => s.ShelfName.Contains(shelfName)).ToList();
        }
        public IEnumerable<int> GetShelfsID(IEnumerable<Shelf> shelfs)
        {
            return shelfs.Select(s => s.Id).ToList();
        }
        public List<int> NumofBookinShelfs() { 
            return context.Shelfs.Select(shelf => context.Books.Count(book => book.ShelfId == shelf.Id)).ToList();
        }
        public bool Add(string shelfName)
        {
            context.Add(new Shelf { ShelfName = shelfName });
            return Save();
        }
        public bool Update(Shelf shelf)
        {
            context.Update(shelf);
            return Save();
        }
        public bool Delete(Shelf shelf)
        {

            context.Remove(shelf);
            return Save();
        }

        public bool DeleteRelatedBooks(int shelfId)
        {
            IEnumerable<Book> books = context.Books.Where(x => x.ShelfId == shelfId);
            context.RemoveRange(books);
            return Save();
        }

        public bool Save()
        {
            var saved = context.SaveChanges();
            return saved > 0;
        }
    }
}
