﻿namespace TGTOAT.Models
{
    public class SubmitAssignmentViewModel{
        public int AssignmentId { get; set; }
        public string AssignmentName { get; set; }
        public string Description { get; set; }
        public int Points { get; set; }
        public string SubmissionType { get; set; }
    }
}
