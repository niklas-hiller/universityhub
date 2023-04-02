using AutoMapper;
using University.Server.Domain.Models;
using University.Server.Domain.Services;
using University.Server.Resources.Request;

namespace University.Server.Mapping.Actions
{
    public class ResolveModulesPatchAction : IMappingAction<PatchResource, PatchModel<Module>>
    {
        private readonly IModuleService _moduleService;

        public ResolveModulesPatchAction(IModuleService moduleService)
        {
            _moduleService = moduleService ?? throw new ArgumentNullException(nameof(moduleService));
        }

        public void Process(PatchResource source, PatchModel<Module> destination, ResolutionContext context)
        {
            destination.AddEntity = _moduleService.GetManyAsync(source.Add).GetAwaiter().GetResult().ToList();
            destination.RemoveEntity = _moduleService.GetManyAsync(source.Remove).GetAwaiter().GetResult().ToList();
        }
    }
}