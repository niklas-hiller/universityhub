namespace University.Server.Resources.Response
{
    public class SemesterModuleResource : AbstractModuleResource
    {
        public ICollection<LectureResource> Lectures { get; set; } = new List<LectureResource>();
        public UserResource? Professor { get; set; }
    }
}
