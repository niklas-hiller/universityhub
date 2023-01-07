namespace University.Server.Domain.Models
{
    public class Module
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int CreditPoints { get; set; }
        public List<Professor> AvailableProfessors { get; set; }
    }
}
