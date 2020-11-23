using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using WebAPI2_Reference.API.DAO;
using WebAPI2_Reference.API.DTO;
using WebAPI2_Reference.Data_Models;
using WebAPI2_Reference.Models;

namespace WebAPI2_Reference.API.Controllers
{
    [Authorize]
    public class AuthorsController : ApiController
    {

        private AuthorDAO _authorDAO = new AuthorDAO();

        // GET: api/Authors
        public IQueryable<AuthorDTO> GetAuthors()
        {
            var authors = _authorDAO.GetAllAuthors();
            return authors;
        }

        // GET: api/Authors/5
        [ResponseType(typeof(AuthorDTO))]
        public IHttpActionResult GetAuthor(int id)
        {
            AuthorDTO author = _authorDAO.GetAuthor(id);
            if (author == null)
                return NotFound();

            return Ok(author);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _authorDAO.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}