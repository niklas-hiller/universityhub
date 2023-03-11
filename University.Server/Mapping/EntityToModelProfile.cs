using AutoMapper;
using University.Server.Domain.Models;
using University.Server.Domain.Persistence.Entities;
using University.Server.Domain.Services;

namespace University.Server.Mapping
{
    public class EntityToModelProfile : Profile
    {
        private readonly IUserService _userService;
        private readonly IModuleService _moduleService;
        private readonly ILocationService _locationService;

        public EntityToModelProfile(IUserService userService, IModuleService moduleService, ILocationService locationService)
        {
            _userService = userService;
            _moduleService = moduleService;
            _locationService = locationService;
        }

        public EntityToModelProfile()
        {
            CreateMap<CourseEntity, Course>()
                .ForMember(dest => dest.Students, opt =>
                    opt.MapFrom(src => src.StudentIds.Select(m => _userService.GetAsync(m).GetAwaiter().GetResult())));
            CreateMap<LectureEntity, Lecture>()
                .ForMember(dest => dest.Location, opt =>
                    opt.MapFrom(src => _locationService.GetAsync(src.LocationId)));
            CreateMap<LocationEntity, Location>();
            CreateMap<ModuleEntity, Module>()
                .ForMember(dest => dest.Professors, opt =>
                    opt.MapFrom(src => src.ProfessorIds.Select(m => _userService.GetAsync(m).GetAwaiter().GetResult())));
            CreateMap<AssignmentEntity, Assignment>()
                .ForMember(dest => dest.ReferenceModule, opt =>
                    opt.MapFrom(src => _moduleService.GetAsync(src.ReferenceModuleId)));
            CreateMap<SemesterModuleEntity, SemesterModule>()
                .ForMember(dest => dest.Professor, opt =>
                    opt.MapFrom(src => _userService.GetAsync(src.ProfessorId)))
                .ForMember(dest => dest.ReferenceModule, opt =>
                    opt.MapFrom(src => _moduleService.GetAsync(src.ModuleId)))
                .ForMember(dest => dest.Lectures, opt => opt.MapFrom(src => src.Lectures));
            CreateMap<SemesterEntity, Semester>()
                .ForMember(dest => dest.Modules, opt => opt.MapFrom(src => src.SemesterModules));
            CreateMap<UserEntity, User>()
                .ForMember(dest => dest.Assignments, opt => opt.MapFrom(src => src.ModuleAssignments));
        }
    }
}
