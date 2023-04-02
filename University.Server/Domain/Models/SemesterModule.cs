namespace University.Server.Domain.Models
{
    public class SemesterModule : Base
    {
        public ICollection<Lecture> Lectures { get; set; } = new List<Lecture>();
        public User? Professor { get; set; }
        public Module ReferenceModule { get; set; }
    }
}