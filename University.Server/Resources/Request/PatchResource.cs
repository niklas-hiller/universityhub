namespace University.Server.Resources.Request
{
    public class PatchResource
    {
        public ICollection<Guid> Add { get; set; }
        public ICollection<Guid> Remove { get; set; }
    }
}
