using AutoMapper;
using University.Server.Domain.Models;
using University.Server.Domain.Services;
using University.Server.Resources;

namespace University.Server.Mapping
{
    public class ResourceToModelProfile : Profile
    {
        public ResourceToModelProfile(IUserService userService)
        {
            CreateMap<SaveUserResource, User>();
            CreateMap<SaveModuleResource, Module>();
            CreateMap<SaveLocationResource, Location>();
            CreateMap<SaveSemesterResource, Semester>();
            CreateMap<SaveCourseResource, Course>();
            CreateMap<UpdateModuleResource, Module>();
                //.AfterMap((src, dest, ctx) =>
                //{
                //    dest.Professors = src.Professors.Select(async id => await userService.GetAsync(id))
                //                                    .Select(t => t.Result)
                //                                    .Where(i => i != null)
                //                                    .ToList();
                //});
        }
    }
}
