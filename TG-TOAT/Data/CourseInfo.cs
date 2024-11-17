namespace Data
{
    public class CourseInfo
    {
        public int CourseId { get; set; } // Unique identifier for the course

        public int DeptID { get; set; }
        public string DeptName { get; set; } // Department
        public int CourseNumber { get; set; } // Course number
        public string CourseName { get; set; } // Course name
        public int NumberOfCredits { get; set; } // Credit hours
        public string? DaysOfTheWeek { get; set; } // Days of the week
        public TimeOnly? StartTime { get; set; } // Start time
        public TimeOnly? EndTime { get; set; } // End time
        public string? Campus { get; set; } //Campus
        public string? Building { get; set; }
        public int? Room { get; set; }//Room
        public int Capacity { get; set; } // Class capacity
        public string Semester { get; set; } // Semester
        public int Year { get; set; }//Year
        public string Instructor { get; set; }//Instructor

        public string CourseDescription { get; set; } // Course Description

        public List<Notifications> Notifications { get; set; }
    }
}
