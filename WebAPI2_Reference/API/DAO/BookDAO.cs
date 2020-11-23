using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using WebAPI2_Reference.API.DTO;
using WebAPI2_Reference.Models;
using System;

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
            throw new NotImplementedException();
        }

        public BookDetailDTO GetBook(int id)
        {
            throw new NotImplementedException();
        }

        internal void Dispose()
        {
            _db.Dispose();
        }
    }
}