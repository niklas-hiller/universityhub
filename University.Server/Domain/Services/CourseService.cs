using Microsoft.IdentityModel.Tokens;
using University.Server.Domain.Models;
using University.Server.Domain.Persistence.Entities;
using University.Server.Domain.Repositories;
using University.Server.Exceptions;

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

        public async Task<Course> SaveAsync(Course course)
        {
            _logger.LogInformation("Attempting to save new course...");

            course.Students = new List<User>();
            course.Modules = new List<Module>();

            try
            {
                await _courseRepository.AddItemAsync(course);

                return course;
            }
            catch (Microsoft.Azure.Cosmos.CosmosException ex)
            {
                throw RequestException.ResolveCosmosException(ex);
            }
            catch (Exception ex)
            {
                throw new InternalServerException($"An error occurred when saving the course: {ex.Message}");
            }
        }

        public async Task<IEnumerable<Course>> GetManyAsync(IEnumerable<Guid> ids)
        {
            if (ids.IsNullOrEmpty())
            {
                return Enumerable.Empty<Course>();
            }
            var query = $"SELECT * FROM c WHERE c.id IN ('{string.Join("', '", ids)}')";
            try
            {
                return await _courseRepository.GetItemsAsync(query);
            }
            catch (Microsoft.Azure.Cosmos.CosmosException ex)
            {
                _logger.LogInformation($"Cosmos DB Exception for: {query})");
                throw ex;
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

        public async Task<Course> GetAsync(Guid id)
        {
            _logger.LogInformation("Attempting to retrieve existing course...");

            try
            {
                var course = await _courseRepository.GetItemAsync(id);

                return course;
            }
            catch (Microsoft.Azure.Cosmos.CosmosException ex)
            {
                throw RequestException.ResolveCosmosException(ex, id);
            }
            catch (Exception ex)
            {
                throw new InternalServerException($"An error occurred when retrieving the course: {ex.Message}");
            }
        }

        public async Task<IEnumerable<Course>> ListAsync()
        {
            _logger.LogInformation("Attempting to retrieve existing courses...");

            return await _courseRepository.GetItemsAsync("SELECT * FROM c");
        }

        public async Task<Course> PatchStudentsAsync(Guid id, PatchModel<User> patch)
        {
            foreach (var user in patch.AddEntity.Union(patch.RemoveEntity))
            {
                if (user.Authorization != EAuthorization.Student)
                {
                    throw new BadRequestException("You can't add non-student as users to a module.");
                }
            }

            var existingCourse = await GetAsync(id);

            _logger.LogInformation($"Initiating adding {patch.AddEntity.Count} students to course...");
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

                        await _userService.PatchAssignmentsAsync(add.Id, patchModules);
                    }

                    #endregion User Assignment Logic

                    existingCourse.Students.Add(add);
                }
            }

            _logger.LogInformation($"Initiating removing {patch.RemoveEntity.Count} students from course...");
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

                        await _userService.PatchAssignmentsAsync(remove.Id, patchModules);
                    }

                    #endregion User Assignment Logic

                    var student = existingCourse.Students.First(x => x.Id == remove.Id);
                    existingCourse.Students.Remove(student);
                }
            }

            try
            {
                await _courseRepository.UpdateItemAsync(existingCourse.Id, existingCourse);

                return existingCourse;
            }
            catch (Microsoft.Azure.Cosmos.CosmosException ex)
            {
                throw RequestException.ResolveCosmosException(ex, id);
            }
            catch (Exception ex)
            {
                throw new InternalServerException($"An error occurred when updating the course: {ex.Message}");
            }
        }

        public async Task<Course> PatchModulesAsync(Guid id, PatchModel<Module> patch)
        {
            foreach (var user in patch.AddEntity.Union(patch.RemoveEntity))
            {
                if (user.ModuleType == EModuleType.Optional)
                {
                    throw new BadRequestException("You can't add optional modules to a course.");
                }
            }

            var existingCourse = await GetAsync(id);

            _logger.LogInformation($"Initiating adding {patch.AddEntity.Count} modules to course...");
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
                            await _userService.PatchAssignmentsAsync(user.Id, patchModules);
                        }
                    }

                    #endregion User Assignment Logic

                    existingCourse.Modules.Add(add);
                }
            }

            _logger.LogInformation($"Initiating removing {patch.RemoveEntity.Count} modules from course...");
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
                            await _userService.PatchAssignmentsAsync(user.Id, patchModules);
                        }
                    }

                    #endregion User Assignment Logic

                    var module = existingCourse.Modules.First(x => x.Id == remove.Id);
                    existingCourse.Modules.Remove(module);
                }
            }

            try
            {
                await _courseRepository.UpdateItemAsync(existingCourse.Id, existingCourse);

                return existingCourse;
            }
            catch (Microsoft.Azure.Cosmos.CosmosException ex)
            {
                throw RequestException.ResolveCosmosException(ex, id);
            }
            catch (Exception ex)
            {
                throw new InternalServerException($"An error occurred when updating the course: {ex.Message}");
            }
        }

        public async Task<Course> UpdateAsync(Guid id, Course course)
        {
            _logger.LogInformation("Attempting to update existing course...");

            var existingCourse = await GetAsync(id);

            existingCourse.Description = course.Description;

            try
            {
                await _courseRepository.UpdateItemAsync(existingCourse.Id, existingCourse);

                return existingCourse;
            }
            catch (Microsoft.Azure.Cosmos.CosmosException ex)
            {
                throw RequestException.ResolveCosmosException(ex, id);
            }
            catch (Exception ex)
            {
                throw new InternalServerException($"An error occurred when updating the course: {ex.Message}");
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            _logger.LogInformation("Attempting to delete existing course...");

            try
            {
                await _courseRepository.DeleteItemAsync(id);
            }
            catch (Microsoft.Azure.Cosmos.CosmosException ex)
            {
                throw RequestException.ResolveCosmosException(ex, id);
            }
            catch (Exception ex)
            {
                throw new InternalServerException($"An error occurred when deleting the course: {ex.Message}");
            }
        }
    }
}