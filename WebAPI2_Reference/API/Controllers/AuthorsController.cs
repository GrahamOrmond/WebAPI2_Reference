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
        public IQueryable<AuthorDetailsDTO> GetAuthors()
        {
            var authors = _authorDAO.GetAllAuthors();
            return authors;
        }

        // GET: api/Authors/5
        [ResponseType(typeof(AuthorDetailsDTO))]
        public IHttpActionResult GetAuthor(int id)
        {
            AuthorDetailsDTO author = _authorDAO.GetAuthor(id);
            if (author == null)
                return NotFound();

            return Ok(author);
        }

        // POST: api/Authors
        [ResponseType(typeof(Author))]
        public async Task<IHttpActionResult> PostAuthor(AuthorCreateDTO authorModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // create new author and return results
            AuthorDetailsDTO newAuthor = await _authorDAO.AddAuthor(authorModel);
            return CreatedAtRoute("DefaultApi", new { id = newAuthor.Id }, newAuthor);
        }


        // PUT: api/Authors/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutAuthor(int id, AuthorUpdateDTO authorUpdate)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // update author and get results
            var authorResults = await _authorDAO.UpdateAuthorAsync(id, authorUpdate);
            if(authorResults == null) // author not found
                return NotFound();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE: api/Authors/5
        [ResponseType(typeof(AuthorDetailsDTO))]
        public async Task<IHttpActionResult> DeleteAuthor(int id)
        {
            // delete author from the database
            var author = await _authorDAO.DeleteAuthorAsync(id);
            if (author == null) // author was not found to delete
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