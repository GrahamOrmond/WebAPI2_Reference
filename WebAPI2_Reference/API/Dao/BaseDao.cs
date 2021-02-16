using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebAPI2_Reference.Models;

namespace WebAPI2_Reference.API.Dao
{
    public class BaseDao
    {
        private ApplicationDbContext _db;

        public BaseDao()
        {
            DbContext = new ApplicationDbContext();
        }

        public ApplicationDbContext DbContext
        {
            get
            {
                return _db;
            }
            private set
            {
                _db = value;
            }
        }

        // throw api error manually
        protected void ThrowResponseException(HttpStatusCode statusCode, string message)
        {
            throw new HttpResponseException(new HttpResponseMessage(statusCode)
            {
                ReasonPhrase = message
            });
        }

        public void Dispose()
        {
            _db.Dispose();
            _db = null;
        }
    }
}