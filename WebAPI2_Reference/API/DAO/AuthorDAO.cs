using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using WebAPI2_Reference.API.DTO;
using WebAPI2_Reference.Data_Models;
using WebAPI2_Reference.Models;

namespace WebAPI2_Reference.API.DAO
{
    public class AuthorDAO
    {
        private ApplicationDbContext _db = new ApplicationDbContext();
        public AuthorDAO()
        {

        }

        public IQueryable<AuthorDetailsDTO> GetAllAuthors()
        {
            return from a in _db.Authors
                   select new AuthorDetailsDTO()
                   {
                       Id = a.Id,
                       Name = a.Name,
                   };
        }

        public AuthorDetailsDTO GetAuthor(int id)
        {
            return _db.Authors
                .Select(a =>
                new AuthorDetailsDTO()
                {
                    Id = a.Id,
                    Name = a.Name,
                }).SingleOrDefault(a => a.Id == id);
        }
        
        internal async Task<AuthorDetailsDTO> AddAuthor(AuthorCreateDTO authorModel)
        {
            throw new NotImplementedException();
        }

        public async Task<AuthorDetailsDTO> UpdateAuthorAsync(int id, AuthorUpdateDTO authorUpdate)
        {
            throw new NotImplementedException();
        }

        public async Task<AuthorDetailsDTO> DeleteAuthorAsync(int id)
        {
            throw new NotImplementedException();
        }

        internal void Dispose()
        {
            _db.Dispose();
        }

    }
}
