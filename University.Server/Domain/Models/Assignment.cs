namespace University.Server.Domain.Models
{
    public class Assignment : Base
    {
        public EModuleStatus Status { get; set; }

        public Module ReferenceModule { get; set; }
    }
}