using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using DataAccessLayer.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class RatingRepository: IRatingRepository
    {
        private ApplicationDbContext context;

        public RatingRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }
        public bool CheckUserId(string userId, int bookId)
        {
            Rating rating = context.Ratings.SingleOrDefault(b => b.UserId == userId && b.BookId == bookId);
            if (rating == null) return false;
            else return true;
           
        }
        public List<int> BookRatings(int bookId)
        {
            List<int> ratings = context.Ratings.Where(r => r.BookId==bookId).Select(b => b.RatingValue).ToList();
            return ratings;
        }
        public bool Add(int bookId, int ratingValue, string userId)
        {
            context.Add(new Rating { BookId = bookId, RatingValue = ratingValue, UserId = userId });
            return Save();
            
        }
        public bool Save()
        {
            var saved = context.SaveChanges();
            return saved > 0;
        }
    }
}
