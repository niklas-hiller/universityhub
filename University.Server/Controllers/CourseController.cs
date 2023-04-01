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
    [Route("courses")]
    [Authorize]
    public class CourseController : Controller
    {

        private readonly ILogger<CourseController> _logger;
        private readonly ICourseService _courseService;
        private readonly IMapper _mapper;

        public CourseController(ILogger<CourseController> logger, ICourseService courseService, IMapper mapper)
        {
            _logger = logger;
            _courseService = courseService;
            _mapper = mapper;
        }

        /// <summary>
        /// Creates a new Course
        /// </summary>
        /// <remarks>This endpoint can only be used by Administrators.</remarks>
        /// <param name="resource"></param>
        /// <returns>The new created course</returns>
        [HttpPost(Name = "Create Course")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CourseResource))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [Permission(EAuthorization.Administrator)]
        public async Task<IActionResult> PostAsync([FromBody] SaveCourseResource resource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }
            var course = _mapper.Map<SaveCourseResource, Course>(resource);
            var createdCourse = await _courseService.SaveAsync(course);

            var createdResource = _mapper.Map<Course, CourseResource>(createdCourse);
            return Created("", value: createdResource);
        }

        /// <summary>
        /// Updates a Course
        /// </summary>
        /// <remarks>This endpoint can only be used by Administrators.</remarks>
        /// <param name="id">The id of the course that should be updated.</param>
        /// <param name="resource"></param>
        /// <returns>The updated course</returns>
        [HttpPut("{id}", Name = "Update Course")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CourseResource))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [Permission(EAuthorization.Administrator)]
        public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpdateCourseResource resource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }
            var course = _mapper.Map<UpdateCourseResource, Course>(resource);
            var updatedCourse = await _courseService.UpdateAsync(id, course);

            var updatedResource = _mapper.Map<Course, CourseResource>(updatedCourse);
            return Ok(updatedResource);
        }

        /// <summary>
        /// Adds/Removes Students to a course
        /// </summary>
        /// <remarks>This endpoint can only be used by Administrators.</remarks>
        /// <param name="id">The id of the course that should be updated.</param>
        /// <param name="resource"></param>
        /// <returns>The updated course</returns>
        [HttpPatch("{id}/students", Name = "Add/Removes students to a course")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CourseResource))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [Permission(EAuthorization.Administrator)]
        public async Task<IActionResult> PatchStudentsAsync(Guid id, [FromBody] PatchResource resource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }
            var patch = _mapper.Map<PatchResource, PatchModel<User>>(resource);
            var updatedCourse = await _courseService.PatchStudentsAsync(id, patch);

            var updatedResource = _mapper.Map<Course, CourseResource>(updatedCourse);
            return Ok(updatedResource);
        }

        /// <summary>
        /// Adds/Removes Compulsory modules to a all students of a course
        /// </summary>
        /// <remarks>This endpoint can only be used by Administrators.</remarks>
        /// <param name="id">The id of the course that should be updated.</param>
        /// <param name="resource"></param>
        /// <returns>The updated course</returns>
        [HttpPatch("{id}/assignments", Name = "Add/Removes compulsory modules to all students of a course")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CourseResource))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [Permission(EAuthorization.Administrator)]
        public async Task<IActionResult> PatchAssignmentsAsync(Guid id, [FromBody] PatchResource resource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }
            var patch = _mapper.Map<PatchResource, PatchModel<Module>>(resource);
            var updatedCourse = await _courseService.PatchModulesAsync(id, patch);

            var updatedResource = _mapper.Map<Course, CourseResource>(updatedCourse);
            return Ok(updatedResource);
        }

        /// <summary>
        /// Retrieves a specific Course by id
        /// </summary>
        /// <remarks>This endpoint can be used by any authenticated user.</remarks>
        /// <param name="id">The id of the course that should be retrieved.</param>
        /// <returns>The retrieved course</returns>
        [HttpGet("{id}", Name = "Get Course By Id")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CourseResource))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetAsync(Guid id)
        {
            var retrievedCourse = await _courseService.GetAsync(id);

            var retrievedResource = _mapper.Map<Course, CourseResource>(retrievedCourse);
            return Ok(retrievedResource);
        }

        /// <summary>
        /// Retrieves all courses
        /// </summary>
        /// <remarks>This endpoint can be used by any authenticated user.</remarks>
        /// <returns>The retrieved courses</returns>
        [HttpGet(Name = "Get all Courses")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CourseResource>))]
        public async Task<IEnumerable<CourseResource>> GetAllAsync()
        {
            var courses = await _courseService.ListAsync();
            var resources = _mapper.Map<IEnumerable<Course>, IEnumerable<CourseResource>>(courses);
            return resources;
        }

        /// <summary>
        /// Deletes a specific Course by id
        /// </summary>
        /// <remarks>This endpoint can only be used by Administrators.</remarks>
        /// <param name="id">The id of the course that should be deleted.</param>
        [HttpDelete("{id}", Name = "Delete Course By Id")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [Permission(EAuthorization.Administrator)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await _courseService.DeleteAsync(id);

            return NoContent();
        }
    }
}