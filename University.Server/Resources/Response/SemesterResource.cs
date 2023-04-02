namespace University.Server.Resources.Response
{
    public class SemesterResource
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Active { get; set; }

        public ICollection<SemesterModuleResource> Modules { get; set; } = new List<SemesterModuleResource>();
    }
}
