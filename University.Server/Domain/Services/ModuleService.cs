using System.Net;
using University.Server.Domain.Models;
using University.Server.Domain.Persistence.Repositories;
using University.Server.Domain.Repositories;
using University.Server.Domain.Services.Communication;

namespace University.Server.Domain.Services
{
    public class ModuleService : IModuleService
    {
        private readonly ILogger<ModuleService> _logger;
        private readonly IModuleRepository _moduleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ModuleService(ILogger<ModuleService> logger, IModuleRepository moduleRepository, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _moduleRepository = moduleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ModuleResponse> SaveAsync(Module module)
        {
            try
            {
                await _moduleRepository.AddAsync(module);
                await _unitOfWork.CompleteAsync();

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
            var modules = await _moduleRepository.ListAsync();
            if (moduleType != null)
            {
                modules = modules.Where(module => module.ModuleType == moduleType);
            }

            return modules;
        }

        public async Task<Module?> GetAsync(Guid id)
        {
            return await _moduleRepository.GetAsync(id);
        }

        public async Task<ModuleResponse> UpdateAsync(Guid id, Module module)
        {
            var existingModule = await _moduleRepository.GetAsync(id);

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
                _moduleRepository.Update(existingModule);
                await _unitOfWork.CompleteAsync();

                return new ModuleResponse(existingModule);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new ModuleResponse($"An error occurred when updating the module: {ex.Message}");
            }
        }

        public async Task<ModuleResponse> DeleteAsync(Guid id)
        {
            var existingModule = await _moduleRepository.GetAsync(id);

            if (existingModule == null)
                return new ModuleResponse("User not found.");

            try
            {
                _moduleRepository.Remove(existingModule);
                await _unitOfWork.CompleteAsync();

                return new ModuleResponse(existingModule);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new ModuleResponse($"An error occurred when deleting the module: {ex.Message}");
            }
        }
    }
}
