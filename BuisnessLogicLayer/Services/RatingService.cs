using DataAccessLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLogicLayer.Services
{
    public class RatingService :IRatingService
    {
        private readonly IRatingRepository _ratingRepository;

        public RatingService(IRatingRepository rep)
        {
            _ratingRepository = rep;
        }
        public bool CheckUserId(string userId, int bookId)
        {
            return _ratingRepository.CheckUserId(userId, bookId);
        }
        public List<int> BookRatings(int bookId) 
        {
            return _ratingRepository.BookRatings(bookId);
        }
        public bool Add(int bookId, int ratingValue, string userId)
        {
            return _ratingRepository.Add(bookId, ratingValue, userId);
        }
        public bool Save()
        {
            return _ratingRepository.Save();
        }
    }
}
