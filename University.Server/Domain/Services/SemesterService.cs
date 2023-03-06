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

        public async Task<SemesterResponse> SaveAsync(Semester semester)
        {
            try
            {
                await _semesterRepository.AddItemAsync(semester);

                return new SemesterResponse(semester);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new SemesterResponse($"An error occurred when saving the semester: {ex.Message}");
            }
        }

        public async Task<IEnumerable<Semester>> ListAsync()
        {
            return await _semesterRepository.GetItemsAsync("SELECT * FROM c");
        }

        public async Task<Semester?> GetAsync(Guid id)
        {
            return await _semesterRepository.GetItemAsync(id);
        }

        public async Task<SemesterResponse> DeleteAsync(Guid id)
        {
            var existingSemester = await _semesterRepository.GetItemAsync(id);

            if (existingSemester == null)
                return new SemesterResponse("User not found.");

            try
            {
                await _semesterRepository.DeleteItemAsync(existingSemester.Id);

                return new SemesterResponse(existingSemester);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new SemesterResponse($"An error occurred when deleting the semester: {ex.Message}");
            }
        }
    }
}
