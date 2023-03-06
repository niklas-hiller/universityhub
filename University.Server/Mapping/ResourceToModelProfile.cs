using AutoMapper;
using University.Server.Domain.Models;
using University.Server.Domain.Services;
using University.Server.Resources;

namespace University.Server.Mapping
{
    public class ResourceToModelProfile : Profile
    {
        private readonly IUserService _userService;

        public ResourceToModelProfile(IUserService userService)
        {
            _userService = userService;
        }

        public ResourceToModelProfile()
        {
            CreateMap<SaveUserResource, User>();
            CreateMap<SaveModuleResource, Module>();
            CreateMap<SaveLocationResource, Location>();
            CreateMap<SaveSemesterResource, Semester>();
            CreateMap<SaveCourseResource, Course>();

            CreateMap<UpdateUserResource, User>();
            CreateMap<UpdateLocationResource, Location>();
            CreateMap<UpdateCourseResource, Course>();
            CreateMap<UpdateModuleResource, Module>();

            CreateMap<OverwriteCourseResource, Course>();
            CreateMap<OverwriteModuleResource, Module>()
                .ForMember(dest => dest.Professors, opt => 
                    opt.MapFrom(src => src.Professors.Select(p => _userService.GetAsync(p).GetAwaiter().GetResult())));
        }
    }
}
