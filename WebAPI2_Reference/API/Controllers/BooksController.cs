using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using WebAPI2_Reference.API.DAO;
using WebAPI2_Reference.API.DTO;
using WebAPI2_Reference.Models;using System.Data.Entity;
using System.Net;

namespace WebAPI2_Reference.API.Controllers
{
    [Authorize]
    public class BooksController : ApiController
    {
        private BookDAO _bookDAO = new BookDAO();

        // GET: api/Authors
        public IQueryable<BookDTO> GetBooks()
        {
            return _bookDAO.GetAllBooks();
        }

        // GET: api/Books/5
        [ResponseType(typeof(BookDetailsDTO))]
        public async Task<IHttpActionResult> GetBook(int Id)
        {
            BookDetailsDTO book = await _bookDAO.GetBookAsync(Id);
            if (book == null)
                return NotFound();

            return Ok(book);
        }

        // POST: api/Authors
        [ResponseType(typeof(BookDetailsDTO))]
        public async Task<IHttpActionResult> PostBook(BookCreateDTO bookModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // create new author and return results
            BookDetailsDTO newBook = await _bookDAO.AddBook(bookModel);
            return CreatedAtRoute("DefaultApi", new { id = newBook.Id }, newBook);
        }


        // PUT: api/Authors/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutAuthor(int id, BookUpdateDTO bookUpdate)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // update author and get results
            var authorResults = await _bookDAO.UpdateBookAsync(id, bookUpdate);
            if (authorResults == null) // author not found
                return NotFound();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE: api/Authors/5
        [ResponseType(typeof(BookDetailsDTO))]
        public async Task<IHttpActionResult> DeleteAuthor(int id)
        {
            // delete author from the database
            var author = await _bookDAO.DeleteBookAsync(id);
            if (author == null) // author was not found to delete
                return NotFound();

            return Ok(author);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _bookDAO.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}