using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using WebAPI2_Reference.API.DAO;
using WebAPI2_Reference.API.DTO;
using WebAPI2_Reference.Models;using System.Data.Entity;


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
        [ResponseType(typeof(BookDetailDTO))]
        public async Task<IHttpActionResult> GetBook(int Id)
        {
            BookDetailDTO book = await _bookDAO.GetBook(Id);
            if (book == null)
                return NotFound();

            return Ok(book);
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