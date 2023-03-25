﻿using AutoMapper;
using University.Server.Domain.Models;
using University.Server.Domain.Persistence.Entities;
using University.Server.Domain.Services;

namespace University.Server.Mapping.Actions
{
    public class ResolveCourseRelationsAction : IMappingAction<CourseEntity, Course>
    {
        private readonly IUserService _userService;
        private readonly IModuleService _moduleService;

        public ResolveCourseRelationsAction(IUserService userService, IModuleService moduleService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _moduleService = moduleService ?? throw new ArgumentNullException(nameof(moduleService));
        }

        public void Process(CourseEntity source, Course destination, ResolutionContext context)
        {
            destination.Students = source.StudentIds.Select(id => _userService.GetAsyncNullable(id).GetAwaiter().GetResult()).ToList();
            destination.Modules = source.ModuleIds.Select(id => _moduleService.GetAsyncNullable(id, false).GetAwaiter().GetResult()).ToList();
        }
    }
}
