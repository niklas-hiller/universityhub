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
        }
    }
}
