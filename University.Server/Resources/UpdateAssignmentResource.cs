using System.ComponentModel.DataAnnotations;
using University.Server.Domain.Models;

namespace University.Server.Resources
{
    public class UpdateAssignmentResource
    {
        [Required]
        public EModuleStatus Status { get; set; }
    }
}
