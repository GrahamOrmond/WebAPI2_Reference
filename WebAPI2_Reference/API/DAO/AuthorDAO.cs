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

        public async Task<AuthorDetailsDTO> AddAuthor(AuthorCreateDTO authorModel)
        {
            // create new author
            Author newAuthor = new Author()
            {
                Name = authorModel.Name
            };

            // add author to database
            _db.Authors.Add(newAuthor);
            await _db.SaveChangesAsync();
            return new AuthorDetailsDTO()
            {
                Id = newAuthor.Id,
                Name = newAuthor.Name,
            };
        }

        public async Task<AuthorDetailsDTO> UpdateAuthorAsync(int id, AuthorUpdateDTO authorUpdate)
        {
            // get the author from the database
            var author = _db.Authors.SingleOrDefault(a => a.Id == id);
            if (author == null) // author not found
                return null;

            // update the author
            author.Name = authorUpdate.Name;
            _db.Entry(author).State = EntityState.Modified;

            try // try to save changes
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) // failed to save changes
            {
                throw;
            }

            // return new instance of author details
            return new AuthorDetailsDTO()
            {
                Id = author.Id,
                Name = author.Name,
            };
        }

        public async Task<bool> DeleteAuthorAsync(int id)
        {
            // get the author from the database
            Author author = await _db.Authors.FindAsync(id);
            if (author == null) // author not found
                return false;

            // delete the author from the database
            _db.Authors.Remove(author);

            return await _db.SaveChangesAsync() > 0; // return delete results
        }

        private bool AuthorExists(int id)
        {
            return _db.Authors.Count(e => e.Id == id) > 0;
        }

        internal void Dispose()
        {
            _db.Dispose();
        }

    }
}
