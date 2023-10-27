using System.ComponentModel.DataAnnotations;

namespace WebUI.DTOs
{
    public class RegisterDto
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
        [Compare("Password")]
        public string PasswordConfirmation { get; set; }
    }
}
