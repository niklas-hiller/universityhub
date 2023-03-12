using University.Server.Domain.Models;
using University.Server.Domain.Persistence.Entities;
using University.Server.Domain.Repositories;
using University.Server.Domain.Services.Communication;

namespace University.Server.Domain.Services
{
    public class ModuleService : IModuleService
    {
        private readonly ILogger<ModuleService> _logger;
        private readonly ICosmosDbRepository<Module, ModuleEntity> _moduleRepository;

        public ModuleService(ILogger<ModuleService> logger, ICosmosDbRepository<Module, ModuleEntity> moduleRepository)
        {
            _logger = logger;
            _moduleRepository = moduleRepository;
        }

        public async Task<Response<Module>> SaveAsync(Module module)
        {
            _logger.LogInformation("Attempting to save new module...");
            try
            {
                await _moduleRepository.AddItemAsync(module);

                return new Response<Module>(StatusCodes.Status201Created, module);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new Response<Module>(StatusCodes.Status400BadRequest, $"An error occurred when saving the module: {ex.Message}");
            }
        }

        public async Task<Module?> GetAsync(Guid id)
        {
            _logger.LogInformation("Attempting to retrieve existing module...");

            return await _moduleRepository.GetItemAsync(id);
        }

        public async Task<IEnumerable<Module>> ListAsync(EModuleType? moduleType)
        {
            _logger.LogInformation("Attempting to retrieve existing modules...");

            if (moduleType != null)
            {
                return await _moduleRepository.GetItemsAsync($"SELECT * FROM c WHERE c.ModuleType = '{moduleType}' AND c.IsArchived = 0");
            }
            else
            {
                return await _moduleRepository.GetItemsAsync("SELECT * FROM c WHERE c.IsArchived = 0");
            }
        }

        public async Task<Response<Module>> UpdateAsync(Guid id, Module module)
        {
            _logger.LogInformation("Attempting to update existing module...");

            var existingModule = await _moduleRepository.GetItemAsync(id);

            if (existingModule == null || existingModule.IsArchived)
                return new Response<Module>(StatusCodes.Status404NotFound, "Module not found.");

            existingModule.Name = module.Name;
            existingModule.Description = module.Description;
            existingModule.CreditPoints = module.CreditPoints;
            existingModule.MaxSize = module.MaxSize;

            try
            {
                await _moduleRepository.UpdateItemAsync(existingModule.Id, existingModule);

                return new Response<Module>(StatusCodes.Status200OK, existingModule);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new Response<Module>(StatusCodes.Status400BadRequest, $"An error occurred when updating the module: {ex.Message}");
            }
        }

        public async Task<Response<Module>> DeleteAsync(Guid id)
        {
            _logger.LogInformation("Attempting to delete existing module...");

            var existingModule = await _moduleRepository.GetItemAsync(id);

            if (existingModule == null)
                return new Response<Module>(StatusCodes.Status404NotFound, "Module not found.");

            try
            {
                // Modules are not deleted, but archived instead.
                existingModule.IsArchived = true;
                await _moduleRepository.UpdateItemAsync(existingModule.Id, existingModule);

                return new Response<Module>(StatusCodes.Status204NoContent);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new Response<Module>(StatusCodes.Status400BadRequest, $"An error occurred when deleting the module: {ex.Message}");
            }
        }
    }
}
