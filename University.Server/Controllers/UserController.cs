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
    [Route("users")]
    [Authorize]
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
        [HttpPost(Name = "Create User")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UserResource))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Permission(EAuthorization.Administrator)]
        public async Task<IActionResult> PostAsync([FromBody] SaveUserResource resource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }

            var user = _mapper.Map<SaveUserResource, User>(resource);
            var result = await _userService.SaveAsync(user);

            switch (result.StatusCode)
            {
                case StatusCodes.Status201Created:
                    if (result.ResponseEntity == null)
                    {
                        return StatusCode(500);
                    }
                    var createdResource = _mapper.Map<User, UserResource>(result.ResponseEntity);
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
        /// Updates a Assignment of a User
        /// </summary>
        /// <param name="id"></param>
        /// <param name="id2"></param>
        /// <param name="resource"></param>
        /// <returns>The updated user</returns>
        [HttpPut("{id}/assignments/{id2}", Name = "Updates a Assignment of a User")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResource))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Obsolete]
        public async Task<IActionResult> PutAssignmentsAsync(Guid id, Guid id2, [FromBody] UpdateAssignmentResource resource)
        {
            // Todo
            return StatusCode(StatusCodes.Status501NotImplemented);
        }

        /// <summary>
        /// Updates a User
        /// </summary>
        /// <param name="id"></param>
        /// <param name="resource"></param>
        /// <returns>The updated user</returns>
        [HttpPut("{id}", Name = "Update User")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResource))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpdateUserResource resource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }
            var user = _mapper.Map<UpdateUserResource, User>(resource);
            var result = await _userService.UpdateAsync(id, user);

            switch (result.StatusCode)
            {
                case StatusCodes.Status200OK:
                    if (result.ResponseEntity == null)
                    {
                        return StatusCode(500);
                    }
                    var updatedResource = _mapper.Map<User, UserResource>(result.ResponseEntity);
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
        /// Adds/Removes modules to a user (Only Students and only Optional) 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="resource"></param>
        /// <returns>The updated user</returns>
        [HttpPatch("{id}/assignments", Name = "Adds/Removes modules to a user (Only Students and only Optional)")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResource))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PatchAssignmentsAsync(Guid id, [FromBody] PatchResource resource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }
            var patch = _mapper.Map<PatchResource, PatchModel<Module>>(resource);

            // Validation (Todo: Only of themself)
            {
                var user = await _userService.GetAsync(id);
                if (user == null)
                {
                    return NotFound("Couldn't find requested user.");
                }

                if (user.Authorization != EAuthorization.Student)
                {
                    return BadRequest("You can only manipulate assignments of Students.");
                }

                foreach (Module module in patch.Add.Union(patch.Remove))
                {
                    if (module.ModuleType == EModuleType.Compulsory)
                    {
                        return BadRequest("You can't add/remove compulsory modules to a specific user. Please use courses.");
                    }
                }
            }

            var result = await _userService.PatchAssignmentsAsync(id, patch);

            switch (result.StatusCode)
            {
                case StatusCodes.Status200OK:
                    if (result.ResponseEntity == null)
                    {
                        return StatusCode(500);
                    }
                    var updatedResource = _mapper.Map<User, UserResource>(result.ResponseEntity);
                    return Ok(updatedResource);
                case StatusCodes.Status400BadRequest:
                    return BadRequest(result.Message);
                default:
                    return StatusCode(500);
            }
        }

        /// <summary>
        /// Retrieves a specific User by his id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The retrieved user</returns>
        [HttpGet("{id}", Name = "Get User By Id")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResource))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAsync(Guid id)
        {
            var user = await _userService.GetAsync(id);
            if (user == null)
            {
                return NotFound($"Couldn't find any user with the id {id}");
            }
            var resource = _mapper.Map<User, UserResource>(user);
            return Ok(resource);
        }

        /// <summary>
        /// Retrieves all users matching filter
        /// </summary>
        /// <param name="authorization"></param>
        /// <returns>The retrieved users</returns>
        [HttpGet(Name = "Get all Users matching filter")]
        [Produces("application/json")]
        public async Task<IEnumerable<UserResource>> GetFilteredAsync(EAuthorization? authorization)
        {
            var users = await _userService.ListAsync(authorization);
            var resources = _mapper.Map<IEnumerable<User>, IEnumerable<UserResource>>(users);
            return resources;
        }

        /// <summary>
        /// Deletes a specific User by his id
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("{id}", Name = "Delete User By Id")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Permission(EAuthorization.Administrator)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var result = await _userService.DeleteAsync(id);

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