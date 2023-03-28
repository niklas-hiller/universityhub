using University.Server.Domain.Models;

namespace University.Server.Resources.Response
{
    public class AssignmentResource : AbstractModuleResource
    {
        public EModuleStatus Status { get; set; }
    }
}
