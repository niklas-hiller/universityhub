using AutoMapper;
using University.Server.Domain.Models;
using University.Server.Resources;

namespace University.Server.Mapping
{
    public class ResourceToModelProfile : Profile
    {
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
            CreateMap<UpdateAssignmentResource, Assignment>();
        }
    }
}
