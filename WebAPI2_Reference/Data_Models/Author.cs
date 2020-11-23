using System.ComponentModel.DataAnnotations;

namespace WebAPI2_Reference.Data_Models
{
    public class Author
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}