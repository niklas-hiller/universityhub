namespace University.Server.Domain.Models
{
    public class ModuleAssignment
    {
        public Guid Id { get; set; }
        public EModuleStatus Status { get; set; }
        public Module ReferenceModule { get; set; }
    }
}
