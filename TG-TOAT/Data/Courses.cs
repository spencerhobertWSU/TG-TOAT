using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using TGTOAT.Models;

namespace TGTOAT.Data
{
    public class Courses
    {
        [Key]
        public int CourseId { get; set; }
        // Department
        [Required(ErrorMessage = "Department is required.")]
        public int DepartmentId { get; set; }
        public Departments Department { get; set; }

        // Course Number
        [Display(Name = "Course Number")]
        [Required(ErrorMessage = "Course number is required.")]
        [MaxLength(10, ErrorMessage = "Max 10 characters allowed.")]
        public string CourseNumber { get; set; }

        // Course Name
        [Display(Name = "Course Name")]
        [Required(ErrorMessage = "Course name is required.")]
        [MaxLength(30, ErrorMessage = "Max 30 characters allowed.")]
        public string CourseName { get; set; }

        // Course Description
        [Display(Name = "Course Description")]
        [Required(ErrorMessage = "Course description is required.")]
        [MaxLength(400, ErrorMessage = "Max 400 characters allowed.")]
        public string CourseDescription { get; set; }

        // Number of Credits
        [Display(Name = "Number of Credits")]
        [Required(ErrorMessage = "Number of credits is required.")]
        [Range(1, 999, ErrorMessage = "Must be a positive number and max 3 digits allowed.")]
        public int NumberOfCredits { get; set; }

        // Capacity
        [Required(ErrorMessage = "Capacity is required.")]
        [Range(1, 999, ErrorMessage = "Must be a positive number and max 3 digits allowed.")]
        public int Capacity { get; set; }

        // Campus
        [Required(ErrorMessage = "Location is required.")]
        [MaxLength(50, ErrorMessage = "Max 50 characters allowed.")]
        public string? Campus { get; set; }

        // Building
        [Required(ErrorMessage = "Location is required.")]
        [MaxLength(50, ErrorMessage = "Max 50 characters allowed.")]
        public string? Building { get; set; }

        // Room Number
        [Display(Name = "Room Number")]
        [Required(ErrorMessage = "Room number is required.")]
        [Range(1, 999, ErrorMessage = "Must be a positive number and max 3 digits allowed.")]
        public int? RoomNumber { get; set; }

        // Meeting Days
        [Display(Name = "Days of the Week")]
        [Required(ErrorMessage = "Days of the week are required.")]
        public string DaysOfTheWeek { get; set; }

        // Start Time
        [Display(Name = "Start Time")]
        [Required(ErrorMessage = "Start time is required.")]
        public TimeOnly? StartTime { get; set; }

        // End Time
        [Display(Name = "End Time")]
        [Required(ErrorMessage = "End time is required.")]
        public TimeOnly? EndTime { get; set; }

        // Semester
        [Required(ErrorMessage = "Semester is required.")]
        [MaxLength(10, ErrorMessage = "Max 10 characters allowed.")]
        public string? Semester { get; set; }

        // Year
        [Required(ErrorMessage = "Year is required.")]
        [Range(2024, 3000, ErrorMessage = "Must be a positive number and max 4 digits allowed.")]
        public int Year { get; set; }

        public List<InstructorCourseConnection> instructorCourseConnections { get; set; } = new List<InstructorCourseConnection>();
        public List<StudentCourseConnection> StudentCourseConnection { get; set; } = new List<StudentCourseConnection>();
        public List<User> Instructors { get; set; } = new List<User>();

        public List<Notifications> Notifications { get; set; } = new List<Notifications>();

        public List<Assignment> Assignments { get; set; }

    }

}