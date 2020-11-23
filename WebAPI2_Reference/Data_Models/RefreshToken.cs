using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebAPI2_Reference.Models;

namespace WebAPI2_Reference.Data_Models
{
    public class RefreshToken
    {
        [Key]
        public string Token { get; set; }
        public DateTime IssuedUtc { get; set; }
        public DateTime ExpiresUtc { get; set; }
        [Required]
        public string ProtectedTicket { get; set; }
        public ApplicationUser User { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }

    }

}