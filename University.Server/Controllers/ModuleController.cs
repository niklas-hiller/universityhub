using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using University.Server.Domain.Models;
using University.Server.Domain.Services;
using University.Server.Extensions;
using University.Server.Resources;

namespace University.Server.Controllers
{
    [ApiController]
    [Route("/api/v1/[controller]")]
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
        /// <param name="resource"></param>
        /// <returns>The new created module</returns>
        [HttpPost("/modules", Name = "Create Module")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ModuleResource))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostAsync([FromBody] SaveModuleResource resource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }

            var module = _mapper.Map<SaveModuleResource, Module>(resource);
            var result = await _moduleService.SaveAsync(module);

            switch (result.StatusCode)
            {
                case StatusCodes.Status201Created:
                    var createdResource = _mapper.Map<Module, ModuleResource>(result.ResponseEntity);
                    return Created("", value: createdResource);
                case StatusCodes.Status400BadRequest:
                    return BadRequest(result.Message);
                case StatusCodes.Status404NotFound:
                    return NotFound(result.Message);
                default:
                    return StatusCode(500);
            }
        }

        /// <summary>
        /// Updates a Module
        /// </summary>
        /// <param name="id"></param>
        /// <param name="resource"></param>
        /// <returns>The updated module</returns>
        [HttpPut("/modules/{id}", Name = "Updates a Module")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ModuleResource))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpdateModuleResource resource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }

            var module = _mapper.Map<UpdateModuleResource, Module>(resource);
            var result = await _moduleService.UpdateAsync(id, module);

            switch (result.StatusCode)
            {
                case StatusCodes.Status200OK:
                    var updatedResource = _mapper.Map<Module, ModuleResource>(result.ResponseEntity);
                    return Ok(updatedResource);
                case StatusCodes.Status400BadRequest:
                    return BadRequest(result.Message);
                case StatusCodes.Status404NotFound:
                    return NotFound(result.Message);
                default:
                    return StatusCode(500);
            }
        }

        /// <summary>
        /// Add/Removes available professors to a module
        /// </summary>
        /// <param name="id"></param>
        /// <param name="resource"></param>
        /// <returns>The updated module</returns>
        [HttpPatch("/modules/{id}/professors", Name = "Add/Removes available professors to a module")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ModuleResource))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Obsolete]
        public async Task<IActionResult> PatchProfessorsAsync(Guid id, [FromBody] PatchResource resource)
        {
            // Todo
            return Forbid("Currently not implemented");
        }

        /// <summary>
        /// Retrieves a specific Module by it's id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The retrieved module</returns>
        [HttpGet("/modules/{id}", Name = "Get Module By Id")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ModuleResource))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAsync(Guid id)
        {
            var module = await _moduleService.GetAsync(id);
            if (module == null || module.IsArchived)
            {
                return NotFound($"Couldn't find any module with the id {id}");
            }
            var resource = _mapper.Map<Module, ModuleResource>(module);
            return Ok(resource);
        }

        /// <summary>
        /// Retrieves all modules matching filter
        /// </summary>
        /// <param name="moduleType"></param>
        /// <returns>The retrieved modules</returns>
        [HttpGet("/modules", Name = "Get all Modules matching filter")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IEnumerable<ModuleResource>> GetFilteredAsync(EModuleType? moduleType)
        {
            var modules = await _moduleService.ListAsync(moduleType);
            var resources = _mapper.Map<IEnumerable<Module>, IEnumerable<ModuleResource>>(modules);
            return resources;
        }

        /// <summary>
        /// Deletes a specific Module by his id
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("/modules/{id}", Name = "Delete Module By Id")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var result = await _moduleService.DeleteAsync(id);

            switch (result.StatusCode)
            {
                case StatusCodes.Status204NoContent:
                    return NoContent();
                case StatusCodes.Status400BadRequest:
                    return BadRequest(result.Message);
                case StatusCodes.Status404NotFound:
                    return NotFound(result.Message);
                default:
                    return StatusCode(500);
            }
        }

    }
}