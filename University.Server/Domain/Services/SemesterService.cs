using University.Server.Domain.Models;
using University.Server.Domain.Persistence.Entities;
using University.Server.Domain.Repositories;
using University.Server.Domain.Services.Communication;

namespace University.Server.Domain.Services
{
    public class SemesterService : ISemesterService
    {
        private readonly ILogger<SemesterService> _logger;
        private readonly ICosmosDbRepository<Semester, SemesterEntity> _semesterRepository;

        public SemesterService(ILogger<SemesterService> logger, ICosmosDbRepository<Semester, SemesterEntity> semesterRepository)
        {
            _logger = logger;
            _semesterRepository = semesterRepository;
        }

        public async Task<Response<Semester>> SaveAsync(Semester semester)
        {
            _logger.LogInformation("Attempting to save new semester...");

            semester.Modules = new List<SemesterModule>();

            try
            {
                await _semesterRepository.AddItemAsync(semester);

                return new Response<Semester>(StatusCodes.Status201Created, semester);
            }
            catch (Microsoft.Azure.Cosmos.CosmosException ex)
            {
                return new Response<Semester>((int)ex.StatusCode, $"Cosmos DB raised an error when saving the semester: {ex.Message}");
            }
            catch (Exception ex)
            {
                return new Response<Semester>(StatusCodes.Status500InternalServerError, $"An error occurred when saving the semester: {ex.Message}");
            }
        }

        public async Task<Semester?> GetAsync(Guid id)
        {
            _logger.LogInformation("Attempting to retrieve existing semesters...");

            return await _semesterRepository.GetItemAsync(id);
        }

        public async Task<IEnumerable<Semester>> ListAsync()
        {
            _logger.LogInformation("Attempting to retrieve existing semester...");

            return await _semesterRepository.GetItemsAsync("SELECT * FROM c");
        }

        public async Task<Response<Semester>> PatchModulesAsync(Guid id, PatchModel<Module> patch)
        {
            var existingSemester = await _semesterRepository.GetItemAsync(id);

            foreach (var add in patch.AddEntity)
            {
                if (!existingSemester.Modules.Any(x => x.ReferenceModule.Id == add.Id))
                {
                    var semesterModule = new SemesterModule()
                    {
                        Id = Guid.NewGuid(),
                        Professor = null,
                        ReferenceModule = add,
                    };
                    existingSemester.Modules.Add(semesterModule);
                }
            }
            foreach (var remove in patch.RemoveEntity)
            {
                if (existingSemester.Modules.Any(x => x.Id == remove.Id))
                {
                    var semesterModule = existingSemester.Modules.First(x => x.ReferenceModule.Id == remove.Id);
                    existingSemester.Modules.Remove(semesterModule);
                }
            }

            try
            {
                await _semesterRepository.UpdateItemAsync(existingSemester.Id, existingSemester);

                return new Response<Semester>(StatusCodes.Status200OK, existingSemester);
            }
            catch (Microsoft.Azure.Cosmos.CosmosException ex)
            {
                return new Response<Semester>((int)ex.StatusCode, $"Cosmos DB raised an error when updating the semester: {ex.Message}");
            }
            catch (Exception ex)
            {
                return new Response<Semester>(StatusCodes.Status500InternalServerError, $"An error occurred when updating the semester: {ex.Message}");
            }
        }

        public async Task<Response<Semester>> DeleteAsync(Guid id)
        {
            _logger.LogInformation("Attempting to delete existing semester...");

            try
            {
                await _semesterRepository.DeleteItemAsync(id);

                return new Response<Semester>(StatusCodes.Status204NoContent);
            }
            catch (Microsoft.Azure.Cosmos.CosmosException ex)
            {
                return new Response<Semester>((int)ex.StatusCode, $"Cosmos DB raised an error when deleting the semester: {ex.Message}");
            }
            catch (Exception ex)
            {
                return new Response<Semester>(StatusCodes.Status500InternalServerError, $"An error occurred when deleting the semester: {ex.Message}");
            }
        }
    }
}
