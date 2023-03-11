using System.ComponentModel.DataAnnotations;

namespace University.Server.Resources
{
    public class UpdateUserResource
    {
        [MaxLength(30)]
        public string FirstName { get; set; }
        [MaxLength(30)]
        public string LastName { get; set; }
        [MaxLength(90)]
        public string Email { get; set; }
    }
}
