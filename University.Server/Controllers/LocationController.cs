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
    public class LocationController : Controller
    {

        private readonly ILogger<LocationController> _logger;
        private readonly ILocationService _locationService;
        private readonly IMapper _mapper;

        public LocationController(ILogger<LocationController> logger, ILocationService locationService, IMapper mapper)
        {
            _logger = logger;
            _locationService = locationService;
            _mapper = mapper;
        }

        /// <summary>
        /// Creates a new Location
        /// </summary>
        /// <param name="resource"></param>
        /// <returns>The new created location</returns>
        [HttpPost("/locations", Name = "Create Location")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(LocationResource))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostAsync([FromBody] SaveLocationResource resource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }
            var location = _mapper.Map<SaveLocationResource, Location>(resource);
            var result = await _locationService.SaveAsync(location);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            var locationResource = _mapper.Map<Location, LocationResource>(result.Location);
            return Created("", value: locationResource);
        }

    }
}