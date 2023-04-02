using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;
using University.Server.Domain.Models;
using University.Server.Domain.Persistence.Entities;
using University.Server.Domain.Repositories;
using University.Server.Exceptions;

namespace University.Server.Domain.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly ICosmosDbRepository<User, UserEntity> _userRepository;

        public UserService(ILogger<UserService> logger, ICosmosDbRepository<User, UserEntity> userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
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

        public async Task<User> SaveAsync(User user)
        {
            _logger.LogInformation("Attempting to save new user...");
            user.Password = Sha256Hash(user.Password);
            user.Assignments = new List<Assignment>();

            try
            {
                await _userRepository.AddItemAsync(user);

                return user;
            }
            catch (Microsoft.Azure.Cosmos.CosmosException ex)
            {
                throw RequestException.ResolveCosmosException(ex);
            }
            catch (Exception ex)
            {
                throw new InternalServerException($"An error occurred when saving the user: {ex.Message}");
            }
        }

        public async Task<IEnumerable<User>> GetManyAsync(IEnumerable<Guid> ids)
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

        public async Task<User> GetAsync(Guid id)
        {
            _logger.LogInformation("Attempting to retrieve existing user...");

            try
            {
                var user = await _userRepository.GetItemAsync(id);

                return user;
            }
            catch (Microsoft.Azure.Cosmos.CosmosException ex)
            {
                throw RequestException.ResolveCosmosException(ex, id);
            }
            catch (Exception ex)
            {
                throw new InternalServerException($"An error occurred when retrieving the user: {ex.Message}");
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

        public async Task<User> PatchAssignmentsAsync(Guid id, PatchModel<Module> patch)
        {
            var existingUser = await GetAsync(id);

            _logger.LogInformation($"Updating assignments of {existingUser.FirstName} {existingUser.LastName} ({existingUser.Id}).");

            _logger.LogInformation($"Initiating adding {patch.AddEntity.Count} assignments to user...");
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
                    _logger.LogInformation("User did not had assignment and added it now.");
                }
            }

            _logger.LogInformation($"Initiating removing {patch.RemoveEntity.Count} assignments from user...");
            foreach (var remove in patch.RemoveEntity)
            {
                if (existingUser.Assignments.Any(x => x.ReferenceModule.Id == remove.Id))
                {
                    var assignment = existingUser.Assignments.First(x => x.ReferenceModule.Id == remove.Id);
                    existingUser.Assignments.Remove(assignment);
                    _logger.LogInformation("Found assignment and removed it from user assignments.");
                }
            }

            try
            {
                await _userRepository.UpdateItemAsync(existingUser.Id, existingUser);

                return existingUser;
            }
            catch (Microsoft.Azure.Cosmos.CosmosException ex)
            {
                throw RequestException.ResolveCosmosException(ex, id);
            }
            catch (Exception ex)
            {
                throw new InternalServerException($"An error occurred when updating the user: {ex.Message}");
            }
        }

        public async Task<User> UpdateAsync(Guid id, User user)
        {
            _logger.LogInformation("Attempting to update existing user...");

            var existingUser = await GetAsync(id);

            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Email = user.Email;

            try
            {
                await _userRepository.UpdateItemAsync(existingUser.Id, existingUser);

                return existingUser;
            }
            catch (Microsoft.Azure.Cosmos.CosmosException ex)
            {
                throw RequestException.ResolveCosmosException(ex, id);
            }
            catch (Exception ex)
            {
                throw new InternalServerException($"An error occurred when updating the user: {ex.Message}");
            }
        }

        public async Task<User> UpdateCredentialsAsync(Guid id, User user)
        {
            _logger.LogInformation("Attempting to update existing user...");

            var existingUser = await GetAsync(id);

            existingUser.Password = Sha256Hash(user.Password);

            try
            {
                await _userRepository.UpdateItemAsync(existingUser.Id, existingUser);

                return existingUser;
            }
            catch (Microsoft.Azure.Cosmos.CosmosException ex)
            {
                throw RequestException.ResolveCosmosException(ex, id);
            }
            catch (Exception ex)
            {
                throw new InternalServerException($"An error occurred when updating the user: {ex.Message}");
            }
        }

        public async Task<User> UpdateAssignmentAsync(Guid userId, Guid moduleId, Assignment assignment)
        {
            _logger.LogInformation("Attempting to update existing user assignment...");

            var existingUser = await GetAsync(userId);

            var existingAssignment = existingUser.Assignments.FirstOrDefault(assignment => assignment.ReferenceModule.Id == moduleId, null);
            if (existingAssignment == null)
                throw new NotFoundException($"Assignment with module id '{moduleId}' not found");
            existingAssignment.Status = assignment.Status;

            try
            {
                await _userRepository.UpdateItemAsync(existingUser.Id, existingUser);

                return existingUser;
            }
            catch (Microsoft.Azure.Cosmos.CosmosException ex)
            {
                throw RequestException.ResolveCosmosException(ex, userId);
            }
            catch (Exception ex)
            {
                throw new InternalServerException($"An error occurred when updating the user assignment: {ex.Message}");
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            _logger.LogInformation("Attempting to delete existing user...");

            try
            {
                await _userRepository.DeleteItemAsync(id);
            }
            catch (Microsoft.Azure.Cosmos.CosmosException ex)
            {
                throw RequestException.ResolveCosmosException(ex, id);
            }
            catch (Exception ex)
            {
                throw new InternalServerException($"An error occurred when deleting the user: {ex.Message}");
            }
        }
    }
}