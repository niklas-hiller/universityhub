namespace University.Server.Domain.Models
{
    public class Assignment
    {
        public EModuleStatus Status { get; set; }

        public Module ReferenceModule { get; set; }
    }
}