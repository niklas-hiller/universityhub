using Microsoft.AspNetCore.Http.HttpResults;
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
        private readonly IUserService _userService;

        public ModuleService(ILogger<ModuleService> logger, ICosmosDbRepository<Module, ModuleEntity> moduleRepository, IUserService userService)
        {
            _logger = logger;
            _moduleRepository = moduleRepository;
            _userService = userService;
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
                return await _moduleRepository.GetItemsAsync($"SELECT * FROM c WHERE c.ModuleType = '{moduleType}' AND c.IsArchived = false");
            }
            else
            {
                _logger.LogInformation("No Module Type specified, retrieving all modules");
                return await _moduleRepository.GetItemsAsync("SELECT * FROM c WHERE c.IsArchived = false");
            }
        }

        public async Task<Response<Module>> PatchProfessorsAsync(Guid id, PatchModel<User> patch)
        {
            foreach(var user in patch.Add.Union(patch.Remove))
            {
                if (user.Authorization != EAuthorization.Professor)
                {
                    return new Response<Module>(StatusCodes.Status400BadRequest, "You can't add non-professors as users to a module.");
                }
            }

            var existingModule = await _moduleRepository.GetItemAsync(id);

            foreach (var add in patch.Add)
            {
                if (!existingModule.ProfessorIds.Contains(add.Id))
                {
                    #region User Assignment Logic
                    {
                        var patchModules = new PatchModel<Module>();
                        patchModules.Add.Add(existingModule);
                        var result = await _userService.PatchAssignmentsAsync(add.Id, patchModules);
                        if (result.StatusCode != StatusCodes.Status200OK)
                        {
                            return new Response<Module>(StatusCodes.Status400BadRequest, $"An error occurred when updating the module: {result.Message}");
                        }
                    }
                    #endregion

                    existingModule.ProfessorIds.Add(add.Id);
                }
            }
            foreach (var remove in patch.Remove)
            {
                if (existingModule.ProfessorIds.Contains(remove.Id))
                {
                    #region User Assignment Logic
                    {
                        var patchModules = new PatchModel<Module>();
                        patchModules.Remove.Add(existingModule);
                        var result = await _userService.PatchAssignmentsAsync(remove.Id, patchModules);
                        if (result.StatusCode != StatusCodes.Status200OK)
                        {
                            return new Response<Module>(StatusCodes.Status400BadRequest, $"An error occurred when updating the module: {result.Message}");
                        }
                    }
                    #endregion

                    existingModule.ProfessorIds.Remove(remove.Id);
                }
            }

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
