using AutoMapper;
using University.Server.Domain.Models;
using University.Server.Resources;

namespace University.Server.Mapping
{
    public class ModelToResourceProfile : Profile
    {
        public ModelToResourceProfile()
        {
            CreateMap<User, UserResource>();
            CreateMap<Module, ModuleResource>();
                //.ForMember(dest => dest.Professors, opt => opt.MapFrom(src => src.Professors.Select(m => m.Id)));
            CreateMap<Location, LocationResource>();
            CreateMap<Semester, SemesterResource>();
            CreateMap<Course, CourseResource>();
        }
    }
}
