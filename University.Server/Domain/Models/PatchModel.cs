namespace University.Server.Domain.Models
{
    public class PatchModel<T> where T : Base
    {
        public ICollection<T> Add { get; set; } = new List<T>();
        public ICollection<T> Remove { get; set; } = new List<T>();
    }
}
