using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Data
{
    public class CurrClasses
    {
        public string DeptName { get; set; }

        public int CourseId { get; set; }
        public int CourseNum { get; set; }
        public string CourseName { get; set; }
        //public string CourseDesc { get; set; }
        //public int Credits { get; set; }
        //public int Capacity { get; set; }
        public string? Campus { get; set; }
        public string? Building { get; set; }
        public int? Room { get; set; }
        public string? Days { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? StopTime { get; set; }
        public string Semester { get; set; }
        public int Year { get; set; }
        public string Color { get; set; }
        public string Picture { get; set; }
    }
}
