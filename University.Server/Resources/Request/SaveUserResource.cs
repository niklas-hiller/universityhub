using System.ComponentModel.DataAnnotations;
using University.Server.Domain.Models;

namespace University.Server.Resources.Request
{
    public class SaveUserResource
    {
        [Required]
        [MinLength(1), MaxLength(30)]
        public string FirstName { get; set; }
        [Required]
        [MinLength(1), MaxLength(30)]
        public string LastName { get; set; }
        [Required]
        [MinLength(1), MaxLength(90)]
        public string Email { get; set; }
        [Required]
        [MinLength(8), MaxLength(30)]
        public string Password { get; set; }
        [Required]
        public EAuthorization Authorization { get; set; }
    }
}
