using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using TGTOAT.Models;

namespace TGTOAT.Data
{
    public class Assignment
{
        // Primary Key for the Assignment
        [Key]
        [Required(ErrorMessage = "Assignment ID is required.")]
        public int AssignmentId { get; set; }


        [Required(ErrorMessage = "Instructor Course ID is required.")]
        public int InstructorCourseId { get; set; }
        public InstructorCourseConnection InstructorCourse { get; set; }


        [StringLength(60, MinimumLength = 1, ErrorMessage = "Description must be between 1 and 60 characters.")]
        [Required(ErrorMessage = "Assignment description is required.")]
        public string AssignmentDescription { get; set; }


        [StringLength(60, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 60 characters.")]
        [Required(ErrorMessage = "Assignment name is required.")]
        public string AssignmentName { get; set; }


        [DataType(DataType.DateTime)]
        [Required(ErrorMessage = "Due date and time is required.")]
        public DateTime DueDateAndTime { get; set; }


        [Required(ErrorMessage = "Assignment points are required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Points must be a positive number.")]
        public int AssignmentPoints { get; set; }


        [StringLength(60, MinimumLength = 1, ErrorMessage = "Assignment type must be between 1 and 60 characters.")]
        [Required(ErrorMessage = "Assignment type is required.")]
        public string AssignmentType { get; set; }

        [AllowNull]
        public string? Submission { get; set; }

        [Required]
        public int CourseId { get; set; }

        public Courses Course { get; set; }


    }
}
