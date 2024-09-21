using System.ComponentModel.DataAnnotations;

namespace TGTOAT.Data
{
    public class Departments
    {
        [Key]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Department name is required.")]
        [MaxLength(50, ErrorMessage = "Max 50 characters allowed.")]
        public string DepartmentName { get; set; }
    }
}
