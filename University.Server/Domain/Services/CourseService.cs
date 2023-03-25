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

            course.Students = new List<User>();
            course.Modules = new List<Module>();

            try
            {
                await _courseRepository.AddItemAsync(course);

                return new Response<Course>(StatusCodes.Status201Created, course);
            }
            catch (Microsoft.Azure.Cosmos.CosmosException ex)
            {
                return new Response<Course>((int)ex.StatusCode, $"Cosmos DB raised an error when saving the course: {ex.Message}");
            }
            catch (Exception ex)
            {
                return new Response<Course>(StatusCodes.Status500InternalServerError, $"An error occurred when saving the course: {ex.Message}");
            }
        }

        public async Task<Course?> GetAsyncNullable(Guid id)
        {
            try
            {
                var course = await _courseRepository.GetItemAsync(id);

                return course;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<Response<Course>> GetAsync(Guid id)
        {
            _logger.LogInformation("Attempting to retrieve existing course...");

            try
            {
                var course = await _courseRepository.GetItemAsync(id);

                return new Response<Course>(StatusCodes.Status200OK, course);
            }
            catch (Microsoft.Azure.Cosmos.CosmosException ex)
            {
                return new Response<Course>((int)ex.StatusCode, $"Cosmos DB raised an error when retrieving the course: {ex.Message}");
            }
            catch (Exception ex)
            {
                return new Response<Course>(StatusCodes.Status500InternalServerError, $"An error occurred when retrieving the course: {ex.Message}");
            }
        }

        public async Task<IEnumerable<Course>> ListAsync()
        {
            _logger.LogInformation("Attempting to retrieve existing courses...");

            return await _courseRepository.GetItemsAsync("SELECT * FROM c");
        }

        public async Task<Response<Course>> PatchStudentsAsync(Guid id, PatchModel<User> patch)
        {
            foreach (var user in patch.AddEntity.Union(patch.RemoveEntity))
            {
                if (user.Authorization != EAuthorization.Student)
                {
                    return new Response<Course>(StatusCodes.Status400BadRequest, "You can't add non-student as users to a module.");
                }
            }

            var existingCourse = await GetAsyncNullable(id);

            if (existingCourse == null)
                return new Response<Course>(StatusCodes.Status404NotFound, "Course not found.");

            foreach (var add in patch.AddEntity)
            {
                if (!existingCourse.Students.Any(x => x.Id == add.Id))
                {
                    #region User Assignment Logic
                    {
                        var patchModules = new PatchModel<Module>();
                        foreach (var module in existingCourse.Modules)
                        {
                            patchModules.AddEntity.Add(module);
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
            foreach (var remove in patch.RemoveEntity)
            {
                if (existingCourse.Students.Any(x => x.Id == remove.Id))
                {
                    #region User Assignment Logic
                    {
                        var patchModules = new PatchModel<Module>();
                        foreach (var module in existingCourse.Modules)
                        {
                            patchModules.RemoveEntity.Add(module);
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
            catch (Microsoft.Azure.Cosmos.CosmosException ex)
            {
                return new Response<Course>((int)ex.StatusCode, $"Cosmos DB raised an error when updating the course: {ex.Message}");
            }
            catch (Exception ex)
            {
                return new Response<Course>(StatusCodes.Status500InternalServerError, $"An error occurred when updating the course: {ex.Message}");
            }
        }

        public async Task<Response<Course>> PatchModulesAsync(Guid id, PatchModel<Module> patch)
        {
            foreach (var user in patch.AddEntity.Union(patch.RemoveEntity))
            {
                if (user.ModuleType == EModuleType.Optional)
                {
                    return new Response<Course>(StatusCodes.Status400BadRequest, "You can't add optional modules to a course.");
                }
            }

            var existingCourse = await GetAsyncNullable(id);

            if (existingCourse == null)
                return new Response<Course>(StatusCodes.Status404NotFound, "Course not found.");

            foreach (var add in patch.AddEntity)
            {
                if (!existingCourse.Modules.Any(x => x.Id == add.Id))
                {
                    #region User Assignment Logic
                    {
                        foreach (var user in existingCourse.Students)
                        {
                            var patchModules = new PatchModel<Module>();
                            patchModules.AddEntity.Add(add);
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
            foreach (var remove in patch.RemoveEntity)
            {
                if (existingCourse.Modules.Any(x => x.Id == remove.Id))
                {
                    #region User Assignment Logic
                    {
                        foreach (var user in existingCourse.Students)
                        {
                            var patchModules = new PatchModel<Module>();
                            patchModules.RemoveEntity.Add(remove);
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
            catch (Microsoft.Azure.Cosmos.CosmosException ex)
            {
                return new Response<Course>((int)ex.StatusCode, $"Cosmos DB raised an error when updating the course: {ex.Message}");
            }
            catch (Exception ex)
            {
                return new Response<Course>(StatusCodes.Status500InternalServerError, $"An error occurred when updating the course: {ex.Message}");
            }
        }

        public async Task<Response<Course>> UpdateAsync(Guid id, Course course)
        {
            _logger.LogInformation("Attempting to update existing course...");

            var existingCourse = await GetAsyncNullable(id);

            if (existingCourse == null)
                return new Response<Course>(StatusCodes.Status404NotFound, "Course not found.");

            existingCourse.Description = course.Description;

            try
            {
                await _courseRepository.UpdateItemAsync(existingCourse.Id, existingCourse);

                return new Response<Course>(StatusCodes.Status200OK, existingCourse);
            }
            catch (Microsoft.Azure.Cosmos.CosmosException ex)
            {
                return new Response<Course>((int)ex.StatusCode, $"Cosmos DB raised an error when updating the course: {ex.Message}");
            }
            catch (Exception ex)
            {
                return new Response<Course>(StatusCodes.Status500InternalServerError, $"An error occurred when updating the course: {ex.Message}");
            }
        }

        public async Task<Response<Course>> DeleteAsync(Guid id)
        {
            _logger.LogInformation("Attempting to delete existing course...");

            try
            {
                await _courseRepository.DeleteItemAsync(id);

                return new Response<Course>(StatusCodes.Status204NoContent);
            }
            catch (Microsoft.Azure.Cosmos.CosmosException ex)
            {
                return new Response<Course>((int)ex.StatusCode, $"Cosmos DB raised an error when deleting the course: {ex.Message}");
            }
            catch (Exception ex)
            {
                return new Response<Course>(StatusCodes.Status500InternalServerError, $"An error occurred when deleting the course: {ex.Message}");
            }
        }
    }
}
