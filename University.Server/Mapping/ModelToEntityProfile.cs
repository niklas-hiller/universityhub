using AutoMapper;
using University.Server.Domain.Models;
using University.Server.Domain.Persistence.Entities;

namespace University.Server.Mapping
{
    public class ModelToEntityProfile : Profile
    {
        public ModelToEntityProfile()
        {
            CreateMap<Course, CourseEntity>()
                .ForMember(dest => dest.StudentIds, opt => opt.MapFrom(src => src.Students.Select(m => m.Id)));
            CreateMap<Lecture, LectureEntity>()
                .ForMember(dest => dest.LocationId, opt => opt.MapFrom(src => src.Location.Id));
            CreateMap<Location, LocationEntity>();
            CreateMap<Module, ModuleEntity>()
                .ForMember(dest => dest.ProfessorIds, opt => opt.MapFrom(src => src.Professors.Select(m => m.Id)));
            CreateMap<Assignment, AssignmentEntity>()
                .ForMember(dest => dest.ReferenceModuleId, opt => opt.MapFrom(src => src.ReferenceModule.Id));
            CreateMap<SemesterModule, SemesterModuleEntity>()
                .ForMember(dest => dest.ProfessorId, opt => opt.MapFrom(src => src.Professor.Id))
                .ForMember(dest => dest.ModuleId, opt => opt.MapFrom(src => src.ReferenceModule.Id))
                .ForMember(dest => dest.Lectures, opt => opt.MapFrom(src => src.Lectures));
            CreateMap<Semester, SemesterEntity>()
                .ForMember(dest => dest.SemesterModules, opt => opt.MapFrom(src => src.Modules));
            CreateMap<User, UserEntity>()
                .ForMember(dest => dest.ModuleAssignments, opt => opt.MapFrom(src => src.Assignments));
        }
    }
}
