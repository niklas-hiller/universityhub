using AutoMapper;
using University.Server.Domain.Models;
using University.Server.Domain.Persistence.Entities;
using University.Server.Domain.Services;

namespace University.Server.Mapping.Actions
{
    public class ResolveLectureRelationsAction : IMappingAction<LectureEntity, Lecture>
    {
        private readonly ILocationService _locationService;

        public ResolveLectureRelationsAction(ILocationService locationService)
        {
            _locationService = locationService ?? throw new ArgumentNullException(nameof(locationService));
        }

        public void Process(LectureEntity source, Lecture destination, ResolutionContext context)
        {
            destination.Location = _locationService.GetAsync(source.LocationId, false).GetAwaiter().GetResult();
        }
    }
}