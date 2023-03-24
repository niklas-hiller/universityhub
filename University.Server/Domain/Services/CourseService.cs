using University.Server.Domain.Models;
using University.Server.Domain.Persistence.Entities;
using University.Server.Domain.Repositories;
using University.Server.Domain.Services.Communication;
using University.Server.Resources;

namespace University.Server.Domain.Services
{
    public class CourseService : ICourseService
    {
        private readonly ILogger<CourseService> _logger;
        private readonly ICosmosDbRepository<Course, CourseEntity> _courseRepository;
        private readonly IUserService _userService;

        public CourseService(ILogger<CourseService> logger, ICosmosDbRepository<Course, CourseEntity> courseRepository, IUserService userService)
        {
            _logger = logger;
            _courseRepository = courseRepository;
            _userService = userService;
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

        public async Task<Response<Course>> PatchStudentsAsync(Guid id, PatchModel<User> patch)
        {
            var existingCourse = await _courseRepository.GetItemAsync(id);

            foreach(var add in patch.Add)
            {
                if (!existingCourse.Students.Any(x => x.Id == add.Id))
                {
                    #region User Assignment Logic
                    {
                        var patchModules = new PatchModel<Module>();
                        foreach (var module in existingCourse.Modules)
                        {
                            patchModules.Add.Add(module);
                        }

                        var result = await _userService.PatchAssignmentsAsync(add.Id, patchModules);
                        if (result.StatusCode != StatusCodes.Status200OK)
                        {
                            return new Response<Course>(StatusCodes.Status400BadRequest, $"An error occurred when updating the course: {result.Message}");
                        }
                    }
                    #endregion

                    existingCourse.Students.Add(add);
                }
            }
            foreach (var remove in patch.Remove)
            {
                if (existingCourse.Students.Any(x => x.Id == remove.Id))
                {
                    #region User Assignment Logic
                    {
                        var patchModules = new PatchModel<Module>();
                        foreach (var module in existingCourse.Modules)
                        {
                            patchModules.Remove.Add(module);
                        }

                        var result = await _userService.PatchAssignmentsAsync(remove.Id, patchModules);
                        if (result.StatusCode != StatusCodes.Status200OK)
                        {
                            return new Response<Course>(StatusCodes.Status400BadRequest, $"An error occurred when updating the course: {result.Message}");
                        }
                    }
                    #endregion

                    existingCourse.Students.Remove(remove);
                }
            }

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

        public async Task<Response<Course>> PatchModulesAsync(Guid id, PatchModel<Module> patch)
        {
            var existingCourse = await _courseRepository.GetItemAsync(id);

            foreach (var add in patch.Add)
            {
                if (!existingCourse.Modules.Any(x => x.Id == add.Id))
                {
                    #region User Assignment Logic
                    {
                        foreach (var user in existingCourse.Students)
                        {
                            var patchModules = new PatchModel<Module>();
                            patchModules.Add.Add(add);
                            var result = await _userService.PatchAssignmentsAsync(user.Id, patchModules);
                            if (result.StatusCode != StatusCodes.Status200OK)
                            {
                                return new Response<Course>(StatusCodes.Status400BadRequest, $"An error occurred when updating the course: {result.Message}");
                            }
                        }
                    }
                    #endregion

                    existingCourse.Modules.Add(add);
                }
            }
            foreach (var remove in patch.Remove)
            {
                if (existingCourse.Modules.Any(x => x.Id == remove.Id))
                {
                    #region User Assignment Logic
                    {
                        foreach (var user in existingCourse.Students)
                        {
                            var patchModules = new PatchModel<Module>();
                            patchModules.Remove.Add(remove);
                            var result = await _userService.PatchAssignmentsAsync(user.Id, patchModules);
                            if (result.StatusCode != StatusCodes.Status200OK)
                            {
                                return new Response<Course>(StatusCodes.Status400BadRequest, $"An error occurred when updating the course: {result.Message}");
                            }
                        }
                    }
                    #endregion

                    existingCourse.Modules.Remove(remove);
                }
            }

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
