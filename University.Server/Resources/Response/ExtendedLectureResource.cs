namespace University.Server.Resources.Response
{
    public class ExtendedLectureResource : LectureResource
    {
        public string ModuleName { get; set; }
        public UserResource Professor { get; set; }
    }
}