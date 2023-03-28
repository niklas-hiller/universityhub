using AutoMapper;
using University.Server.Domain.Models;
using University.Server.Mapping.Converter;
using University.Server.Resources;
using University.Server.Resources.Response;

namespace University.Server.Mapping
{
    public class ModelToResourceProfile : Profile
    {
        public ModelToResourceProfile()
        {
            CreateMap<Assignment, AssignmentResource>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ReferenceModule.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ReferenceModule.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.ReferenceModule.Description))
                .ForMember(dest => dest.CreditPoints, opt => opt.MapFrom(src => src.ReferenceModule.CreditPoints))
                .ForMember(dest => dest.ModuleType, opt => opt.MapFrom(src => src.ReferenceModule.ModuleType));
            CreateMap<User, UserResource>()
                .ForMember(dest => dest.Assignments, opt => opt.MapFrom((user, userResource, i, context) =>
                {
                    return user.Assignments.Select(assignment => context.Mapper.Map<Assignment, AssignmentResource>(assignment));
                }));
            CreateMap<Module, ModuleResource>();
            CreateMap<Course, CourseResource>()
                .ForMember(dest => dest.Students, opt => opt.MapFrom((course, courseResource, i, context) =>
                {
                    return course.Students.Select(user => context.Mapper.Map<User, UserResource>(user));
                }))
                .ForMember(dest => dest.Modules, opt => opt.MapFrom((module, moduleResource, i, context) =>
                {
                    return module.Modules.Select(module => context.Mapper.Map<Module, ModuleResource>(module));
                }));

            CreateMap<Location, LocationResource>();
            CreateMap<Lecture, LectureResource>()
                .ForMember(dest => dest.Location, opt => opt.MapFrom((lecture, lectureResource, i, context) =>
                {
                    return context.Mapper.Map<Location, LocationResource>(lecture.Location);
                }));
            CreateMap<SemesterModule, SemesterModuleResource>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ReferenceModule.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ReferenceModule.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.ReferenceModule.Description))
                .ForMember(dest => dest.CreditPoints, opt => opt.MapFrom(src => src.ReferenceModule.CreditPoints))
                .ForMember(dest => dest.ModuleType, opt => opt.MapFrom(src => src.ReferenceModule.ModuleType))
                .ForMember(dest => dest.Professor, opt => opt.MapFrom((semesterModule, semesterModuleResource, i, context) =>
                 {
                     return semesterModule.Professor != null
                        ? context.Mapper.Map<User, UserResource>(semesterModule.Professor)
                        : null;
                 }));
            CreateMap<Semester, SemesterResource>();

            CreateMap<SemesterModule, IEnumerable<ExtendedLectureResource>>()
                .ConvertUsing<LectureConverter>();

            CreateMap<Token, TokenResource>()
                .ForMember(dest => dest.Token, opt => opt.MapFrom(src => src.Value));
        }
    }
}
