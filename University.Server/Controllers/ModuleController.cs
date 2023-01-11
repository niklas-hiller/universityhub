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

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            var moduleResource = _mapper.Map<Module, ModuleResource>(result.Module);
            return Created("", value: moduleResource);
        }

    }
}