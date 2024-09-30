using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using TGTOAT.Models;

namespace TGTOAT.Data
{
    public class InstructorCourseConnection
    {
        [Key]
        [Required(ErrorMessage = "User ID is required.")]
        public int InstructorID { get; set; }
        public User? Instructor { get; set; }

        [Required(ErrorMessage = "Course ID is required.")]
        public int CourseId { get; set; }
        public Courses? Course { get; set; }
    }
}
