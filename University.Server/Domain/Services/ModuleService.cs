using University.Server.Domain.Models;
using University.Server.Domain.Persistence;
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
    }
}
