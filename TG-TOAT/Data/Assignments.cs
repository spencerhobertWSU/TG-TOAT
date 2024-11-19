using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using Mono.TextTemplating;

namespace Data
{
    public class Assignment
    {
        [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AssignId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        public string AssignName { get; set; }

        [Required]
        public string AssignDesc { get; set; }

        [Required]
        public string AssignType { get; set; }

        [Required]
        public int MaxPoints { get; set; }

        [Required, DataType(DataType.DateTime)]
        public DateTime DueDate { get; set; }

        //public List<Notifications> Notifications { get; set; }


    }
}
