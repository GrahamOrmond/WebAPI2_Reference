using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using WebAPI2_Reference.API.Attributes;
using WebAPI2_Reference.API.Dao;
using WebAPI2_Reference.API.Dto;

namespace WebAPI2_Reference.API.Controllers
{
    [Authorize]
    [RoutePrefix("api/Authors")]
    [UnhandledExeption] // handles any internal server error exeptions
    public class AuthorsController : ApiController
    {

        private AuthorDao _authorDao;

        public AuthorsController()
        {
            AuthorDao = new AuthorDao();
        }

        public AuthorDao AuthorDao
        {
            get
            {
                return _authorDao;
            }
            private set
            {
                _authorDao = value;
            }
        }

        // GET: api/Authors
        [HttpGet]
        [Route("")]
        public IQueryable<AuthorDetailsDto> GetAuthors()
        {
            var authors = AuthorDao.GetAllAuthors();
            return authors;
        }

        // GET: api/Authors/{id}
        [HttpGet]
        [Route("{id}", Name = "GetAuthorById")]
        [ResponseType(typeof(AuthorDetailsDto))]
        public IHttpActionResult GetAuthor(int id)
        {
            AuthorDetailsDto author = AuthorDao.GetAuthor(id);
            if (author == null)
                return NotFound();

            return Ok(author);
        }

        // POST: api/Authors
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> PostAuthor(AuthorCreateDto authorModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // create new author and return results
            AuthorDetailsDto newAuthor = await AuthorDao.AddAuthor(authorModel);
            return CreatedAtRoute("GetAuthorById", new { id = newAuthor.Id }, newAuthor);
        }


        // PUT: api/Authors/{id}
        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> PutAuthor(int id, AuthorUpdateDto authorUpdate)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // update author and get results
            var authorResults = await AuthorDao.UpdateAuthorAsync(id, authorUpdate);
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
            bool results = await AuthorDao.DeleteAuthorAsync(id);
            if (!results) // author was not found to delete
                return NotFound();

            return StatusCode(HttpStatusCode.NoContent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                AuthorDao.Dispose();

            base.Dispose(disposing);
        }
    }
}