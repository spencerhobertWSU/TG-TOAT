using TGTOAT.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace TGTOAT.Data
{
    public class Address
    {
        [Key]
        [Required]
        public int UserId { get; set; }

        [StringLength(255, MinimumLength = 2)]
        public string AddressLineOne { get; set; }

        [StringLength(255, MinimumLength = 2)]
        public string AddressLineTwo { get; set; }

        [StringLength(30, MinimumLength = 2)]
        public string ZipCode { get; set; }

        public User User { get; set; } // Navigation property back to User
    }
}
