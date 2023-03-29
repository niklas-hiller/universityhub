using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;
using University.Server.Domain.Models;
using University.Server.Domain.Persistence.Entities;
using University.Server.Domain.Repositories;
using University.Server.Domain.Services.Communication;

namespace University.Server.Domain.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly ICosmosDbRepository<User, UserEntity> _userRepository;
        private readonly ISemesterService _semesterService;

        public UserService(ILogger<UserService> logger, ICosmosDbRepository<User, UserEntity> userRepository, ISemesterService semesterService)
        {
            _logger = logger;
            _userRepository = userRepository;
            _semesterService = semesterService;
        }

        private string Sha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public async Task<User?> GetUserByCredentials(string email, string password)
        {
            var users = await _userRepository.GetItemsAsync($"SELECT * FROM c WHERE c.Email = '{email}' AND c.Password = '{Sha256Hash(password)}'");
            return users.FirstOrDefault();
        }

        public async Task<Response<User>> SaveAsync(User user)
        {
            _logger.LogInformation("Attempting to save new user...");
            user.Password = Sha256Hash(user.Password);
            user.Assignments = new List<Assignment>();

            try
            {
                await _userRepository.AddItemAsync(user);

                return new Response<User>(StatusCodes.Status201Created, user);
            }
            catch (Microsoft.Azure.Cosmos.CosmosException ex)
            {
                return new Response<User>((int)ex.StatusCode, $"Cosmos DB raised an error when saving the user: {ex.Message}");
            }
            catch (Exception ex)
            {
                return new Response<User>(StatusCodes.Status500InternalServerError, $"An error occurred when saving the user: {ex.Message}");
            }
        }

        public async Task<IEnumerable<User>> GetManyAsync(ICollection<Guid> ids)
        {
            if (ids.IsNullOrEmpty())
            {
                return Enumerable.Empty<User>();
            }
            var query = $"SELECT * FROM c WHERE c.id IN ('{string.Join("', '", ids)}')";
            try
            {
                return await _userRepository.GetItemsAsync(query);
            }
            catch (Microsoft.Azure.Cosmos.CosmosException ex)
            {
                _logger.LogInformation($"Cosmos DB Exception for: {query})");
                throw ex;
            }
        }

        public async Task<IEnumerable<SemesterModule>> GetActiveSemesterModulesOfUser(Guid id)
        {
            var existingUser = await GetAsyncNullable(id);

            if (existingUser == null)
                return Enumerable.Empty<SemesterModule>();
            if (existingUser.Assignments.IsNullOrEmpty())
                return Enumerable.Empty<SemesterModule>();

            var activeAssignmentIds = existingUser.Assignments
                .Where(assignment => assignment.Status == EModuleStatus.Enrolled || assignment.Status == EModuleStatus.Educates)
                .Select(assignment => assignment.ReferenceModule.Id);

            var activeSemesters = await _semesterService.GetManyAsyncByTime(DateTime.Now, new TimeSpan(30, 0, 0, 0));
            if (activeSemesters.IsNullOrEmpty())
                return Enumerable.Empty<SemesterModule>();

            var semesterModules = activeSemesters
                .SelectMany(semester => semester.Modules)
                .Where(semesterModule => activeAssignmentIds.Contains(semesterModule.ReferenceModule.Id));

            return semesterModules;
        }

        public async Task<User?> GetAsyncNullable(Guid id)
        {
            try
            {
                var user = await _userRepository.GetItemAsync(id);

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<Response<User>> GetAsync(Guid id)
        {
            _logger.LogInformation("Attempting to retrieve existing user...");

            try
            {
                var user = await _userRepository.GetItemAsync(id);

                return new Response<User>(StatusCodes.Status200OK, user);
            }
            catch (Microsoft.Azure.Cosmos.CosmosException ex)
            {
                return new Response<User>((int)ex.StatusCode, $"Cosmos DB raised an error when retrieving the user: {ex.Message}");
            }
            catch (Exception ex)
            {
                return new Response<User>(StatusCodes.Status500InternalServerError, $"An error occurred when retrieving the user: {ex.Message}");
            }
        }

        public async Task<IEnumerable<User>> ListAsync(EAuthorization? authorization)
        {
            _logger.LogInformation("Attempting to retrieve existing users...");

            if (authorization != null)
            {
                return await _userRepository.GetItemsAsync($"SELECT * FROM c WHERE c.Authorization = {(byte)authorization}");
            }
            else
            {
                return await _userRepository.GetItemsAsync($"SELECT * FROM c");
            }
        }

        public async Task<Response<User>> PatchAssignmentsAsync(Guid id, PatchModel<Module> patch)
        {
            var existingUser = await GetAsyncNullable(id);

            if (existingUser == null)
                return new Response<User>(StatusCodes.Status404NotFound, "User not found.");

            foreach (var add in patch.AddEntity)
            {
                if (!existingUser.Assignments.Any(x => x.ReferenceModule.Id == add.Id))
                {
                    var assignment = new Assignment()
                    {
                        Id = Guid.NewGuid(),
                        Status = existingUser.Authorization == EAuthorization.Professor ? EModuleStatus.Educates : EModuleStatus.Enrolled,
                        ReferenceModule = add
                    };
                    existingUser.Assignments.Add(assignment);
                }
            }
            foreach (var remove in patch.RemoveEntity)
            {
                if (existingUser.Assignments.Any(x => x.ReferenceModule.Id == remove.Id))
                {
                    var assignment = existingUser.Assignments.First(x => x.ReferenceModule.Id == remove.Id);
                    existingUser.Assignments.Remove(assignment);
                }
            }

            try
            {
                await _userRepository.UpdateItemAsync(existingUser.Id, existingUser);

                return new Response<User>(StatusCodes.Status200OK, existingUser);
            }
            catch (Microsoft.Azure.Cosmos.CosmosException ex)
            {
                return new Response<User>((int)ex.StatusCode, $"Cosmos DB raised an error when updating the user: {ex.Message}");
            }
            catch (Exception ex)
            {
                return new Response<User>(StatusCodes.Status500InternalServerError, $"An error occurred when updating the user: {ex.Message}");
            }
        }

        public async Task<Response<User>> UpdateAsync(Guid id, User user)
        {
            _logger.LogInformation("Attempting to update existing user...");

            var existingUser = await GetAsyncNullable(id);

            if (existingUser == null)
                return new Response<User>(StatusCodes.Status404NotFound, "User not found.");

            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Email = user.Email;

            try
            {
                await _userRepository.UpdateItemAsync(existingUser.Id, existingUser);

                return new Response<User>(StatusCodes.Status200OK, existingUser);
            }
            catch (Microsoft.Azure.Cosmos.CosmosException ex)
            {
                return new Response<User>((int)ex.StatusCode, $"Cosmos DB raised an error when updating the user: {ex.Message}");
            }
            catch (Exception ex)
            {
                return new Response<User>(StatusCodes.Status500InternalServerError, $"An error occurred when updating the user: {ex.Message}");
            }
        }

        public async Task<Response<User>> UpdateCredentialsAsync(Guid id, User user)
        {
            _logger.LogInformation("Attempting to update existing user...");

            var existingUser = await GetAsyncNullable(id);

            if (existingUser == null)
                return new Response<User>(StatusCodes.Status404NotFound, "User not found.");

            existingUser.Password = Sha256Hash(user.Password);

            try
            {
                await _userRepository.UpdateItemAsync(existingUser.Id, existingUser);

                return new Response<User>(StatusCodes.Status200OK, existingUser);
            }
            catch (Microsoft.Azure.Cosmos.CosmosException ex)
            {
                return new Response<User>((int)ex.StatusCode, $"Cosmos DB raised an error when updating the user: {ex.Message}");
            }
            catch (Exception ex)
            {
                return new Response<User>(StatusCodes.Status500InternalServerError, $"An error occurred when updating the user: {ex.Message}");
            }
        }

        public async Task<Response<User>> UpdateAssignmentAsync(Guid userId, Guid moduleId, Assignment assignment)
        {
            _logger.LogInformation("Attempting to update existing user assignment...");

            var existingUser = await GetAsyncNullable(userId);

            if (existingUser == null)
                return new Response<User>(StatusCodes.Status404NotFound, "User not found.");

            var existingAssignment = existingUser.Assignments.FirstOrDefault(assignment => assignment.ReferenceModule.Id == moduleId, null);
            if (existingAssignment == null)
                return new Response<User>(StatusCodes.Status404NotFound, "Assignment not found.");
            existingAssignment.Status = assignment.Status;

            try
            {
                await _userRepository.UpdateItemAsync(existingUser.Id, existingUser);

                return new Response<User>(StatusCodes.Status200OK, existingUser);
            }
            catch (Microsoft.Azure.Cosmos.CosmosException ex)
            {
                return new Response<User>((int)ex.StatusCode, $"Cosmos DB raised an error when updating the user assignment: {ex.Message}");
            }
            catch (Exception ex)
            {
                return new Response<User>(StatusCodes.Status500InternalServerError, $"An error occurred when updating the user assignment: {ex.Message}");
            }
        }

        public async Task<Response<User>> DeleteAsync(Guid id)
        {
            _logger.LogInformation("Attempting to delete existing user...");

            try
            {
                await _userRepository.DeleteItemAsync(id);

                return new Response<User>(StatusCodes.Status204NoContent);
            }
            catch (Microsoft.Azure.Cosmos.CosmosException ex)
            {
                return new Response<User>((int)ex.StatusCode, $"Cosmos DB raised an error when deleting the user: {ex.Message}");
            }
            catch (Exception ex)
            {
                return new Response<User>(StatusCodes.Status500InternalServerError, $"An error occurred when deleting the user: {ex.Message}");
            }
        }
    }
}
