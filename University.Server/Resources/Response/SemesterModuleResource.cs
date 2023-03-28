namespace University.Server.Resources.Response
{
    public class SemesterModuleResource : AbstractModuleResource
    {
        public List<LectureResource> Lectures { get; set; } = new List<LectureResource>();
        public UserResource? Professor { get; set; }
    }
}
