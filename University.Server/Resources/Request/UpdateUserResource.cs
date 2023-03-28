using System.ComponentModel.DataAnnotations;

namespace University.Server.Resources.Request
{
    public class UpdateUserResource
    {
        [Required]
        [MaxLength(30)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(30)]
        public string LastName { get; set; }
        [Required]
        [MaxLength(90)]
        public string Email { get; set; }
    }
}
