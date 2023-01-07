namespace University.Server.Domain.Models
{
    public class SemesterModule
    {
        public Guid Id { get; set; }
        public List<Lecture> Lectures { get; set; }
        public User Professor { get; set; }
        public Module ReferenceModule { get; set; }
    }
}
