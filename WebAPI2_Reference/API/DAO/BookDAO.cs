using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using WebAPI2_Reference.API.DTO;
using WebAPI2_Reference.Models;
using System;
using WebAPI2_Reference.Data_Models;
using System.Data.Entity.Infrastructure;

namespace WebAPI2_Reference.API.DAO
{
    public class BookDAO
    {
        private ApplicationDbContext _db = new ApplicationDbContext();
        public BookDAO()
        {

        }

        public IQueryable<BookDTO> GetAllBooks()
        {
            return from b in _db.Books
                   select new BookDTO()
                   {
                       Id = b.Id,
                       Title = b.Title,
                       AuthorName = b.Author.Name,
                   };
        }

        public async Task<BookDetailsDTO> GetBookAsync(int id)
        {
            return await _db.Books.Select(b =>
                new BookDetailsDTO()
                {
                    Id = b.Id,
                    Title = b.Title,
                    Year = b.Year,
                    Price = b.Price,
                    AuthorName = b.Author.Name,
                    Genre = b.Genre
                }).SingleOrDefaultAsync(b => b.Id == id);

        }

        public async Task<BookDetailsDTO> AddBook(BookCreateDTO bookModel)
        {
            throw new NotImplementedException();
        }

        public async Task<BookDetailsDTO> UpdateBookAsync(int id, BookUpdateDTO bookUpdate)
        {
            throw new NotImplementedException();
        }

        public async Task<BookDetailsDTO> DeleteBookAsync(int id)
        {
            throw new NotImplementedException();
        }

        internal void Dispose()
        {
            _db.Dispose();
        }


    }
}
