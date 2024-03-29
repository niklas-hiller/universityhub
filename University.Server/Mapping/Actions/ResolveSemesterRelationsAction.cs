﻿using AutoMapper;
using University.Server.Domain.Models;
using University.Server.Domain.Persistence.Entities;
using University.Server.Domain.Services;

namespace University.Server.Mapping.Actions
{
    public class ResolveSemesterRelationsAction : IMappingAction<SemesterModuleEntity, SemesterModule>
    {
        private readonly IUserService _userService;
        private readonly IModuleService _moduleService;

        public ResolveSemesterRelationsAction(IUserService userService, IModuleService moduleService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _moduleService = moduleService ?? throw new ArgumentNullException(nameof(moduleService));
        }

        public void Process(SemesterModuleEntity source, SemesterModule destination, ResolutionContext context)
        {
            destination.Professor = source.ProfessorId != Guid.Empty
                ? _userService.GetAsync(source.ProfessorId).GetAwaiter().GetResult()
                : null;

            destination.ReferenceModule = _moduleService.GetAsync(source.ModuleId, false).GetAwaiter().GetResult();
        }
    }
}