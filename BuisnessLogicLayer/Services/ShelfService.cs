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
    public class ShelfService : IShelfService
    {

        private readonly IShelfRepository _shelfRepository;

        public ShelfService(IShelfRepository repo)
        {
            _shelfRepository = repo;
        }
        public IQueryable<Shelf> Shelfs => _shelfRepository.Shelfs;
        public bool CheckName(string shelfName)
        {
            return _shelfRepository.CheckName(shelfName);
        }
        public string GetShelfNameById(int shelfId)
        { 
            return _shelfRepository.GetShelfNameById(shelfId);
        }
        public int GetIdByName(string categoryName)
        {
            return _shelfRepository.GetIdByName(categoryName);
        }
        public IQueryable<string> GetAllShelfs()
        {
            return _shelfRepository.GetAllShelfs();
        }
        public IEnumerable<Shelf> GetShelfByName(string shelfName)
        {
            return _shelfRepository.GetShelfByName(shelfName);
        }
        public IEnumerable<int> GetShelfsID(IEnumerable<Shelf> shelfs)
        {
            return _shelfRepository.GetShelfsID(shelfs);
        }
        public List<int> NumofBookinShelfs() 
        {
            return _shelfRepository.NumofBookinShelfs();
        }
        public bool Add(string shelfName)
        {
            return _shelfRepository.Add(shelfName);
        }
        public bool Update(Shelf shelf)
        {
            return _shelfRepository.Update(shelf);
        }
        public bool Delete(Shelf shelf)
        {
            return _shelfRepository.Delete(shelf);
        }

        public bool DeleteRelatedBooks(int shelfId)
        {
            return _shelfRepository.DeleteRelatedBooks(shelfId);
        }

        public bool Save()
        {
            return _shelfRepository.Save();
        }
    }
}


