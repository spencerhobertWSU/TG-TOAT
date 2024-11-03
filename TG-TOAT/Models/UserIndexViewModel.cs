using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using TGTOAT.Data;

namespace TGTOAT.Models
{
    public class UserIndexViewModel
    {
        /* User */
        public int Id { get; set; } // Add this property
        // First Name
        [Display(Name = "First Name")]
        [StringLength(60, MinimumLength = 1)]
        [Required]
        public string? FirstName { get; set; }

        // Last Name
        [Display(Name = "Last Name")]
        [StringLength(60, MinimumLength = 1)]
        [Required]
        public string? LastName { get; set; }

        // User Role (Instructor, Student, etc.)
        [Display(Name = "User Role")]
        [Required]
        public string? UserRole { get; set; }

        //notifications
        public List<Notifications> Notifications { get; set; }

        /* Courses */

        public List<Courses> Courses { get; set; } = new List<Courses>();

        public List<Assignment> UpcomingAssignments { get; set; }
    }
}
