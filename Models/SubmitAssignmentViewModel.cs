using System.ComponentModel.DataAnnotations;
using TGTOAT.Data;

namespace TGTOAT.Models
{
    public class SubmitAssignmentViewModel{
        public int AssignmentId { get; set; }
        public string AssignmentName { get; set; }
        public string Description { get; set; }
        public int Points { get; set; }
        public string SubmissionType { get; set; }

        public int StudentId { get; set; }
        public StudentCourseConnection? studentCourseConnection { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime DueDateAndTime { get; set; }
        public int? Grade { get; set; }


    }
}
