using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebAPI2_Reference.API.DTO
{
    public class BookResultsDTO
    {
        public IEnumerable<BookDTO> Books { get; set; }
    }

    public class BookDTO
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string AuthorName { get; set; }
    }

    public class BookDetailsDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public decimal Price { get; set; }
        public string AuthorName { get; set; }
        public string Genre { get; set; }
    }


    public class BookCreateDTO
    {
        [Required]
        public string Title { get; set; }
        public int Year { get; set; }
        public decimal Price { get; set; }
        public string Genre { get; set; }

        // Author Reference
        [Required]
        public int AuthorId { get; set; } // Foreign Key
    }

    public class BookUpdateDTO
    {
        public string Title { get; set; }
        public int Year { get; set; }
        public decimal Price { get; set; }
        public string Genre { get; set; }

        public int AuthorId { get; set; } // Foreign Key
    }
}
