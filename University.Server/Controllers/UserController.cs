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
    public class UserController : Controller
    {

        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserController(ILogger<UserController> logger, IUserService userService, IMapper mapper)
        {
            _logger = logger;
            _userService = userService;
            _mapper = mapper;
        }

        /// <summary>
        /// Creates a new User
        /// </summary>
        /// <param name="resource"></param>
        /// <returns>The new created user</returns>
        [HttpPost("/users", Name = "Create User")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UserResource))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostAsync([FromBody] SaveUserResource resource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }
            var user = _mapper.Map<SaveUserResource, User>(resource);
            var result = await _userService.SaveAsync(user);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            var userResource = _mapper.Map<User, UserResource>(result.User);
            return Created("", value: userResource);
        }

        /// <summary>
        /// Retrieves a specific User by his id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The retrieved user</returns>
        [HttpGet("/users/{id}", Name = "Get User By Id")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResource))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAsync(Guid id)
        {
            var user = await _userService.GetAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var resource = _mapper.Map<User, UserResource>(user);
            return Ok(resource);
        }

        /// <summary>
        /// Retrieves a all users
        /// </summary>
        /// <returns>The retrieved users</returns>
        [HttpGet("/users", Name = "Get all Users")]
        [Produces("application/json")]
        public async Task<IEnumerable<UserResource>> GetAllAsync()
        {
            var users = await _userService.ListAsync();
            var resources = _mapper.Map<IEnumerable<User>, IEnumerable<UserResource>>(users);
            return resources;
        }

    }
}