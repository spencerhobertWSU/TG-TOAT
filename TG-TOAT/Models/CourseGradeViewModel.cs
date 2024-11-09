using System.Collections.Generic;
using Data;

namespace Models
{
    public class CourseGradeViewModel
    {
        public int CourseId { get; set; }
        public string UserRole { get; set; }
        public string Department { get; set; }
        public string CourseNum { get; set; }
        public string? Grade { get; set; }

        public List<KeyValuePair<string, int>> GradeDistribution { get; set; }

        public List<Notifications> Notifications { get; set; }
    }
}
