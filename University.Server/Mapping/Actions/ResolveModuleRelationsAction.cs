using AutoMapper;
using University.Server.Domain.Models;
using University.Server.Domain.Services;
using University.Server.Resources.Response;

namespace University.Server.Mapping.Actions
{
    public class ResolveModuleRelationsAction : IMappingAction<Module, ModuleResource>
    {
        private readonly IUserService _userService;

        public ResolveModuleRelationsAction(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public void Process(Module source, ModuleResource destination, ResolutionContext context)
        {
            destination.Professors = source.ProfessorIds.Select(id => context.Mapper.Map<UserResource>(_userService.GetAsync(id, false).GetAwaiter().GetResult())).ToList();
        }
    }
}