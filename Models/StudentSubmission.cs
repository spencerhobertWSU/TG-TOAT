﻿using System;

namespace Models
{
    public class StudentSubmission
    {
        public string StudentFullName { get; set; }
        public DateTime SubmissionDate { get; set; }
        public int MaxPoints { get; set; }
        public string GivenPoints { get; set; }
        public int AssignmentId { get; set; }  
        public int StudentId { get; set; }   
    }
}
