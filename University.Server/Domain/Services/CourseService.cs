using University.Server.Domain.Models;
using University.Server.Domain.Persistence.Entities;
using University.Server.Domain.Repositories;
using University.Server.Domain.Services.Communication;

namespace University.Server.Domain.Services
{
    public class CourseService : ICourseService
    {
        private readonly ILogger<CourseService> _logger;
        private readonly ICosmosDbRepository<Course, CourseEntity> _courseRepository;

        public CourseService(ILogger<CourseService> logger, ICosmosDbRepository<Course, CourseEntity> courseRepository)
        {
            _logger = logger;
            _courseRepository = courseRepository;
        }

        public async Task<Response<Course>> SaveAsync(Course course)
        {
            _logger.LogInformation("Attempting to save new course...");
            try
            {
                await _courseRepository.AddItemAsync(course);

                return new Response<Course>(StatusCodes.Status201Created, course);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new Response<Course>(StatusCodes.Status400BadRequest, $"An error occurred when saving the course: {ex.Message}");
            }
        }

        public async Task<Course?> GetAsync(Guid id)
        {
            _logger.LogInformation("Attempting to retrieve existing course...");

            return await _courseRepository.GetItemAsync(id);
        }

        public async Task<IEnumerable<Course>> ListAsync()
        {
            _logger.LogInformation("Attempting to retrieve existing courses...");

            return await _courseRepository.GetItemsAsync("SELECT * FROM c");
        }

        public async Task<Response<Course>> UpdateAsync(Guid id, Course course)
        {
            _logger.LogInformation("Attempting to update existing course...");

            var existingCourse = await _courseRepository.GetItemAsync(id);

            if (existingCourse == null)
                return new Response<Course>(StatusCodes.Status404NotFound, "Course not found.");

            existingCourse.Description = course.Description;

            try
            {
                await _courseRepository.UpdateItemAsync(existingCourse.Id, existingCourse);

                return new Response<Course>(StatusCodes.Status200OK, existingCourse);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new Response<Course>(StatusCodes.Status400BadRequest, $"An error occurred when updating the course: {ex.Message}");
            }
        }

        public async Task<Response<Course>> DeleteAsync(Guid id)
        {
            _logger.LogInformation("Attempting to delete existing course...");

            var existingCourse = await _courseRepository.GetItemAsync(id);

            if (existingCourse == null)
                return new Response<Course>(StatusCodes.Status404NotFound, "Course not found.");

            try
            {
                await _courseRepository.DeleteItemAsync(existingCourse.Id);

                return new Response<Course>(StatusCodes.Status204NoContent);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new Response<Course>(StatusCodes.Status400BadRequest, $"An error occurred when deleting the course: {ex.Message}");
            }
        }
    }
}
