using University.Server.Domain.Models;
using University.Server.Domain.Persistence.Repositories;
using University.Server.Domain.Repositories;
using University.Server.Domain.Services.Communication;

namespace University.Server.Domain.Services
{
    public class CourseService : ICourseService
    {
        private readonly ILogger<CourseService> _logger;
        private readonly ICourseRepository _courseRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CourseService(ILogger<CourseService> logger, ICourseRepository courseRepository, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _courseRepository = courseRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<CourseResponse> SaveAsync(Course course)
        {
            try
            {
                await _courseRepository.AddAsync(course);
                await _unitOfWork.CompleteAsync();

                return new CourseResponse(course);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new CourseResponse($"An error occurred when saving the course: {ex.Message}");
            }
        }

        public async Task<IEnumerable<Course>> ListAsync()
        {
            return await _courseRepository.ListAsync();
        }

        public async Task<Course?> GetAsync(Guid id)
        {
            return await _courseRepository.GetAsync(id);
        }

        public async Task<CourseResponse> UpdateAsync(Guid id, Course course)
        {
            var existingCourse = await _courseRepository.GetAsync(id);

            if (existingCourse == null)
                return new CourseResponse("Course not found.");

            if (!String.IsNullOrEmpty(course.Name))
            {
                existingCourse.Name = course.Name;
            }

            try
            {
                _courseRepository.Update(existingCourse);
                await _unitOfWork.CompleteAsync();

                return new CourseResponse(existingCourse);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new CourseResponse($"An error occurred when updating the course: {ex.Message}");
            }
        }

        public async Task<CourseResponse> OverwriteAsync(Guid id, Course course)
        {
            var existingCourse = await _courseRepository.GetAsync(id);

            if (existingCourse == null)
                return new CourseResponse("Course not found.");

            existingCourse.Students = course.Students;
            existingCourse.Modules = course.Modules;

            try
            {
                _courseRepository.Update(existingCourse);
                await _unitOfWork.CompleteAsync();

                return new CourseResponse(existingCourse);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new CourseResponse($"An error occurred when overwriting the course: {ex.Message}");
            }
        }

        public async Task<CourseResponse> DeleteAsync(Guid id)
        {
            var existingCourse = await _courseRepository.GetAsync(id);

            if (existingCourse == null)
                return new CourseResponse("Location not found.");

            try
            {
                _courseRepository.Remove(existingCourse);
                await _unitOfWork.CompleteAsync();

                return new CourseResponse(existingCourse);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new CourseResponse($"An error occurred when deleting the course: {ex.Message}");
            }
        }
    }
}
