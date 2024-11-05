using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using Mono.TextTemplating;

namespace Data
{
    public class Quizzes
    {
        [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int QuizId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        public string QuizName { get; set; }

        [Required]
        public string QuizDesc { get; set; }

        [Required]
        public string NumQuestions { get; set; }

        [Required]
        public int MaxPoints { get; set; }

        [Required, DataType(DataType.DateTime)]
        public DateTime DueDate { get; set; }

        [Required]
        public string Questions { get; set; }


    }
}
