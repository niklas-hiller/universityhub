namespace University.Server.Domain.Models
{
    public class PatchModel<T> where T : Base
    {
        public ICollection<T> AddEntity { get; set; } = new List<T>();
        public ICollection<T> RemoveEntity { get; set; } = new List<T>();
    }
}
