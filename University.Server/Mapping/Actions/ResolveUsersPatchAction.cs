using AutoMapper;
using University.Server.Domain.Models;
using University.Server.Domain.Services;
using University.Server.Resources;

namespace University.Server.Mapping.Actions
{
    public class ResolveUsersPatchAction : IMappingAction<PatchResource, PatchUsers>
    {
        private readonly IUserService _userService;

        public ResolveUsersPatchAction(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public void Process(PatchResource source, PatchUsers destination, ResolutionContext context)
        {
            destination.Add = source.Add.Select(id => _userService.GetAsync(id).GetAwaiter().GetResult()).ToList();
            destination.Remove = source.Remove.Select(id => _userService.GetAsync(id).GetAwaiter().GetResult()).ToList();
        }
    }
}
