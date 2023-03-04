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
        [HttpPost("/courses", Name = "Create Course")]
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

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            var courseResource = _mapper.Map<Course, CourseResource>(result.Course);
            return Created("", value: courseResource);
        }

        /// <summary>
        /// Updates a Course
        /// </summary>
        /// <param name="id"></param>
        /// <param name="resource"></param>
        /// <returns>The updated course</returns>
        [HttpPatch("/courses/{id}", Name = "Update Course")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CourseResource))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PatchAsync(Guid id, [FromBody] UpdateCourseResource resource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }
            var course = _mapper.Map<UpdateCourseResource, Course>(resource);
            var result = await _courseService.UpdateAsync(id, course);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            var courseResource = _mapper.Map<Course, CourseResource>(result.Course);
            return Ok(value: courseResource);
        }

        /// <summary>
        /// Overwrites Course
        /// </summary>
        /// <param name="id"></param>
        /// <param name="resource"></param>
        /// <returns>The updated course</returns>
        [HttpPut("/courses/{id}", Name = "Overwrite Course")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CourseResource))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutAsync(Guid id, [FromBody] OverwriteCourseResource resource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }
            var course = _mapper.Map<OverwriteCourseResource, Course>(resource);
            var result = await _courseService.OverwriteAsync(id, course);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            var courseResource = _mapper.Map<Course, CourseResource>(result.Course);
            return Ok(value: courseResource);
        }

        /// <summary>
        /// Retrieves a specific Course by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The retrieved course</returns>
        [HttpGet("/courses/{id}", Name = "Get Course By Id")]
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
        [HttpGet("/courses", Name = "Get all Courses")]
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
        [HttpDelete("/courses/{id}", Name = "Delete Course By Id")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var result = await _courseService.DeleteAsync(id);

            if (!result.Success)
                return BadRequest(result.Message);

            return NoContent();
        }
    }
}