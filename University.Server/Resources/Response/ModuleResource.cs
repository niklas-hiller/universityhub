namespace University.Server.Resources.Response
{
    public class ModuleResource : AbstractModuleResource
    {
        public int MaxSize { get; set; }
        public ICollection<UserResource> Professors { get; set; } = new List<UserResource>();
    }
}
