using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public interface ICategoryRepository
    {
        IQueryable<Category> Categories { get; }
        IQueryable<string> GetAllCategories();
        string GetCategoryById(int categoryId);
        Category GetCategoryById_(int categoryId);
        int GetIdByName(string categoryName);
        bool CheckCategoryName(string categoryName);

        IEnumerable<int> GetCategoriesID(IEnumerable<Category> categories);
        IEnumerable<Category> GetCategoryByName(string categoryName);
        bool Add( string categoryName);


        bool Delete(Category category);

        bool DeleteRelatedBooks(int categoryId);
        bool Update(Category category);

        bool Save();
    }
}
