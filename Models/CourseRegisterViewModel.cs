using System;
using System.ComponentModel.DataAnnotations;
using Data;
using TGTOAT.Data;

namespace Models
{
    public class CourseRegisterViewModel
    {

        public List<CourseInfo> Courses { get; set; } = new List<CourseInfo>();
        public int CurrentStudent { get; set; }
        //public List<Departments> Departments { get; set; } = new List<Departments>();
        //public List<User> Instructors { get; set; } = new List<User>();
        //public List<InstructorConnection> InstructorConnections { get; set; } = new List<InstructorConnection>();
        public List<StudentConnection> StudentConnection { get; set; } = new List<StudentConnection>();

    }
}