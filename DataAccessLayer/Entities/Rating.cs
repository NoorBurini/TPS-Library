using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    public class Rating
    {
        public int Id { get; set; }
        public int RatingValue { get; set; }

        [ForeignKey("Book")]
        public int BookId { get; set; }
        [ForeignKey("AspNetUsers")]
        public string UserId { get; set; }

    }
}
