namespace University.Server.Resources.Response
{
    public class ModuleResource : AbstractModuleResource
    {
        public int MaxSize { get; set; }
        public List<Guid> ProfessorIds { get; set; } = new List<Guid>();
    }
}
