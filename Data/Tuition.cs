using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace Data
{
    public class Tuition
    {
        [Key, Required]
        public int UserId { get; set; }

        [Required, DataType(DataType.Currency)]
        public Decimal AmountDue { get; set; }
    }
}
