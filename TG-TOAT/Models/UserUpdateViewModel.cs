using TGTOAT.Data;

namespace TGTOAT.Models
{
    public class UserUpdateViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Address Address { get; set; }
    }

}
