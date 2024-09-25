using System;
using System.ComponentModel.DataAnnotations;
using TGTOAT.Data;

namespace TGTOAT.Models
{
    public class CourseRegisterViewModel
    {
        //public int CourseId { get; set; } // Unique identifier for the course

        //public string CourseNumber { get; set; } // Course number

        //public int DepartmentId { get; set; } // Department ID

        //public Departments Department { get; set; } // Department

        //public string CourseName { get; set; } // Course name

        //public int NumberOfCredits { get; set; } // Credit hours

        //public string Instructor { get; set; } // Instructor name

        // public int InstructorId { get; set; }

        // public User Instructor { get; set; }

        //public string DaysOfTheWeek { get; set; } // Days of the week

        //public DateTime StartTime { get; set; } // Start time

        //public DateTime EndTime { get; set; } // End time

        //public int RoomNumber { get; set; } // Course location

        //public string CourseDescription { get; set; } // Course Description
        //public int Capacity { get; set; } // Class capacity
        //public string Semester { get; set; } // Semester
        public List<Departments> Departments { get; set; } = new List<Departments>();
        public List<Courses> Courses { get; set; } = new List<Courses>();

        public List<UserCourseConnection> userCourseConnections { get; set; } = new List<UserCourseConnection>();
    }
}