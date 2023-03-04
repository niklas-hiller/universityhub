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

        /// <summary>
        /// Updates a Location
        /// </summary>
        /// <param name="id"></param>
        /// <param name="resource"></param>
        /// <returns>The updated location</returns>
        [HttpPatch("/locations/{id}", Name = "Update Location")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LocationResource))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PatchAsync(Guid id, [FromBody] UpdateLocationResource resource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }
            var location = _mapper.Map<UpdateLocationResource, Location>(resource);
            var result = await _locationService.UpdateAsync(id, location);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            var locationResource = _mapper.Map<Location, LocationResource>(result.Location);
            return Ok(value: locationResource);
        }

        /// <summary>
        /// Retrieves a specific Location by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The retrieved location</returns>
        [HttpGet("/locations/{id}", Name = "Get Location By Id")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LocationResource))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAsync(Guid id)
        {
            var location = await _locationService.GetAsync(id);
            if (location == null)
            {
                return NotFound($"Couldn't find any location with the id {id}");
            }
            var resource = _mapper.Map<Location, LocationResource>(location);
            return Ok(resource);
        }

        /// <summary>
        /// Retrieves all locations
        /// </summary>
        /// <returns>The retrieved locations</returns>
        [HttpGet("/locations", Name = "Get all Locations")]
        [Produces("application/json")]
        public async Task<IEnumerable<LocationResource>> GetAllAsync()
        {
            var locations = await _locationService.ListAsync();
            var resources = _mapper.Map<IEnumerable<Location>, IEnumerable<LocationResource>>(locations);
            return resources;
        }

        /// <summary>
        /// Deletes a specific Location by id
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("/locations/{id}", Name = "Delete Location By Id")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var result = await _locationService.DeleteAsync(id);

            if (!result.Success)
                return BadRequest(result.Message);

            return NoContent();
        }
    }
}