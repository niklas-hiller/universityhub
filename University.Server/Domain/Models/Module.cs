namespace University.Server.Domain.Models
{
    public class Module : Archivable
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public int CreditPoints { get; set; }
        public EModuleType ModuleType { get; set; }
        public int MaxSize { get; set; }

        public ICollection<Guid> ProfessorIds { get; set; } = new List<Guid>();
    }
}
