using System.ComponentModel.DataAnnotations;
using University.Server.Domain.Models;

namespace University.Server.Resources
{
    public class SaveCourseResource
    {
        [Required]
        public string Name { get; set; }
        public List<Guid> Students { get; set; }
        public List<Guid> Modules { get; set; }
    }
}
