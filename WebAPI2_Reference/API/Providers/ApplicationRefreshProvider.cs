using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebAPI2_Reference.API.DAO;
using WebAPI2_Reference.Data_Models;

namespace WebAPI2_Reference.API.Providers
{
    public class ApplicationRefreshProvider : IAuthenticationTokenProvider
    {
        // get private token settings
        private static int _refreshTokenExpireMinutes = Convert.ToInt32(ConfigurationManager.AppSettings["Token:RefreshTokenExpireMinutes"]);

        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            // create refresh token
            const int strengthInBits = 512;
            var refreshToken = RandomOAuthGenerator.Generate(strengthInBits);
            var hashedToken = ComputeHash(refreshToken); // hash the token

            //copy properties and set the desired lifetime of refresh token
            var userId = context.Ticket.Properties.Dictionary["userId"];
            var token = new RefreshToken()
            {
                UserId = userId,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(_refreshTokenExpireMinutes),
                IssuedUtc = context.Ticket.Properties.IssuedUtc.GetValueOrDefault().UtcDateTime,
                Token = hashedToken, // hashed token for database
            };
            token.ProtectedTicket = context.SerializeTicket();

            // set context properties
            context.Ticket.Properties.IssuedUtc = token.IssuedUtc;
            context.Ticket.Properties.ExpiresUtc = token.ExpiresUtc;

            // save the refresh token to the database
            var results = await RefreshTokenDAO.AddRefreshToken(token);
            if (results) 
                context.SetToken(refreshToken); // set the context token
        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            // get the token from the database
            var hashedToken = ComputeHash(context.Token); // hash the context token
            var refreshToken = await RefreshTokenDAO
                .FindRefreshToken(hashedToken); // search by hash
            if (refreshToken != null) // token found
            {
                context.DeserializeTicket(refreshToken.ProtectedTicket); // set the ticket
                await RefreshTokenDAO.RemoveRefreshToken(hashedToken); // remove old refresh token
            }
        }

        public void Create(AuthenticationTokenCreateContext context)
        {
            throw new NotImplementedException();
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            throw new NotImplementedException();
        }

        /*
         *  Hash a given string
         *  Used for hashing the refresh token in the database
         */
        private string ComputeHash(string input)
        {
            byte[] source = Encoding.ASCII.GetBytes(input);
            var encoder = new SHA256Managed();
            byte[] encoded = encoder.ComputeHash(source);

            return Convert.ToBase64String(encoded);
        }

        /*
         *  Generate a random string
         *  Used for generating refresh token
         */
        private static class RandomOAuthGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }
    }
}