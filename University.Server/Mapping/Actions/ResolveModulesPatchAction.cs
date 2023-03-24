using AutoMapper;
using University.Server.Domain.Models;
using University.Server.Domain.Services;
using University.Server.Resources;

namespace University.Server.Mapping.Actions
{
    public class ResolveModulesPatchAction : IMappingAction<PatchResource, PatchModules>
    {
        private readonly IModuleService _moduleService;

        public ResolveModulesPatchAction(IModuleService moduleService)
        {
            _moduleService = moduleService ?? throw new ArgumentNullException(nameof(moduleService));
        }

        public void Process(PatchResource source, PatchModules destination, ResolutionContext context)
        {
            destination.Add = source.Add.Select(id => _moduleService.GetAsync(id).GetAwaiter().GetResult()).ToList();
            destination.Remove = source.Remove.Select(id => _moduleService.GetAsync(id).GetAwaiter().GetResult()).ToList();
        }
    }
}
