using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using Mono.TextTemplating;

namespace Data
{
    public class InstructorConnection
    {
        [Key, ForeignKey("UserId"), Required]
        public int InstructorId { get; set; }

        [ForeignKey("CourseId"), Required]
        public int CourseId { get; set; }

    }
}
