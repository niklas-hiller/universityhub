using Microsoft.IdentityModel.Tokens;
using University.Server.Domain.Models;
using University.Server.Domain.Persistence.Entities;
using University.Server.Domain.Repositories;
using University.Server.Exceptions;

namespace University.Server.Domain.Services
{
    public class ModuleService : IModuleService
    {
        private readonly ILogger<ModuleService> _logger;
        private readonly ICosmosDbRepository<Module, ModuleEntity> _moduleRepository;
        private readonly IUserService _userService;

        public ModuleService(ILogger<ModuleService> logger, ICosmosDbRepository<Module, ModuleEntity> moduleRepository, IUserService userService)
        {
            _logger = logger;
            _moduleRepository = moduleRepository;
            _userService = userService;
        }

        public async Task<Module> SaveAsync(Module module)
        {
            _logger.LogInformation("Attempting to save new module...");

            module.ProfessorIds = new List<Guid>();

            try
            {
                await _moduleRepository.AddItemAsync(module);

                return module;
            }
            catch (Microsoft.Azure.Cosmos.CosmosException ex)
            {
                throw RequestException.ResolveCosmosException(ex);
            }
            catch (Exception ex)
            {
                throw new InternalServerException($"An error occurred when saving the module: {ex.Message}");
            }
        }

        public async Task<IEnumerable<Module>> GetManyAsync(ICollection<Guid> ids)
        {
            if (ids.IsNullOrEmpty())
            {
                return Enumerable.Empty<Module>();
            }
            var query = $"SELECT * FROM c WHERE c.id IN ('{string.Join("', '", ids)}')";
            try
            {
                return await _moduleRepository.GetItemsAsync(query);
            }
            catch (Microsoft.Azure.Cosmos.CosmosException ex)
            {
                _logger.LogInformation($"Cosmos DB Exception for: SELECT * FROM c WHERE c.id IN ({string.Join(", ", ids)})");
                throw ex;
            }
        }

        public async Task<Module?> GetAsyncNullable(Guid id, bool excludeArchived = true)
        {
            try
            {
                var module = await _moduleRepository.GetItemAsync(id);

                if (module.IsArchived && excludeArchived)
                {
                    return null;
                }

                return module;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<Module> GetAsync(Guid id)
        {
            _logger.LogInformation("Attempting to retrieve existing module...");

            try
            {
                var module = await _moduleRepository.GetItemAsync(id);

                if (module.IsArchived)
                {
                    throw new NotFoundException("Module not found.");
                }

                return module;
            }
            catch (Microsoft.Azure.Cosmos.CosmosException ex)
            {
                throw RequestException.ResolveCosmosException(ex, id);
            }
            catch (Exception ex)
            {
                throw new InternalServerException($"An error occurred when retrieving the module: {ex.Message}");
            }
        }

        public async Task<IEnumerable<Module>> ListAsync(EModuleType? moduleType)
        {
            _logger.LogInformation($"Attempting to retrieve existing modules...");

            if (moduleType != null)
            {
                return await _moduleRepository.GetItemsAsync($"SELECT * FROM c WHERE c.ModuleType = {(byte)moduleType} AND c.IsArchived = false");
            }
            else
            {
                _logger.LogInformation("No Module Type specified, retrieving all modules");
                return await _moduleRepository.GetItemsAsync("SELECT * FROM c WHERE c.IsArchived = false");
            }
        }

        public async Task<Module> PatchProfessorsAsync(Guid id, PatchModel<User> patch)
        {
            foreach (var user in patch.AddEntity.Union(patch.RemoveEntity))
            {
                if (user.Authorization != EAuthorization.Professor)
                {
                    throw new BadRequestException("You can't add non-professors as users to a module.");
                }
            }

            var existingModule = await GetAsync(id);

            foreach (var add in patch.AddEntity)
            {
                if (!existingModule.ProfessorIds.Contains(add.Id))
                {
                    #region User Assignment Logic
                    {
                        var patchModules = new PatchModel<Module>();
                        patchModules.AddEntity.Add(existingModule);
                        await _userService.PatchAssignmentsAsync(add.Id, patchModules);
                    }
                    #endregion

                    existingModule.ProfessorIds.Add(add.Id);
                }
            }
            foreach (var remove in patch.RemoveEntity)
            {
                if (existingModule.ProfessorIds.Contains(remove.Id))
                {
                    #region User Assignment Logic
                    {
                        var patchModules = new PatchModel<Module>();
                        patchModules.RemoveEntity.Add(existingModule);
                        await _userService.PatchAssignmentsAsync(remove.Id, patchModules);
                    }
                    #endregion

                    existingModule.ProfessorIds.Remove(remove.Id);
                }
            }

            try
            {
                await _moduleRepository.UpdateItemAsync(existingModule.Id, existingModule);

                return existingModule;
            }
            catch (Microsoft.Azure.Cosmos.CosmosException ex)
            {
                throw RequestException.ResolveCosmosException(ex, id);
            }
            catch (Exception ex)
            {
                throw new InternalServerException($"An error occurred when updating the module: {ex.Message}");
            }
        }

        public async Task<Module> UpdateAsync(Guid id, Module module)
        {
            _logger.LogInformation("Attempting to update existing module...");

            var existingModule = await GetAsync(id);

            existingModule.Name = module.Name;
            existingModule.Description = module.Description;
            existingModule.CreditPoints = module.CreditPoints;
            existingModule.MaxSize = module.MaxSize;

            try
            {
                await _moduleRepository.UpdateItemAsync(existingModule.Id, existingModule);

                return existingModule;
            }
            catch (Microsoft.Azure.Cosmos.CosmosException ex)
            {
                throw RequestException.ResolveCosmosException(ex, id);
            }
            catch (Exception ex)
            {
                throw new InternalServerException($"An error occurred when updating the module: {ex.Message}");
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            _logger.LogInformation("Attempting to delete existing module...");

            var existingModule = await GetAsync(id);

            try
            {
                // Modules are not deleted, but archived instead.
                existingModule.IsArchived = true;
                await _moduleRepository.UpdateItemAsync(existingModule.Id, existingModule);
            }
            catch (Microsoft.Azure.Cosmos.CosmosException ex)
            {
                throw RequestException.ResolveCosmosException(ex, id);
            }
            catch (Exception ex)
            {
                throw new InternalServerException($"An error occurred when deleting the module: {ex.Message}");
            }
        }
    }
}
