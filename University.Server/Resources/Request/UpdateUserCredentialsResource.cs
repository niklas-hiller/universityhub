using System.ComponentModel.DataAnnotations;

namespace University.Server.Resources.Request
{
    public class UpdateUserCredentialsResource
    {
        [Required]
        [MinLength(8), MaxLength(30)]
        public string Password { get; set; }
    }
}
