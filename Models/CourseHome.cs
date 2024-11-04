using Data;

namespace Models
{
    public class CourseHome
    {
        public int CourseId { get; set; }
        public string UserRole { get; set; }
        public string Department { get; set; }
        public int CourseNum { get; set; }
        public List<Assignment> Assignments { get; set; }

        public List<Quizzes> Quizzes { get; set; }
    }
}
