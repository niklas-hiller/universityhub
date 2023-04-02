namespace University.Server.Resources.Request
{
    public class PatchResource
    {
        public IEnumerable<Guid> Add { get; set; }
        public IEnumerable<Guid> Remove { get; set; }
    }
}