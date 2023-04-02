namespace University.Server.Resources.Response
{
    public class CourseResource
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<UserResource> Students { get; set; } = new List<UserResource>();
        public ICollection<ModuleResource> Modules { get; set; } = new List<ModuleResource>();
    }
}
