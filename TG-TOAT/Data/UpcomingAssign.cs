using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using Mono.TextTemplating;

namespace Data
{
    public class UpcomingAssign
    {
        [Key, Required]
        public int AssignId { get; set; }

        public int Type { get; set; }

        [Required]
        public string DeptName { get; set; }

        [Required]
        public int CourseNum { get; set; }

        [Required]
        public string AssignName { get; set; }

        [Required]
        public DateTime DueDate { get; set; }



    }
}
