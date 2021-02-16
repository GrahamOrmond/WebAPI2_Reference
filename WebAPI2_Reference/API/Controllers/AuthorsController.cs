using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using WebAPI2_Reference.API.Attributes;
using WebAPI2_Reference.API.DAO;
using WebAPI2_Reference.API.DTO;

namespace WebAPI2_Reference.API.Controllers
{
    [Authorize]
    [UnhandledExeption] // handles any internal server error exeptions
    public class AuthorsController : ApiController
    {

        private AuthorDAO _authorDAO;

        public AuthorsController()
        {
            AuthorDAO = new AuthorDAO();
        }

        public AuthorDAO AuthorDAO
        {
            get
            {
                return _authorDAO;
            }
            private set
            {
                _authorDAO = value;
            }
        }

        // GET: api/Authors
        [HttpGet]
        [Route("")]
        public IQueryable<AuthorDetailsDTO> GetAuthors()
        {
            var authors = AuthorDAO.GetAllAuthors();
            return authors;
        }

        // GET: api/Authors/{id}
        [HttpGet]
        [Route("{id}", Name = "GetAuthorById")]
        [ResponseType(typeof(AuthorDetailsDTO))]
        public IHttpActionResult GetAuthor(int id)
        {
            AuthorDetailsDTO author = AuthorDAO.GetAuthor(id);
            if (author == null)
                return NotFound();

            return Ok(author);
        }

        // POST: api/Authors
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> PostAuthor(AuthorCreateDTO authorModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // create new author and return results
            AuthorDetailsDTO newAuthor = await AuthorDAO.AddAuthor(authorModel);
            return CreatedAtRoute("GetAuthorById", new { id = newAuthor.Id }, newAuthor);
        }


        // PUT: api/Authors/{id}
        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> PutAuthor(int id, AuthorUpdateDTO authorUpdate)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // update author and get results
            var authorResults = await AuthorDAO.UpdateAuthorAsync(id, authorUpdate);
            if(authorResults == null) // author not found
                return NotFound();

            return Ok(authorResults);
        }

        // DELETE: api/Authors/{id}
        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> DeleteAuthor(int id)
        {
            // delete author from the database
            bool results = await AuthorDAO.DeleteAuthorAsync(id);
            if (!results) // author was not found to delete
                return NotFound();

            return StatusCode(HttpStatusCode.NoContent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                AuthorDAO.Dispose();

            base.Dispose(disposing);
        }
    }
}