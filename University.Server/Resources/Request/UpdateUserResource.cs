using System.ComponentModel.DataAnnotations;

namespace University.Server.Resources.Request
{
    public class UpdateUserResource
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
    }
}
