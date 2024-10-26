using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using TGTOAT.Models;

namespace TGTOAT.Data
{
    public class StudentCourseConnection
    {
        [Key]
        public int StudentCourseId { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        public int StudentID { get; set; }

        [ForeignKey("StudentID")]
        public User? User { get; set; }

        [Required(ErrorMessage = "Course ID is required.")]
        public int CourseId { get; set; }

        [ForeignKey("CourseId")]
        public Courses? Course { get; set; }

        [AllowNull]
        [Column(TypeName = "decimal(5, 2)")] // RAAAAH
        public decimal? Grade { get; set; }
    }
}
