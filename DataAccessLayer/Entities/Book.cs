using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Entities
{
    public class Book
    {
        public int Id { get; set; }
        public string BookName { get; set; }

        [ForeignKey("Shelf")]
        public int ShelfId { get; set; }
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public float RatingValue { get; set; }
    }
}
