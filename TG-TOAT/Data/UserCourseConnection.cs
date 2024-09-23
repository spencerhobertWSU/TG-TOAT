using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using TGTOAT.Models;

namespace TGTOAT.Data
{
    public class UserCourseConnection
    {
        [Key]
        public int UserCourseConnectionId { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        public int UserId { get; set; }
        public User? User { get; set; }

        [Required(ErrorMessage = "Course ID is required.")]
        public int CourseId { get; set; }
        public Courses? Course { get; set; }
    }
}
