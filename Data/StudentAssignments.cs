using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace TGTOAT.Data
{
    public class StudentAssignments
{
        [Key]
        [Required]
        public int AssignmentGradeId { get; set; }

        [Required]
        public int AssignmentId { get; set; }
        [Required]
        public int StudentId { get; set; }


        [Required]
        public int MaxGrade { get; set; }
        public int? Grade {  get; set; }


        [DataType(DataType.DateTime)]
        public DateTime SubmissionDate { get; set; }

        [AllowNull]
        public string? TextSubmission { get; set; }

        [AllowNull]
        public string? FileSubmission { get; set; }



        public Assignment? Assignments { get; set; }
        public StudentCourseConnection? studentCourseConnection { get; set; }

    }
}
