using AutoMapper;
using University.Server.Domain.Models;
using University.Server.Domain.Services;
using University.Server.Resources;

namespace University.Server.Mapping.Actions
{
    public class ResolveUsersPatchAction : IMappingAction<PatchResource, PatchModel<User>>
    {
        private readonly IUserService _userService;

        public ResolveUsersPatchAction(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public void Process(PatchResource source, PatchModel<User> destination, ResolutionContext context)
        {
            destination.AddEntity = source.Add.Select(id => _userService.GetAsync(id).GetAwaiter().GetResult()).ToList();
            destination.RemoveEntity = source.Remove.Select(id => _userService.GetAsync(id).GetAwaiter().GetResult()).ToList();
        }
    }
}
