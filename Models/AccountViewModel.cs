using TGTOAT.Data;

namespace TGTOAT.Models
{
    public class AccountViewModel
    {
        public int Id { get; set; }
        public List<Notifications> Notifications { get; set; }
        public string UserRole { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Address Address { get; set; } = new Address();
    }

}
