using System.ComponentModel.DataAnnotations;
using Data;

namespace Models
{
    public class TakeQuizModel
    {
        public int stuId { get; set; }
        public string UserRole { get; set; }
        public int CourseId { get; set; }
        public int QuizId { get; set; }

        public int InstructorCourseId { get; set; }
        public string QuizName { get; set; }

        public string QuizDescription { get; set; }

        public int QuizPoints { get; set; }
        public string NumQuestions { get; set; }

        public string Questions { get; set; }
        public DateTime DueDateAndTime { get; set; }
        public string StudentId { get; set; }
        public int Points { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime Submitted { get; set; }
        public string Submission { get; set; }
        public List<Notifications> Notifications { get; internal set; }
    }
}
