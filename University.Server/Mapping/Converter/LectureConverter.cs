using AutoMapper;
using University.Server.Domain.Models;
using University.Server.Resources.Response;

namespace University.Server.Mapping.Converter
{
    public class LectureConverter : ITypeConverter<SemesterModule, IEnumerable<ExtendedLectureResource>>
    {
        public IEnumerable<ExtendedLectureResource> Convert(SemesterModule source, IEnumerable<ExtendedLectureResource> destination, ResolutionContext context)
        {
            foreach (LectureResource lecture in source.Lectures.Select(e => context.Mapper.Map<LectureResource>(e)))
            {
                var model = new ExtendedLectureResource()
                {
                    ModuleName = source.ReferenceModule.Name,
                    Professor = context.Mapper.Map<UserResource>(source.Professor),
                    Location = lecture.Location,
                    Duration = lecture.Duration,
                    Date = lecture.Date
                };
                yield return model;
            }
        }
    }
}
