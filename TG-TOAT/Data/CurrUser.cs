using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Data
{
    public class CurrUser
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public byte[] PFP { get; set; }
        public DateOnly BirthDate { get; set; }
    }

}
