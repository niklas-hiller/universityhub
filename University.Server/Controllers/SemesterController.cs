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
        [HttpPost("/semesters", Name = "Create Semester")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(SemesterResource))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostAsync([FromBody] SaveSemesterResource resource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }
            var semester = _mapper.Map<SaveSemesterResource, Semester>(resource);
            var result = await _semesterService.SaveAsync(semester);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            var semesterResource = _mapper.Map<Semester, SemesterResource>(result.Semester);
            return Created("", value: semesterResource);
        }

        /// <summary>
        /// Add/Removes modules to a semester
        /// </summary>
        /// <param name="id"></param>
        /// <param name="resource"></param>
        /// <returns>The updated semester</returns>
        [HttpPatch("/semesters/{id}/modules", Name = "Add/Removes modules to a semester")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SemesterResource))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PatchModulesAsync(Guid id, [FromBody] PatchResource resource)
        {
            // Todo
            return Ok();
        }

        /// <summary>
        /// Retrieves a specific Semester by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The retrieved semester</returns>
        [HttpGet("/semesters/{id}", Name = "Get Semester By Id")]
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
        [HttpGet("/semesters", Name = "Get all Semesters")]
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
        [HttpDelete("/semesters/{id}", Name = "Delete Semester By Id")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var result = await _semesterService.DeleteAsync(id);

            if (!result.Success)
                return BadRequest(result.Message);

            return NoContent();
        }
    }
}