using System.Collections.Generic;

namespace TGTOAT.Models
{
    public class CourseGradeViewModel
    {
        public int CourseId { get; set; }
        public string UserRole { get; set; }
        public string Department { get; set; }
        public string CourseNum { get; set; }
        public string? Grade { get; set; }

        public List<KeyValuePair<string, int>> GradeDistribution { get; set; }
    }
}
