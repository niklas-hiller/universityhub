namespace University.Server.Domain.Persistence.Entities
{
    public class CourseEntity : BaseEntity
    {
        public string Name { get; set; }

        public ICollection<Guid> StudentIds { get; set; } = new List<Guid>();
        public ICollection<Guid> ModuleIds { get; set; } = new List<Guid>();
    }
}
