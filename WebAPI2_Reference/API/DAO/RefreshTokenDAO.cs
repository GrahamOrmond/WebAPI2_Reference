using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WebAPI2_Reference.Data_Models;
using WebAPI2_Reference.Models;

namespace WebAPI2_Reference.API.DAO
{
    public static class RefreshTokenDAO
    {
        public static async Task<bool> AddRefreshToken(RefreshToken token)
        {
            using (var db = new ApplicationDbContext())
            {
                var existingToken = db.RefreshTokens
                .Where(r => r.UserId == token.UserId)
                .SingleOrDefault();

                if (existingToken != null)
                    db.RefreshTokens.Remove(existingToken);

                db.RefreshTokens.Add(token);
                return await db.SaveChangesAsync() > 0;
            }
        }

        public static async Task<RefreshToken> FindRefreshToken(string hashedToken)
        {
            using (var db = new ApplicationDbContext())
            {
                return await db.RefreshTokens.FindAsync(hashedToken);
            }
        }

        public static async Task<bool> RemoveRefreshToken(string hashedToken)
        {
            using (var db = new ApplicationDbContext())
            {
                var refreshTokenModel = await db.RefreshTokens.FindAsync(hashedToken);
                if (refreshTokenModel != null)
                {
                    db.RefreshTokens.Remove(refreshTokenModel);
                    return await db.SaveChangesAsync() > 0;
                }
            }
            return false;
        }
    }
}