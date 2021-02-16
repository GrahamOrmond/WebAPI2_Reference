using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using WebAPI2_Reference.API.DAO;
using WebAPI2_Reference.API.DTO;
using System.Net;
using WebAPI2_Reference.API.Attributes;

namespace WebAPI2_Reference.API.Controllers
{
    [Authorize]
    [UnhandledExeption] // handles any internal server error exeptions
    public class BooksController : ApiController
    {
        private BookDAO _bookDAO;

        public BooksController()
        {
            BookDAO = new BookDAO();
        }

        public BookDAO BookDAO
        {
            get
            {
                return _bookDAO;
            }
            private set
            {
                _bookDAO = value;
            }
        }

        // GET: api/Books
        [HttpGet]
        [Route("")]
        public IQueryable<BookDTO> GetBooks()
        {
            return BookDAO.GetAllBooks();
        }

        // GET: api/Books/{id}
        [HttpGet]
        [Route("{id}", Name = "GetBookById")]
        [ResponseType(typeof(BookDetailsDTO))]
        public async Task<IHttpActionResult> GetBook(int Id)
        {
            BookDetailsDTO book = await BookDAO.GetBookAsync(Id);
            if (book == null)
                return NotFound();

            return Ok(book);
        }

        // POST: api/Books
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> PostBook(BookCreateDTO bookModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // create new author and return results
            BookDetailsDTO newBook = await BookDAO.AddBook(bookModel);
            return CreatedAtRoute("GetBookById", new { id = newBook.Id }, newBook);
        }


        // PUT: api/Books/{id}
        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> PutAuthor(int id, BookUpdateDTO bookUpdate)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // update book and get results
            var authorResults = await BookDAO.UpdateBookAsync(id, bookUpdate);
            if (authorResults == null) // author not found
                return NotFound();

            return Ok(authorResults);
        }

        // DELETE: api/Books/{id}
        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> DeleteAuthor(int id)
        {
            // delete author from the database
            bool results = await BookDAO.DeleteBookAsync(id);
            if (!results) // author was not found to delete
                return NotFound();

            return StatusCode(HttpStatusCode.NoContent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                BookDAO.Dispose();

            base.Dispose(disposing);
        }
    }
}