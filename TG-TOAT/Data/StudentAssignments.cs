using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace TGTOAT.Data
{
    public class StudentAssignments
{
        [Key]
        public int AssignmentGradeId { get; set; }
        public int AssignmentId { get; set; }
        public Assignment? Assignments { get; set; }
        public int StudentId { get; set; }
        public StudentCourseConnection? studentCourseConnection { get; set; }

        public int? Grade {  get; set; }


        [DataType(DataType.DateTime)]
        public DateTime SubmissionDate { get; set; }

        [AllowNull]
        public string? TextSubmission { get; set; }

        [AllowNull]
        public string? FileSubmission { get; set; }





    }
}
