using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WebAPI2_Reference.API.Dto;
using WebAPI2_Reference.Data_Models;

namespace WebAPI2_Reference.API.Dao
{
    public class AuthorDao : BaseDao
    {

        public IQueryable<AuthorDetailsDto> GetAllAuthors()
        {
            return from a in DbContext.Authors
                   select new AuthorDetailsDto()
                   {
                       Id = a.Id,
                       Name = a.Name,
                   };
        }

        public AuthorDetailsDto GetAuthor(int id)
        {
            return DbContext.Authors
                .Select(a =>
                new AuthorDetailsDto()
                {
                    Id = a.Id,
                    Name = a.Name,
                }).SingleOrDefault(a => a.Id == id);
        }

        public async Task<AuthorDetailsDto> AddAuthor(AuthorCreateDto authorModel)
        {
            // create new author
            Author newAuthor = new Author()
            {
                Name = authorModel.Name
            };

            // add author to database
            DbContext.Authors.Add(newAuthor);
            await DbContext.SaveChangesAsync();
            return new AuthorDetailsDto()
            {
                Id = newAuthor.Id,
                Name = newAuthor.Name,
            };
        }

        public async Task<AuthorDetailsDto> UpdateAuthorAsync(int id, AuthorUpdateDto authorUpdate)
        {
            // get the author from the database
            var author = DbContext.Authors.SingleOrDefault(a => a.Id == id);
            if (author == null) // author not found
                // throw manual api error with custom message
                ThrowResponseException(HttpStatusCode.NotFound, "Author Not Found"); 

            // update the author
            author.Name = authorUpdate.Name;
            DbContext.Entry(author).State = EntityState.Modified;

            try // try to save changes
            {
                await DbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) // failed to save changes
            {
                throw;
            }

            // return new instance of author details
            return new AuthorDetailsDto()
            {
                Id = author.Id,
                Name = author.Name,
            };
        }

        public async Task<bool> DeleteAuthorAsync(int id)
        {
            // get the author from the database
            Author author = await DbContext.Authors.FindAsync(id);
            if (author == null) // author not found
                // throw manual api error with custom message
                ThrowResponseException(HttpStatusCode.NotFound, "Author Not Found");

            // delete the author from the database
            DbContext.Authors.Remove(author);

            return await DbContext.SaveChangesAsync() > 0; // return delete results
        }

        private bool AuthorExists(int id)
        {
            return DbContext.Authors.Count(e => e.Id == id) > 0;
        }
    }
}
