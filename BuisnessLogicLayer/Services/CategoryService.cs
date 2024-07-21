using DataAccessLayer.Entities;
using DataAccessLayer.Migrations;
using DataAccessLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLogicLayer.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository rep)
        {
            _categoryRepository = rep;
        }
        public IQueryable<Category> Categories => _categoryRepository.Categories;

        public IQueryable<string> GetAllCategories()
        {
            return _categoryRepository.GetAllCategories();
        }

        public string GetCategoryById(int categoryId)
        {
            return _categoryRepository.GetCategoryById(categoryId);
        }
        public int GetIdByName(string categoryName)
        { 
            return _categoryRepository.GetIdByName(categoryName);
        }
        public Category GetCategoryById_(int categoryId)
        {
            return _categoryRepository.GetCategoryById_(categoryId);
        }
        public bool CheckCategoryName(string categoryName)
        {
            return _categoryRepository.CheckCategoryName(categoryName);
        }
        public IEnumerable<int> GetCategoriesID(IEnumerable<Category> categories) 
        {
            return _categoryRepository.GetCategoriesID(categories);
        }
        public IEnumerable<Category> GetCategoryByName(string categoryName)
        {
            return _categoryRepository.GetCategoryByName(categoryName);
        }
        public bool Add( string categoryName)
        {
            return _categoryRepository.Add( categoryName);
        }


        public bool Delete(Category category)
        {
            return _categoryRepository.Delete(category);
        }

        public bool DeleteRelatedBooks(int categoryId)
        {
            return _categoryRepository.DeleteRelatedBooks(categoryId);
        }
        public bool Update(Category category)
        {
            return _categoryRepository.Update(category);
        }

        public bool Save()
        {
            return _categoryRepository.Save();
        }

    }
}
