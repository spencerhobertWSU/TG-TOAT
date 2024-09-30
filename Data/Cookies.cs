using TGTOAT.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace TGTOAT.Data
{
    public class Cookies
    {
        [Key]

        public int UserId { get; set; }

        [Required, StringLength(100), DataType(DataType.EmailAddress)]
        public string Email {get; set; }

        [StringLength(100)]
        public string Series {get; set; }

        [StringLength(100)]
        public string Token { get; set; }
    }
}
