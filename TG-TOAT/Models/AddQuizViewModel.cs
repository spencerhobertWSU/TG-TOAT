using System.ComponentModel.DataAnnotations;
namespace Models
{
    public class AddQuizViewModel
    {
        
        // Primary Key for the Assignment
        public int QuizId { get; set; }

        // Foreign Key for InstructorCourseConnection
        [Required(ErrorMessage = "Instructor Course ID is required.")]
        public int InstructorCourseId { get; set; }

        [StringLength(60, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 60 characters.")]
        [Required(ErrorMessage = "Quiz name is required.")]
        public string QuizName { get; set; }

        [StringLength(60, MinimumLength = 1, ErrorMessage = "Description must be between 1 and 60 characters.")]
        [Required(ErrorMessage = "Quiz description is required.")]
        public string QuizDescription { get; set; }


        [Required(ErrorMessage = "Quiz points are required.")]
        [Range(0, 1000, ErrorMessage = "Points must be a positive number.")]
        public int QuizPoints { get; set; }

        [StringLength(1000, MinimumLength = 1, ErrorMessage = "Assignment type must be between 1 and 60 characters.")]
        [Required(ErrorMessage = "Number of Questions is Required.")]
        public string NumQuestions { get; set; }


        [StringLength(1000, MinimumLength = 1, ErrorMessage = "Assignment type must be between 1 and 60 characters.")]
        [Required(ErrorMessage = "Questions are Required.")]
        public string Questions { get; set; }

        [DataType(DataType.DateTime)]
        [Required(ErrorMessage = "Due date and time is required.")]
        public DateTime DueDateAndTime { get; set; }

    }
}
