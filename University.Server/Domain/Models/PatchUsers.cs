namespace University.Server.Domain.Models
{
    public class PatchUsers
    {
        public ICollection<User> Add { get; set; } = new List<User>();
        public ICollection<User> Remove { get; set; } = new List<User>();
    }
}
