using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic; // For IEnumerable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TGTOAT.Data;

namespace TGTOAT.Models
{
    public class AddCourseViewModel
    {
        public int? CourseId { get; set; }  // For editing course
        public string? UserRole { get; set; }

        [Required(ErrorMessage = "Department is required.")]
        [Display(Name = "Department")]
        public int SelectedDepartmentId { get; set; }

        public List<Departments> Departments { get; set; } = new List<Departments>();

        // Course Number
        [Required(ErrorMessage = "Course number is required.")]
        [Range(0000, 9999, ErrorMessage = "Must be a positive number and max 4 digits allowed.")]
        public string CourseNumber { get; set; }

        // Course Name
        [Required(ErrorMessage = "Course name is required.")]
        [MaxLength(30, ErrorMessage = "Max 30 characters allowed.")]
        public string CourseName { get; set; }

        // Course Description
        [Required(ErrorMessage = "Course description is required.")]
        [MaxLength(400, ErrorMessage = "Max 400 characters allowed.")]
        public string CourseDescription { get; set; }

        // Number of Credits
        [Required(ErrorMessage = "Number of credits is required.")]
        [Range(1, 999, ErrorMessage = "Must be a positive number and max 3 digits allowed.")]
        public int NumberOfCredits { get; set; }

        // Capacity
        [Required(ErrorMessage = "Capacity is required.")]
        [Range(1, 999, ErrorMessage = "Must be a positive number and max 3 digits allowed.")]
        public int Capacity { get; set; }

        // Campus
        [Required(ErrorMessage = "Campus is required.")]
        [MaxLength(5)]
        public string? Campus { get; set; }

        // Campus List
        public List<SelectListItem>? CampusList { get; set; }

        // Building List       
        public List<SelectListItem> Buildings { get; set; } = new List<SelectListItem>();

        // Building
        [Required(ErrorMessage = "Building is required.")]
        [MaxLength(50, ErrorMessage = "Max 50 characters allowed.")]
        public string? Building { get; set; }

        // Room Number
        [Required(ErrorMessage = "Room number is required.")]
        [Range(0, 999, ErrorMessage = "Must be a positive number and max 3 digits allowed.") ]
        public int? RoomNumber { get; set; }

        // Meeting Days
        [Display(Name = "Days of the Week")]
        [Required(ErrorMessage = "Days of the week are required.")]
        public string DaysOfTheWeek { get; set; }

        // Start Time
        [Required(ErrorMessage = "Start time is required.")]
        [DataType(DataType.Time)]
        public TimeOnly? StartTime { get; set; }

        // End Time
        [Required(ErrorMessage = "End time is required.")]
        [DataType(DataType.Time)]
        public TimeOnly? EndTime { get; set; }

        // Semester
        [Required(ErrorMessage = "Semester is required.")]
        [MaxLength(10, ErrorMessage = "Max 10 characters allowed.")]
        public string? Semester { get; set; }

        // Semester List
        public List<SelectListItem>? SemesterList { get; set; }

        // Year
        [Required(ErrorMessage = "Year is required.")]
        [Range(2024, 3000, ErrorMessage = "Must be a positive number and max 4 digits allowed.")]
        public int Year { get; set; }
        public int CurrYear { get; set; }

        // Instructor
        [Required(ErrorMessage = "Instructor is required.")]
        [Display(Name = "Instructor")]
        public int SelectedInstructorId { get; set; }

        public List<User> Instructors { get; set; } = new List<User>();

        //notifications
        public List<Notifications> Notifications { get; set; } = new List<Notifications>();

    }
}
