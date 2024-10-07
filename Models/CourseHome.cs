using TGTOAT.Data;

namespace TGTOAT.Models
{
    public class CourseHome
    {
        public int CourseId { get; set; }
        public string UserRole { get; set; }
        public string Department { get; set; }
        public string CourseNum { get; set; }
        public List<Assignment> Assignments { get; set; }
    }
}
