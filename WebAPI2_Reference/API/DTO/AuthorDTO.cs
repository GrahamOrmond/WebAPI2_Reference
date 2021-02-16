
namespace WebAPI2_Reference.API.Dto
{
    public class AuthorDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class AuthorUpdateDto
    {
        public string Name { get; set; }
    }

    public class AuthorCreateDto
    {
        public string Name { get; set; }
    }
}
