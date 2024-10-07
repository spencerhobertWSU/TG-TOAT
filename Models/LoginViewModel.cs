using System.ComponentModel.DataAnnotations;

namespace TGTOAT.Models
{
    public class LoginViewModel
    {
        [EmailAddress]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Keep Me Logged In")]
        public bool RememberMe { get; set; }
    }
}
