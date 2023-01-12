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
            CreateMap<Location, LocationResource>();
        }
    }
}
