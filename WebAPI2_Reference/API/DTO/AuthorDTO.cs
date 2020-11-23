using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI2_Reference.API.DTO
{
    public class AuthorDetailsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class AuthorUpdateDTO
    {
        public string Name { get; set; }
    }

    public class AuthorCreateDTO
    {
        public string Name { get; set; }
    }
}