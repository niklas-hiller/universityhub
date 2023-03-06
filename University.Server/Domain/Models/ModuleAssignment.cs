namespace University.Server.Domain.Models
{
    public class ModuleAssignment : Base
    {
        public EModuleStatus Status { get; set; }

        public Module ReferenceModule { get; set; }
    }
}
