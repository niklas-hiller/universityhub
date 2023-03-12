namespace University.Server.Domain.Persistence.Entities
{
    public class ArchivableEntity : BaseEntity
    {
        public bool IsArchived { get; set; } = false;
    }
}
