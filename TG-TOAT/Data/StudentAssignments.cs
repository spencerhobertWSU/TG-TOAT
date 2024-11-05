using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using Mono.TextTemplating;

namespace Data
{
    public class StudentAssignments
    {
        [Key, ForeignKey("AssignId"), Required]
        public int AssignId { get; set; }

        [ForeignKey("UserId"), Required]
        public int StudentId { get; set; }

        public int? Points { get; set; }

        [Required]
        public DateTime Submitted { get; set; }

        [Required]
        public string Submission { get; set; }


    }
}
