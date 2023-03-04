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
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public ModuleController(ILogger<ModuleController> logger, IModuleService moduleService,
            IUserService userService, IMapper mapper)
        {
            _logger = logger;
            _moduleService = moduleService;
            _userService = userService;
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

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            var moduleResource = _mapper.Map<Module, ModuleResource>(result.Module);
            return Created("", value: moduleResource);
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
            if (module == null)
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
        public async Task<IEnumerable<ModuleResource>> GetFilteredAsync(EModuleType? moduleType)
        {
            var modules = await _moduleService.ListAsync(moduleType);
            var resources = _mapper.Map<IEnumerable<Module>, IEnumerable<ModuleResource>>(modules);
            return resources;
        }

        /// <summary>
        /// Updates a Module
        /// </summary>
        /// <param name="id"></param>
        /// <param name="resource"></param>
        /// <returns>The updated module</returns>
        [HttpPatch("/modules/{id}", Name = "Updates a Module")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ModuleResource))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateModuleResource resource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }

            var module = _mapper.Map<UpdateModuleResource, Module>(resource);
            var result = await _moduleService.UpdateAsync(id, module);

            var updatedResource = _mapper.Map<Module, ModuleResource>(result.Module);
            return Ok(updatedResource);
        }

        /// <summary>
        /// Deletes a specific Module by his id
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("/modules/{id}", Name = "Delete Module By Id")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var result = await _moduleService.DeleteAsync(id);

            if (!result.Success)
                return BadRequest(result.Message);

            return NoContent();
        }

    }
}