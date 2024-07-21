using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using DataAccessLayer.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private ApplicationDbContext context;

        public CategoryRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }
        public IQueryable<Category> Categories => context.Categories;

        public IQueryable<string> GetAllCategories()
        {
            return context.Categories.Select(c => c.CategoryName);
        }
        public string GetCategoryById(int categoryId)
        {
            Category c = context.Categories.SingleOrDefault(i => i.Id == categoryId);
            return c.CategoryName;
        }
        public Category GetCategoryById_(int categoryId)
        {
            return context.Categories.SingleOrDefault(i => i.Id == categoryId);
        }
        public int GetIdByName(string categoryName)
        {
            Category c = context.Categories.SingleOrDefault(i => i.CategoryName == categoryName);
            return c.Id;
        }
        public bool CheckCategoryName(string categoryName)
        {
            Category category = context.Categories.SingleOrDefault(c => c.CategoryName == categoryName);
            if (category == null) return false;
            else return true;
        }
        public IEnumerable<int> GetCategoriesID(IEnumerable<Category> categories) 
        {
            return categories.Select(s => s.Id).ToList();
        }
        public IEnumerable<Category> GetCategoryByName(string categoryName)
        {
            return context.Categories.Where(s => s.CategoryName.Contains(categoryName)).ToList();

        }
        public bool Add( string categoryName)
        {
            context.Add(new Category {  CategoryName = categoryName });
            return Save();
        }


        public bool Delete(Category category)
        {

            context.Remove(category);
            return Save();
        }

        public bool DeleteRelatedBooks(int categoryId)
        {
            IEnumerable<Book> books = context.Books.Where(x => x.CategoryId == categoryId);
            context.RemoveRange(books);
            return Save();
        }

        public bool Update(Category category)
        {
            context.Update(category);
            return Save();
        }

        public bool Save()
        {
            var saved = context.SaveChanges();
            return saved > 0;
        }
      
    }
}
