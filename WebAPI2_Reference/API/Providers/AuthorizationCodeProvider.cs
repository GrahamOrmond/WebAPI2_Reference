using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WebAPI2_Reference.API.Providers
{
    public class AuthorizationCodeProvider : IAuthenticationTokenProvider
    {
        private readonly ConcurrentDictionary<string, string> _authenticationCodes =
        new ConcurrentDictionary<string, string>(StringComparer.Ordinal);

        /*
         *  generate and set code
         */
        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            // create code grant
            const int strengthInBits = 512;
            var codeGrant = RandomOAuthGenerator.Generate(strengthInBits);
            context.SetToken(codeGrant); // set the context token
            string hashedCode = ComputeHash(codeGrant); // hash the token
            _authenticationCodes.TryAdd(hashedCode, context.SerializeTicket());
        }

        /*
         *  THIS FUNCTION IS NOT WORKING
         *  
         *  from what ive read from documentation is that GrantAuthorizationCode inside of OAuthAuthorizationServerProvider
         *  is suppose to run before this function where you can validate the ticket and what not. but it doesnt run
         *  for some reason this method runs first and returns response 400 invaid request no matter what i try.
         *  
         *  It would be cool if there was proper documentation on this eh.. Ive spend way to much time on this,
         *  time to move on. i have no idea what im doing
         */
        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            string hashedCode = ComputeHash(context.Token);

            string serializedTicket = null;
            _authenticationCodes.TryRemove(hashedCode, out serializedTicket);
            context.DeserializeTicket(serializedTicket);
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
         *  Used for hashing the token if it was to be saved
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
         *  Used for generating code
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