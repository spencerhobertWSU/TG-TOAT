using System.ComponentModel.DataAnnotations;
namespace TGTOAT.Models
{
    public class AddAssignmentViewModel
{
        // Primary Key for the Assignment
        public int AssignmentId { get; set; }

        // Foreign Key for InstructorCourseConnection
        [Required(ErrorMessage = "Instructor Course ID is required.")]
        public int InstructorCourseId { get; set; }

        [StringLength(60, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 60 characters.")]
        [Required(ErrorMessage = "Assignment name is required.")]
        public string AssignmentName { get; set; }

        [StringLength(60, MinimumLength = 1, ErrorMessage = "Description must be between 1 and 60 characters.")]
        [Required(ErrorMessage = "Assignment description is required.")]
        public string AssignmentDescription { get; set; }


        [Required(ErrorMessage = "Assignment points are required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Points must be a positive number.")]
        public int AssignmentPoints { get; set; }


        [StringLength(60, MinimumLength = 1, ErrorMessage = "Assignment type must be between 1 and 60 characters.")]
        [Required(ErrorMessage = "Assignment type is required.")]
        public string AssignmentType { get; set; }

        [DataType(DataType.DateTime)]
        [Required(ErrorMessage = "Due date and time is required.")]
        public DateTime DueDateAndTime { get; set; }

    }
}
