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
                default:
                    return StatusCode(result.StatusCode, result.Message);
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
                default:
                    return StatusCode(result.StatusCode, result.Message);
            }
        }

        /// <summary>
        /// Updates a User Credentials
        /// </summary>
        /// <param name="id"></param>
        /// <param name="resource"></param>
        /// <returns>The updated user</returns>
        [HttpPut("{id}/credentials", Name = "Update User Credentials")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResource))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutCredentialsAsync(Guid id, [FromBody] UpdateUserCredentialsResource resource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }
            var user = _mapper.Map<UpdateUserCredentialsResource, User>(resource);
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
                default:
                    return StatusCode(result.StatusCode, result.Message);
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
                var response = await _userService.GetAsync(id);

                if (response.ResponseEntity == null) 
                {
                    return NotFound("Couldn't find requested user.");
                }
                var user = response.ResponseEntity;

                if (user.Authorization != EAuthorization.Student)
                {
                    return BadRequest("You can only manipulate assignments of Students.");
                }

                foreach (Module module in patch.AddEntity.Union(patch.RemoveEntity))
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
                default:
                    return StatusCode(result.StatusCode, result.Message);
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
            var result = await _userService.GetAsync(id);

            switch (result.StatusCode)
            {
                case StatusCodes.Status200OK:
                    if (result.ResponseEntity == null)
                    {
                        return StatusCode(500);
                    }
                    var retrievedResource = _mapper.Map<User, UserResource>(result.ResponseEntity);
                    return Ok(retrievedResource);
                default:
                    return StatusCode(result.StatusCode, result.Message);
            }
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
                default:
                    return StatusCode(result.StatusCode, result.Message);
            }
        }

    }
}