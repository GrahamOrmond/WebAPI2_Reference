using System;
using System.Linq;
using WebAPI2_Reference.API.DTO;
using WebAPI2_Reference.Models;

namespace WebAPI2_Reference.API.DAO
{
    public class AuthorDAO
    {
        private ApplicationDbContext _db = new ApplicationDbContext();
        public AuthorDAO()
        {

        }

        public IQueryable<AuthorDTO> GetAllAuthors()
        {
            throw new NotImplementedException();
        }

        public AuthorDTO GetAuthor(int id)
        {
            throw new NotImplementedException();
        }

        internal void Dispose()
        {
            _db.Dispose();
        }
    }
}