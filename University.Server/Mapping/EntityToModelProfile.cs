using AutoMapper;
using University.Server.Domain.Models;
using University.Server.Domain.Persistence.Entities;
using University.Server.Domain.Services;
using University.Server.Mapping.Actions;

namespace University.Server.Mapping
{
    public class EntityToModelProfile : Profile
    {
        public EntityToModelProfile()
        {
            CreateMap<CourseEntity, Course>()
                .AfterMap<ResolveCourseRelationsAction>();
            CreateMap<LectureEntity, Lecture>()
                .AfterMap<ResolveLectureRelationsAction>();
            CreateMap<LocationEntity, Location>();
            CreateMap<ModuleEntity, Module>()
                .AfterMap<ResolveModuleRelationsAction>();
            CreateMap<AssignmentEntity, Assignment>()
                .AfterMap<ResolveAssignmentRelationsAction>();
            CreateMap<SemesterModuleEntity, SemesterModule>()
                .ForMember(dest => dest.Lectures, opt => opt.MapFrom(src => src.Lectures))
                .AfterMap<ResolveSemesterRelationsAction>();
            CreateMap<SemesterEntity, Semester>()
                .ForMember(dest => dest.Modules, opt => opt.MapFrom(src => src.Modules));
            CreateMap<UserEntity, User>()
                .ForMember(dest => dest.Assignments, opt => opt.MapFrom(src => src.Assignments));
        }
    }
}
