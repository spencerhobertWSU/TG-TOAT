using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace Data
{
    public class Departments
    {
        [Key, Required]
        public int DeptId { get; set; }

        [Required]
        public string DeptName { get; set; }


    }
}
