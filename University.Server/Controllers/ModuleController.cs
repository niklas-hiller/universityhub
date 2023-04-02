using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using University.Server.Domain.Models;
using University.Server.Domain.Services;
using University.Server.Extensions;
using University.Server.Filters;
using University.Server.Resources.Request;
using University.Server.Resources.Response;

namespace University.Server.Controllers
{
    [ApiController]
    [Route("modules")]
    [Authorize]
    public class ModuleController : Controller
    {
        private readonly ILogger<ModuleController> _logger;
        private readonly IModuleService _moduleService;
        private readonly IMapper _mapper;

        public ModuleController(ILogger<ModuleController> logger, IModuleService moduleService, IMapper mapper)
        {
            _logger = logger;
            _moduleService = moduleService;
            _mapper = mapper;
        }

        /// <summary>
        /// Creates a new Module
        /// </summary>
        /// <remarks>This endpoint can only be used by Administrators.</remarks>
        /// <param name="resource"></param>
        /// <returns>The new created module</returns>
        [HttpPost(Name = "Create Module")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ModuleResource))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [Permission(EAuthorization.Administrator)]
        public async Task<IActionResult> PostAsync([FromBody] SaveModuleResource resource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }

            var module = _mapper.Map<SaveModuleResource, Module>(resource);

            var createdModule = await _moduleService.SaveAsync(module);
            var createdResource = _mapper.Map<Module, ModuleResource>(createdModule);

            return Created("", value: createdResource);
        }

        /// <summary>
        /// Updates a Module
        /// </summary>
        /// <remarks>This endpoint can only be used by Administrators.</remarks>
        /// <param name="id">The id of the module that should be updated.</param>
        /// <param name="resource"></param>
        /// <returns>The updated module</returns>
        [HttpPut("{id}", Name = "Updates a Module")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ModuleResource))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [Permission(EAuthorization.Administrator)]
        public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpdateModuleResource resource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }

            var module = _mapper.Map<UpdateModuleResource, Module>(resource);

            var updatedModule = await _moduleService.UpdateAsync(id, module);
            var updatedResource = _mapper.Map<Module, ModuleResource>(updatedModule);

            return Ok(updatedResource);
        }

        /// <summary>
        /// Add/Removes available professors to a module
        /// </summary>
        /// <remarks>This endpoint can only be used by Administrators.</remarks>
        /// <param name="id">The id of the module that should be updated.</param>
        /// <param name="resource"></param>
        /// <returns>The updated module</returns>
        [HttpPatch("{id}/professors", Name = "Add/Removes available professors to a module")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ModuleResource))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [Permission(EAuthorization.Administrator)]
        public async Task<IActionResult> PatchProfessorsAsync(Guid id, [FromBody] PatchResource resource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }
            var patch = _mapper.Map<PatchResource, PatchModel<User>>(resource);

            var updatedModule = await _moduleService.PatchProfessorsAsync(id, patch);
            var updatedResource = _mapper.Map<Module, ModuleResource>(updatedModule);

            return Ok(updatedResource);
        }

        /// <summary>
        /// Retrieves a specific Module by it's id
        /// </summary>
        /// <remarks>This endpoint can be used by any authenticated user.</remarks>
        /// <param name="id">The id of the module that should be retrieved.</param>
        /// <returns>The retrieved module</returns>
        [HttpGet("{id}", Name = "Get Module By Id")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ModuleResource))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetAsync(Guid id)
        {
            var retrievedModule = await _moduleService.GetAsync(id);
            var retrievedResource = _mapper.Map<Module, ModuleResource>(retrievedModule);

            return Ok(retrievedResource);
        }

        /// <summary>
        /// Retrieves all modules matching filter
        /// </summary>
        /// <remarks>This endpoint can be used by any authenticated user.</remarks>
        /// <param name="moduleType">The type the retrieved modules should have. If left empty, will retrieve all modules.</param>
        /// <returns>The retrieved modules</returns>
        [HttpGet(Name = "Get all Modules matching filter")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ModuleResource>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetFilteredAsync(EModuleType? moduleType)
        {
            var retrievedModules = await _moduleService.ListAsync(moduleType);
            var retrievedResources = _mapper.Map<IEnumerable<Module>, IEnumerable<ModuleResource>>(retrievedModules);

            return Ok(retrievedResources);
        }

        /// <summary>
        /// Deletes a specific Module by his id
        /// </summary>
        /// <remarks>This endpoint can only be used by Administrators.</remarks>
        /// <param name="id">The id of the module that should be deleted.</param>
        [HttpDelete("{id}", Name = "Delete Module By Id")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [Permission(EAuthorization.Administrator)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await _moduleService.DeleteAsync(id);

            return NoContent();
        }
    }
}