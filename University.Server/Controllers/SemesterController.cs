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
    [Route("semesters")]
    [Authorize]
    public class SemesterController : Controller
    {

        private readonly ILogger<SemesterController> _logger;
        private readonly ISemesterService _semesterService;
        private readonly IMapper _mapper;

        public SemesterController(ILogger<SemesterController> logger, ISemesterService semesterService, IMapper mapper)
        {
            _logger = logger;
            _semesterService = semesterService;
            _mapper = mapper;
        }

        /// <summary>
        /// Creates a new Semester
        /// </summary>
        /// <param name="resource"></param>
        /// <returns>The new created semester</returns>
        [HttpPost(Name = "Create Semester")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(SemesterResource))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Permission(EAuthorization.Administrator)]
        public async Task<IActionResult> PostAsync([FromBody] SaveSemesterResource resource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }
            var semester = _mapper.Map<SaveSemesterResource, Semester>(resource);
            var result = await _semesterService.SaveAsync(semester);

            switch (result.StatusCode)
            {
                case StatusCodes.Status201Created:
                    var createdResource = _mapper.Map<Semester, SemesterResource>(result.ResponseEntity);
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
        /// Add/Removes modules to a semester
        /// </summary>
        /// <param name="id"></param>
        /// <param name="resource"></param>
        /// <returns>The updated semester</returns>
        [HttpPatch("{id}/modules", Name = "Add/Removes modules to a semester")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SemesterResource))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Obsolete]
        public async Task<IActionResult> PatchModulesAsync(Guid id, [FromBody] PatchResource resource)
        {
            // Todo
            return StatusCode(StatusCodes.Status501NotImplemented);
        }

        /// <summary>
        /// Retrieves a specific Semester by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The retrieved semester</returns>
        [HttpGet("{id}", Name = "Get Semester By Id")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SemesterResource))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAsync(Guid id)
        {
            var semester = await _semesterService.GetAsync(id);
            if (semester == null)
            {
                return NotFound($"Couldn't find any semester with the id {id}");
            }
            var resource = _mapper.Map<Semester, SemesterResource>(semester);
            return Ok(resource);
        }

        /// <summary>
        /// Retrieves a all semesters
        /// </summary>
        /// <returns>The retrieved semesters</returns>
        [HttpGet(Name = "Get all Semesters")]
        [Produces("application/json")]
        public async Task<IEnumerable<SemesterResource>> GetAllAsync()
        {
            var semesters = await _semesterService.ListAsync();
            var resources = _mapper.Map<IEnumerable<Semester>, IEnumerable<SemesterResource>>(semesters);
            return resources;
        }

        /// <summary>
        /// Deletes a specific Semester by id
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("{id}", Name = "Delete Semester By Id")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Permission(EAuthorization.Administrator)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var result = await _semesterService.DeleteAsync(id);

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