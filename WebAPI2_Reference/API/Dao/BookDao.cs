using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using WebAPI2_Reference.API.Dto;
using WebAPI2_Reference.Data_Models;
using System.Data.Entity.Infrastructure;
using System.Net;

namespace WebAPI2_Reference.API.Dao
{
    public class BookDao : BaseDao
    {

        public IQueryable<BookDto> GetAllBooks()
        {
            return from b in DbContext.Books
                   select new BookDto()
                   {
                       Id = b.Id,
                       Title = b.Title,
                       AuthorName = b.Author.Name,
                   };
        }

        public async Task<BookDetailsDto> GetBookAsync(int id)
        {
            return await DbContext.Books.Select(b =>
                new BookDetailsDto()
                {
                    Id = b.Id,
                    Title = b.Title,
                    Year = b.Year,
                    Price = b.Price,
                    AuthorName = b.Author.Name,
                    Genre = b.Genre
                }).SingleOrDefaultAsync(b => b.Id == id);

        }

        public async Task<BookDetailsDto> AddBook(BookCreateDto bookModel)
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
            DbContext.Books.Add(newBook);
            await DbContext.SaveChangesAsync();

            // load author info and return book details
            DbContext.Entry(newBook).Reference(x => x.Author).Load();
            return new BookDetailsDto()
            {
                Id = newBook.Id,
                Genre = newBook.Genre,
                Title = newBook.Title,
                Year = newBook.Year,
                Price = newBook.Price,
                AuthorName = newBook.Author.Name,
            };
        }

        public async Task<BookDetailsDto> UpdateBookAsync(int id, BookUpdateDto bookUpdate)
        {
            var book = DbContext.Books.SingleOrDefault(a => a.Id == id);
            if (book == null) // author not found
                // throw manual api error with custom message
                ThrowResponseException(HttpStatusCode.NotFound, "Author Not Found");

            // update the author
            book.Title = bookUpdate.Title;
            book.Price = bookUpdate.Price;
            book.Year = bookUpdate.Year;
            book.Genre = bookUpdate.Genre;
            book.AuthorId = bookUpdate.AuthorId;
            DbContext.Entry(book).State = EntityState.Modified;

            try // try to save changes
            {
                await DbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) // failed to save changes
            {
                throw;
            }

            // load author info and return book details
            DbContext.Entry(book).Reference(x => x.Author).Load();
            // return new instance of author details
            return new BookDetailsDto()
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
            Book book = await DbContext.Books.FindAsync(id);
            if (book == null) // author not found
                // throw manual api error with custom message
                ThrowResponseException(HttpStatusCode.NotFound, "Author Not Found");

            // delete the author from the database
            DbContext.Books.Remove(book);

            return await DbContext.SaveChangesAsync() > 0; // return delete results
        }

        private bool BookExists(int id)
        {
            return DbContext.Books.Count(e => e.Id == id) > 0;
        }
    }
}
