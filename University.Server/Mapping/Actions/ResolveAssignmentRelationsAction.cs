using AutoMapper;
using University.Server.Domain.Models;
using University.Server.Domain.Persistence.Entities;
using University.Server.Domain.Services;

namespace University.Server.Mapping.Actions
{
    public class ResolveAssignmentRelationsAction : IMappingAction<AssignmentEntity, Assignment>
    {
        private readonly IModuleService _moduleService;

        public ResolveAssignmentRelationsAction(IModuleService moduleService)
        {
            _moduleService = moduleService ?? throw new ArgumentNullException(nameof(moduleService));
        }

        public void Process(AssignmentEntity source, Assignment destination, ResolutionContext context)
        {
            destination.ReferenceModule = _moduleService.GetAsyncNullable(source.ReferenceModuleId, false).GetAwaiter().GetResult();
        }
    }
}
