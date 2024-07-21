using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLogicLayer.Services
{
    public interface IRatingService
    {
        bool CheckUserId(string userId, int bookId);
        List<int> BookRatings(int bookId);
        bool Add(int bookId, int ratingValue, string userId);
        bool Save();
    }
}
