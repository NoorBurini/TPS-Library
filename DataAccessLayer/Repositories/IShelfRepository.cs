using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public interface IShelfRepository
    {
        IQueryable<Shelf> Shelfs { get; }
        IQueryable<string> GetAllShelfs();
        IEnumerable<Shelf> GetShelfByName(string shelfName);
        IEnumerable<int> GetShelfsID(IEnumerable<Shelf> shelfs);
        bool CheckName(string shelfName);
        string GetShelfNameById(int shelfId);
        int GetIdByName(string shelfName);
        public List<int> NumofBookinShelfs();
        bool Add(string shelfName);
        bool Delete(Shelf shelf);
        bool DeleteRelatedBooks(int shelfId);
        bool Update(Shelf shelf);
        bool Save();
    }
}
