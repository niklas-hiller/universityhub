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

    }
}