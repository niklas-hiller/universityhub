namespace University.Server.Resources
{
    public class PatchResource
    {
        public ICollection<Guid> Add { get; set; }
        public ICollection<Guid> Remove { get; set; }
    }
}
