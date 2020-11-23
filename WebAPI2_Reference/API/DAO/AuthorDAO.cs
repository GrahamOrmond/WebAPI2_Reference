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
            return from a in _db.Authors
                   select new AuthorDTO()
                   {
                       Id = a.Id,
                       Name = a.Name,
                   };
        }

        public AuthorDTO GetAuthor(int id)
        {
            return _db.Authors
                .Select(a =>
                new AuthorDTO()
                {
                    Id = a.Id,
                    Name = a.Name,
                }).SingleOrDefault(a => a.Id == id);
        }

        internal void Dispose()
        {
            _db.Dispose();
        }
    }
}