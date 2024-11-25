using System.ComponentModel.DataAnnotations;
using Data;

namespace Models
{
    public class SubmitAssignmentViewModel
    {
        public int AssignmentId { get; set; }
        public string AssignmentName { get; set; }
        public string Description { get; set; }
        public int Points { get; set; }
        public string SubmissionType { get; set; }
        public string Submission { get; set; }

        public int StudentId { get; set; }
        public StudentConnection? studentConnection { get; set; }
        //notifications
        public List<Notifications> Notifications { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime DueDateAndTime { get; set; }
        public int? Grade { get; set; }


    }
}
