using System.ComponentModel.DataAnnotations;

namespace University.Server.Resources
{
    public class LoginResource
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
