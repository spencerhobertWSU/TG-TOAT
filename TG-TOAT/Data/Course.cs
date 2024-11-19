using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using Mono.TextTemplating;

namespace Data
{
    public class Courses
    {
        [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CourseId { get; set; }

        [ForeignKey("DeptId"), Required]
        public int DeptId { get; set; }

        [Required, Range(0000, 9999)]
        public int CourseNum { get; set; }

        [Required]
        public string CourseName { get; set; }

        [Required]
        public string CourseDesc { get; set; }

        [Required, Range(1, 99)]
        public int Credits { get; set; }

        [Required, Range(1, 99)]
        public int Capacity { get; set; }

        public string? Campus { get; set; }

        public string? Building { get; set; }

        public int? Room { get; set; }

        public string? Days { get; set; }

        [DataType(DataType.Time)]
        public TimeOnly? StartTime { get; set; }

        [DataType(DataType.Time)]
        public TimeOnly? StopTime { get; set; }

        [Required]
        public string Semester { get; set; }

        [Required, Range(0000, 9999)]
        public int Year { get; set; }

        [Required]
        public string Color { get; set; }

        public string? Picture { get; set; }

    }
}
