using System.Net;
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

        public async Task<ModuleResponse> SaveAsync(Module module)
        {
            try
            {
                await _moduleRepository.AddItemAsync(module);

                return new ModuleResponse(module);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new ModuleResponse($"An error occurred when saving the module: {ex.Message}");
            }
        }

        public async Task<IEnumerable<Module>> ListAsync(EModuleType? moduleType)
        {
            if (moduleType != null)
            {
                return await _moduleRepository.GetItemsAsync($"SELECT * FROM c WHERE c.ModuleType = '{moduleType}'");
            }
            else
            {
                return await _moduleRepository.GetItemsAsync("SELECT * FROM c");
            }
        }

        public async Task<Module?> GetAsync(Guid id)
        {
            return await _moduleRepository.GetItemAsync(id);
        }

        public async Task<ModuleResponse> UpdateAsync(Guid id, Module module)
        {
            var existingModule = await _moduleRepository.GetItemAsync(id);

            if (existingModule == null)
                return new ModuleResponse("Module not found.");

            if (!String.IsNullOrEmpty(module.Name))
            {
                existingModule.Name = module.Name;
            }
            if (!String.IsNullOrEmpty(module.Description))
            {
                existingModule.Description = module.Description;
            }
            if (module.CreditPoints > 0)
            {
                existingModule.CreditPoints = module.CreditPoints;
            }
            if (module.ModuleType != 0)
            {
                existingModule.ModuleType = module.ModuleType;
            }

            try
            {
                await _moduleRepository.UpdateItemAsync(existingModule.Id, existingModule);

                return new ModuleResponse(existingModule);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new ModuleResponse($"An error occurred when updating the module: {ex.Message}");
            }
        }

        public async Task<ModuleResponse> OverwriteAsync(Guid id, Module module)
        {
            var existingModule = await _moduleRepository.GetItemAsync(id);

            if (existingModule == null)
                return new ModuleResponse("Module not found.");

            existingModule.Professors = module.Professors;

            try
            {
                await _moduleRepository.UpdateItemAsync(existingModule.Id, existingModule);

                return new ModuleResponse(existingModule);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new ModuleResponse($"An error occurred when overwriting the module: {ex.Message}");
            }
        }

        public async Task<ModuleResponse> DeleteAsync(Guid id)
        {
            var existingModule = await _moduleRepository.GetItemAsync(id);

            if (existingModule == null)
                return new ModuleResponse("User not found.");

            try
            {
                await _moduleRepository.DeleteItemAsync(existingModule.Id);

                return new ModuleResponse(existingModule);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new ModuleResponse($"An error occurred when deleting the module: {ex.Message}");
            }
        }

        public async Task<List<Module>> ConvertGuidListToModuleList(List<Guid> moduleIds)
        {
            List<Module> modules = new List<Module>();
            foreach (Guid moduleId in moduleIds)
            {
                Module? module = await GetAsync(moduleId);
                if (module != null)
                {
                    modules.Add(module);
                }
            }
            return modules;
        }
    }
}
