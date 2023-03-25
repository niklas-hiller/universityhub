using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using University.Server.Attributes;
using University.Server.Domain.Models;
using University.Server.Domain.Services;
using University.Server.Extensions;
using University.Server.Resources;

namespace University.Server.Controllers
{
    [ApiController]
    [Route("locations")]
    [Authorize]
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
        [HttpPost(Name = "Create Location")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(LocationResource))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Permission(EAuthorization.Administrator)]
        public async Task<IActionResult> PostAsync([FromBody] SaveLocationResource resource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }
            var location = _mapper.Map<SaveLocationResource, Location>(resource);
            var result = await _locationService.SaveAsync(location);

            switch (result.StatusCode)
            {
                case StatusCodes.Status201Created:
                    if (result.ResponseEntity == null)
                    {
                        return StatusCode(500);
                    }
                    var createdResource = _mapper.Map<Location, LocationResource>(result.ResponseEntity);
                    return Created("", value: createdResource);
                default:
                    return StatusCode(result.StatusCode, result.Message);
            }
        }

        /// <summary>
        /// Updates a Location
        /// </summary>
        /// <param name="id"></param>
        /// <param name="resource"></param>
        /// <returns>The updated location</returns>
        [HttpPut("{id}", Name = "Update Location")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LocationResource))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpdateLocationResource resource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }
            var location = _mapper.Map<UpdateLocationResource, Location>(resource);
            var result = await _locationService.UpdateAsync(id, location);

            switch (result.StatusCode)
            {
                case StatusCodes.Status200OK:
                    if (result.ResponseEntity == null)
                    {
                        return StatusCode(500);
                    }
                    var updatedResource = _mapper.Map<Location, LocationResource>(result.ResponseEntity);
                    return Ok(updatedResource);
                default:
                    return StatusCode(result.StatusCode, result.Message);
            }
        }

        /// <summary>
        /// Retrieves a specific Location by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The retrieved location</returns>
        [HttpGet("{id}", Name = "Get Location By Id")]
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
        [HttpGet(Name = "Get all Locations")]
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
        [HttpDelete("{id}", Name = "Delete Location By Id")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Permission(EAuthorization.Administrator)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var result = await _locationService.DeleteAsync(id);

            switch (result.StatusCode)
            {
                case StatusCodes.Status204NoContent:
                    return NoContent();
                default:
                    return StatusCode(result.StatusCode, result.Message);
            }
        }
    }
}