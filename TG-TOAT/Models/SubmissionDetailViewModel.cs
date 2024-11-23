using Data;
using System;

namespace Models
{
    public class SubmissionDetailViewModel
    {
        public List<Notifications> Notifications { get; set; }
        public string StudentFullName { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime SubmissionDate { get; set; }
        public int MaxPoints { get; set; }
        public string GivenPoints { get; set; }
        public string TextSubmission { get; set; }
        public string FileSubmission { get; set; }
        public bool HasFile { get; set; }
        public int AssignmentId { get; set; } 
        public int StudentId { get; set; }

    }
}
