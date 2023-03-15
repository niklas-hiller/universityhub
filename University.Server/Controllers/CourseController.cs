using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using University.Server.Domain.Models;
using University.Server.Domain.Services;
using University.Server.Extensions;
using University.Server.Resources;

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
        /// <param name="resource"></param>
        /// <returns>The new created course</returns>
        [HttpPost(Name = "Create Course")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CourseResource))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostAsync([FromBody] SaveCourseResource resource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }
            var course = _mapper.Map<SaveCourseResource, Course>(resource);
            var result = await _courseService.SaveAsync(course);

            switch (result.StatusCode)
            {
                case StatusCodes.Status201Created:
                    var createdResource = _mapper.Map<Course, CourseResource>(result.ResponseEntity);
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
        /// Updates a Course
        /// </summary>
        /// <param name="id"></param>
        /// <param name="resource"></param>
        /// <returns>The updated course</returns>
        [HttpPut("{id}", Name = "Update Course")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CourseResource))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpdateCourseResource resource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }
            var course = _mapper.Map<UpdateCourseResource, Course>(resource);
            var result = await _courseService.UpdateAsync(id, course);

            switch (result.StatusCode)
            {
                case StatusCodes.Status200OK:
                    var updatedResource = _mapper.Map<Course, CourseResource>(result.ResponseEntity);
                    return Ok(updatedResource);
                case StatusCodes.Status400BadRequest:
                    return BadRequest(result.Message);
                case StatusCodes.Status404NotFound:
                    return NotFound(result.Message);
                default:
                    return StatusCode(500);
            }
        }

        /// <summary>
        /// Adds/Removes Students to a course
        /// </summary>
        /// <param name="id"></param>
        /// <param name="resource"></param>
        /// <returns>The updated course</returns>
        [HttpPatch("{id}/students", Name = "Add/Removes students to a course")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CourseResource))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Obsolete]
        public async Task<IActionResult> PatchStudentsAsync(Guid id, [FromBody] PatchResource resource)
        {
            // Todo
            return Forbid("Currently not implemented");
        }

        /// <summary>
        /// Adds/Removes Compulsory modules to a all students of a course
        /// </summary>
        /// <param name="id"></param>
        /// <param name="resource"></param>
        /// <returns>The updated course</returns>
        [HttpPatch("{id}/assignments", Name = "Add/Removes compulsory modules to all students of a course")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CourseResource))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Obsolete]
        public async Task<IActionResult> PatchAssignmentsAsync(Guid id, [FromBody] PatchResource resource)
        {
            // Todo
            return Forbid("Currently not implemented");
        }

        /// <summary>
        /// Retrieves a specific Course by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The retrieved course</returns>
        [HttpGet("{id}", Name = "Get Course By Id")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CourseResource))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAsync(Guid id)
        {
            var course = await _courseService.GetAsync(id);
            if (course == null)
            {
                return NotFound($"Couldn't find any course with the id {id}");
            }
            var resource = _mapper.Map<Course, CourseResource>(course);
            return Ok(resource);
        }

        /// <summary>
        /// Retrieves all courses
        /// </summary>
        /// <returns>The retrieved courses</returns>
        [HttpGet(Name = "Get all Courses")]
        [Produces("application/json")]
        public async Task<IEnumerable<CourseResource>> GetAllAsync()
        {
            var courses = await _courseService.ListAsync();
            var resources = _mapper.Map<IEnumerable<Course>, IEnumerable<CourseResource>>(courses);
            return resources;
        }

        /// <summary>
        /// Deletes a specific Course by id
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("{id}", Name = "Delete Course By Id")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var result = await _courseService.DeleteAsync(id);

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