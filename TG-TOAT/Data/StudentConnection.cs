using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using Mono.TextTemplating;
using System.Diagnostics.CodeAnalysis;

namespace Data
{
    public class StudentConnection
    {
        [Key, ForeignKey("UserId"), Required]
        public int StudentId { get; set; }

        [ForeignKey("CourseId"), Required]
        public int CourseId { get; set; }

        [AllowNull]
        [Column(TypeName = "decimal(5, 2)")] // RAAAAH
        public decimal? Grade { get; set; }
    }
}
