using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using Mono.TextTemplating;

namespace Data
{
    public class UserInfo
    {
        [Key, ForeignKey("UserID"), Required]
        public int UserId { get; set; }

        [Required, RegularExpression(@"[a-zA-Z]*$")]
        public string FirstName { get; set; }

        [Required, RegularExpression(@"[a-zA-Z]*$")]
        public string LastName { get; set; }

        [Required]
        public string Role { get; set; }

        [Required]
        public string PFP { get; set; }

        [Required, DataType(DataType.Date)]
        public DateOnly BirthDate { get; set; }
    }
}
