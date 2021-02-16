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
            // create new book to save
            Book newBook = new Book()
            {
                Title = bookModel.Title,
                Genre = bookModel.Genre,
                Year = bookModel.Year,
                Price = bookModel.Price,
                AuthorId = bookModel.AuthorId,
            };

            // save the book to the database
            _db.Books.Add(newBook);
            await _db.SaveChangesAsync();

            // load author info and return book details
            _db.Entry(newBook).Reference(x => x.Author).Load();
            return new BookDetailsDTO()
            {
                Id = newBook.Id,
                Genre = newBook.Genre,
                Title = newBook.Title,
                Year = newBook.Year,
                Price = newBook.Price,
                AuthorName = newBook.Author.Name,
            };
        }

        public async Task<BookDetailsDTO> UpdateBookAsync(int id, BookUpdateDTO bookUpdate)
        {
            var book = _db.Books.SingleOrDefault(a => a.Id == id);
            if (book == null) // author not found
                return null;

            // update the author
            book.Title = bookUpdate.Title;
            book.Price = bookUpdate.Price;
            book.Year = bookUpdate.Year;
            book.Genre = bookUpdate.Genre;
            book.AuthorId = bookUpdate.AuthorId;
            _db.Entry(book).State = EntityState.Modified;

            try // try to save changes
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) // failed to save changes
            {
                throw;
            }

            // load author info and return book details
            _db.Entry(book).Reference(x => x.Author).Load();
            // return new instance of author details
            return new BookDetailsDTO()
            {
                Id = book.Id,
                Genre = book.Genre,
                Title = book.Title,
                Year = book.Year,
                Price = book.Price,
                AuthorName = book.Author.Name,
            };
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            Book book = await _db.Books.FindAsync(id);
            if (book == null) // author not found
                return false;

            // delete the author from the database
            _db.Books.Remove(book);

            return await _db.SaveChangesAsync() > 0; // return delete results
        }

        private bool BookExists(int id)
        {
            return _db.Books.Count(e => e.Id == id) > 0;
        }

        internal void Dispose()
        {
            _db.Dispose();
        }


    }
}
