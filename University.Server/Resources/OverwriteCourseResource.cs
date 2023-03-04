namespace University.Server.Resources
{
    public class OverwriteCourseResource
    {
        public List<Guid> Students { get; set; } = new List<Guid>();
        public List<Guid> Modules { get; set; } = new List<Guid>();
    }
}
