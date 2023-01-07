using Microsoft.AspNetCore.Mvc;
using University.Server.Domain.Models;
using University.Server.Domain.Services;

namespace University.Server.Controllers
{
    [ApiController]
    [Route("/api/v1/[controller]")]
    public class UserController : Controller
    {

        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;

        public UserController(ILogger<UserController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        /// <summary>
        /// Retrieves a specific User by his id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The retrieved user</returns>
        [HttpGet("/users/{id}", Name = "Get User By Id")]
        [Produces("application/json")]
        public async Task<User?> GetAsync(Guid id)
        {
            return await _userService.GetAsync(id);
        }

        /// <summary>
        /// Retrieves a all users
        /// </summary>
        /// <returns>The retrieved users</returns>
        [HttpGet("/users", Name = "Get all Users")]
        [Produces("application/json")]
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var users = await _userService.ListAsync();
            return users;
        }

    }
}