using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebAPI2_Reference.API.Dto
{
    public class BookResultsDto
    {
        public IEnumerable<BookDto> Books { get; set; }
    }

    public class BookDto
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string AuthorName { get; set; }
    }

    public class BookDetailsDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public decimal Price { get; set; }
        public string AuthorName { get; set; }
        public string Genre { get; set; }
        public int AuthorId { get; set; } // Foreign Key
    }


    public class BookCreateDto
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

    public class BookUpdateDto
    {
        public string Title { get; set; }
        public int Year { get; set; }
        public decimal Price { get; set; }
        public string Genre { get; set; }

        public int AuthorId { get; set; } // Foreign Key
    }
}
