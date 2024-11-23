using Data;

namespace Models
{
    public class CourseHome
    {
        public int CourseId { get; set; }
        public string UserRole { get; set; }
        public string Department { get; set; }
        public string Description   { get; set; }
        public int CourseNum { get; set; }
        public List<Assignment> Assignments { get; set; }
        public List<Notifications> Notifications { get; set; } // Adjust type as needed

        public List<Quizzes> Quizzes { get; set; }

        public List<StudentQuizzes> studentQuizzes { get; set; }
    }
}
