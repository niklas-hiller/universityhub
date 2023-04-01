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
        /// <remarks>This endpoint can only be used by Administrators.</remarks>
        /// <param name="resource"></param>
        /// <returns>The new created location</returns>
        [HttpPost(Name = "Create Location")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(LocationResource))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [Permission(EAuthorization.Administrator)]
        public async Task<IActionResult> PostAsync([FromBody] SaveLocationResource resource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }
            var location = _mapper.Map<SaveLocationResource, Location>(resource);
            var createdLocation = await _locationService.SaveAsync(location);

            var createdResource = _mapper.Map<Location, LocationResource>(createdLocation);
            return Created("", value: createdResource);
        }

        /// <summary>
        /// Updates a Location
        /// </summary>
        /// <remarks>This endpoint can only be used by Administrators.</remarks>
        /// <param name="id">The id of the location that should be updated.</param>
        /// <param name="resource"></param>
        /// <returns>The updated location</returns>
        [HttpPut("{id}", Name = "Update Location")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LocationResource))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [Permission(EAuthorization.Administrator)]
        public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpdateLocationResource resource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }
            var location = _mapper.Map<UpdateLocationResource, Location>(resource);
            var updatedLocation = await _locationService.UpdateAsync(id, location);

            var updatedResource = _mapper.Map<Location, LocationResource>(updatedLocation);
            return Ok(updatedResource);
        }

        /// <summary>
        /// Retrieves a specific Location by id
        /// </summary>
        /// <remarks>This endpoint can be used by any authenticated user.</remarks>
        /// <param name="id">The id of the location that should be retrieved.</param>
        /// <returns>The retrieved location</returns>
        [HttpGet("{id}", Name = "Get Location By Id")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LocationResource))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetAsync(Guid id)
        {
            var retrievedLocation = await _locationService.GetAsync(id);

            var retrievedResource = _mapper.Map<Location, LocationResource>(retrievedLocation);
            return Ok(retrievedResource);
        }

        /// <summary>
        /// Retrieves all locations
        /// </summary>
        /// <remarks>This endpoint can be used by any authenticated user.</remarks>
        /// <returns>The retrieved locations</returns>
        [HttpGet(Name = "Get all Locations")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<LocationResource>))]
        public async Task<IEnumerable<LocationResource>> GetAllAsync()
        {
            var locations = await _locationService.ListAsync();
            var resources = _mapper.Map<IEnumerable<Location>, IEnumerable<LocationResource>>(locations);
            return resources;
        }

        /// <summary>
        /// Deletes a specific Location by id
        /// </summary>
        /// <remarks>This endpoint can only be used by Administrators.</remarks>
        /// <param name="id">The id of the location that should be deleted.</param>
        [HttpDelete("{id}", Name = "Delete Location By Id")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [Permission(EAuthorization.Administrator)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await _locationService.DeleteAsync(id);

            return NoContent();
        }
    }
}