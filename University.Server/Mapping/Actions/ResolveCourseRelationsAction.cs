using AutoMapper;
using University.Server.Domain.Models;
using University.Server.Domain.Persistence.Entities;
using University.Server.Domain.Services;

namespace University.Server.Mapping.Actions
{
    public class ResolveCourseRelationsAction : IMappingAction<CourseEntity, Course>
    {
        private readonly IUserService _userService;

        public ResolveCourseRelationsAction(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public void Process(CourseEntity source, Course destination, ResolutionContext context)
        {
            destination.Students = source.StudentIds.Select(id => _userService.GetAsync(id).GetAwaiter().GetResult()).ToList();
        }
    }
}
