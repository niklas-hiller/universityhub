using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using University.Server.Domain.Models;
using University.Server.Domain.Services;
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
            if (user == null) {
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