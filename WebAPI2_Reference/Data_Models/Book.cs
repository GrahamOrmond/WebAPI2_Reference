using System.ComponentModel.DataAnnotations;

namespace WebAPI2_Reference.Data_Models
{
    public class Book
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public int Year { get; set; }
        public decimal Price { get; set; }
        public string Genre { get; set; }

        // Author Reference
        public int AuthorId { get; set; } // Foreign Key
        public Author Author { get; set; } // Navigation property
    }
}