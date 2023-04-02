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
        /// <remarks>This endpoint can only be used by Administrators.</remarks>
        /// <param name="resource"></param>
        /// <returns>The new created semester</returns>
        [HttpPost(Name = "Create Semester")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(SemesterResource))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [Permission(EAuthorization.Administrator)]
        public async Task<IActionResult> PostAsync([FromBody] SaveSemesterResource resource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }
            var semester = _mapper.Map<SaveSemesterResource, Semester>(resource);

            var createdSemester = await _semesterService.SaveAsync(semester);
            var createdResource = _mapper.Map<Semester, SemesterResource>(createdSemester);

            return Created("", value: createdResource);
        }

        /// <summary>
        /// Add/Removes modules to a semester
        /// </summary>
        /// <remarks>This endpoint can only be used by Administrators.</remarks>
        /// <param name="id">The id of the semester that should be updated.</param>
        /// <param name="resource"></param>
        /// <returns>The updated semester</returns>
        [HttpPatch("{id}/modules", Name = "Add/Removes modules to a semester")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SemesterResource))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [Permission(EAuthorization.Administrator)]
        public async Task<IActionResult> PatchModulesAsync(Guid id, [FromBody] PatchResource resource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }
            var patch = _mapper.Map<PatchResource, PatchModel<Module>>(resource);

            var updatedSemester = await _semesterService.PatchModulesAsync(id, patch);
            var updatedResource = _mapper.Map<Semester, SemesterResource>(updatedSemester);

            return Ok(updatedResource);
        }

        /// <summary>
        /// Retrieves a specific Semester by id
        /// </summary>
        /// <remarks>This endpoint can be used by any authenticated user.</remarks>
        /// <param name="id">The id of the semester that should be retrieved.</param>
        /// <returns>The retrieved semester</returns>
        [HttpGet("{id}", Name = "Get Semester By Id")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SemesterResource))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetAsync(Guid id)
        {
            var retrievedSemester = await _semesterService.GetAsync(id);
            var retrievedResource = _mapper.Map<Semester, SemesterResource>(retrievedSemester);

            return Ok(retrievedResource);
        }

        /// <summary>
        /// Retrieves a all semesters
        /// </summary>
        /// <remarks>This endpoint can be used by any authenticated user.</remarks>
        /// <returns>The retrieved semesters</returns>
        [HttpGet(Name = "Get all Semesters")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<SemesterResource>))]
        public async Task<IActionResult> GetAllAsync()
        {
            var retrievedSemesters = await _semesterService.ListAsync();
            var retrievedResources = _mapper.Map<IEnumerable<Semester>, IEnumerable<SemesterResource>>(retrievedSemesters);

            return Ok(retrievedResources);
        }

        /// <summary>
        /// Deletes a specific Semester by id
        /// </summary>
        /// <remarks>This endpoint can only be used by Administrators.</remarks>
        /// <param name="id">The id of the semester that should be deleted.</param>
        [HttpDelete("{id}", Name = "Delete Semester By Id")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [Permission(EAuthorization.Administrator)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await _semesterService.DeleteAsync(id);

            return NoContent();
        }

        /// <summary>
        /// Sets a semester to active, starting calculation of lectures
        /// </summary>
        /// <remarks>This endpoint can only be used by Administrators.</remarks>
        /// <param name="id">The id of the semester that should be activated.</param>
        /// <returns>The updated semester</returns>
        [HttpPost("{id}/activate", Name = "Set status of semester to active")]
        [Produces("application/json")]
        [Permission(EAuthorization.Administrator)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(SemesterResource))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> PostActivateAsync(Guid id)
        {
            var createdSemester = await _semesterService.CalculateAsync(id);
            var createdResource = _mapper.Map<Semester, SemesterResource>(createdSemester);

            return Created("", value: createdResource);
        }
    }
}