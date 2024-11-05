using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using Mono.TextTemplating;

namespace Data
{
    public class Addresses
    {
        [Key, ForeignKey("UserID"), Required]
        public int UserId { get; set; }

        public string? AddOne { get; set; }

        public string? AddTwo { get; set; }

        public string? City { get; set; }

        [StringLength(2)]
        public string? State { get; set; }

        [Range(00000, 99999)]
        public int? Zip { get; set; }

    }
}
