using AutoMapper;
using University.Server.Domain.Models;
using University.Server.Domain.Persistence.Entities;
using University.Server.Domain.Services;

namespace University.Server.Mapping.Actions
{
    public class ResolveModuleRelationsAction : IMappingAction<ModuleEntity, Module>
    {
        private readonly IUserService _userService;

        public ResolveModuleRelationsAction(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public void Process(ModuleEntity source, Module destination, ResolutionContext context)
        {
            destination.Professors = source.ProfessorIds.Select(id => _userService.GetAsync(id).GetAwaiter().GetResult()).ToList();
        }
    }
}
