using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using WebAPI2_Reference.API.DTO;
using WebAPI2_Reference.API.Filters;
using WebAPI2_Reference.Data_Models;
using WebAPI2_Reference.Models;

namespace WebAPI2_Reference.API.Controllers
{
    [Authorize]
    public class BooksController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}