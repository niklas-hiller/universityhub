using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using University.Server.Domain.Models;
using University.Server.Domain.Services;
using University.Server.Extensions;
using University.Server.Filters;
using University.Server.Resources.Request;
using University.Server.Resources.Response;

namespace University.Server.Controllers
{
    [ApiController]
    [Route("users")]
    [Authorize]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;
        private readonly ISemesterService _semesterService;
        private readonly IMapper _mapper;

        public UserController(ILogger<UserController> logger, IUserService userService, ISemesterService semesterService, IMapper mapper)
        {
            _logger = logger;
            _userService = userService;
            _semesterService = semesterService;
            _mapper = mapper;
        }

        /// <summary>
        /// Creates a new User
        /// </summary>
        /// <remarks>This endpoint can only be used by Administrators.</remarks>
        /// <param name="resource"></param>
        /// <returns>The new created user</returns>
        [HttpPost(Name = "Create User")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UserResource))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [Permission(EAuthorization.Administrator)]
        public async Task<IActionResult> PostAsync([FromBody] SaveUserResource resource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }

            var user = _mapper.Map<SaveUserResource, User>(resource);
            var createdUser = await _userService.SaveAsync(user);

            var createdResource = _mapper.Map<User, UserResource>(createdUser);
            return Created("", value: createdResource);
        }

        /// <summary>
        /// Updates a Assignment of a User
        /// </summary>
        /// <remarks>This endpoint can only be used by Administrators.</remarks>
        /// <param name="id">The id of the user that should be updated.</param>
        /// <param name="id2">The id of the assignment that should be updated.</param>
        /// <param name="resource"></param>
        /// <returns>The updated user</returns>
        [HttpPut("{id}/assignments/{id2}", Name = "Updates a Assignment of a User")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [Permission(EAuthorization.Administrator)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResource))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> PutAssignmentsAsync(Guid id, Guid id2, [FromBody] UpdateAssignmentResource resource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }

            var assignment = _mapper.Map<UpdateAssignmentResource, Assignment>(resource);
            var updatedUser = await _userService.UpdateAssignmentAsync(id, id2, assignment);

            var updatedResource = _mapper.Map<User, UserResource>(updatedUser);
            return Ok(updatedResource);
        }

        /// <summary>
        /// Updates a User
        /// </summary>
        /// <remarks>This endpoint can only be used by target user or administrators.</remarks>
        /// <param name="id">The id of the user that should be updated.</param>
        /// <param name="resource"></param>
        /// <returns>The updated user</returns>
        [HttpPut("{id}", Name = "Update User")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResource))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpdateUserResource resource)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            if (!(userId == id.ToString() || User.HasClaim("authorization", EAuthorization.Administrator.ToString())))
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }
            var user = _mapper.Map<UpdateUserResource, User>(resource);
            var updatedUser = await _userService.UpdateAsync(id, user);

            var updatedResource = _mapper.Map<User, UserResource>(updatedUser);
            return Ok(updatedResource);
        }

        /// <summary>
        /// Updates a User Credentials
        /// </summary>
        /// <remarks>This endpoint can only be used by target user or administrators.</remarks>
        /// <param name="id">The id of the user that should be updated.</param>
        /// <param name="resource"></param>
        /// <returns>The updated user</returns>
        [HttpPut("{id}/credentials", Name = "Update User Credentials")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResource))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> PutCredentialsAsync(Guid id, [FromBody] UpdateUserCredentialsResource resource)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            if (!(userId == id.ToString() || User.HasClaim("authorization", EAuthorization.Administrator.ToString())))
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }
            var user = _mapper.Map<UpdateUserCredentialsResource, User>(resource);
            var updatedUser = await _userService.UpdateAsync(id, user);

            var updatedResource = _mapper.Map<User, UserResource>(updatedUser);
            return Ok(updatedResource);
        }

        /// <summary>
        /// Adds/Removes modules to a user (Only Students and only Optional) 
        /// </summary>
        /// <remarks>This endpoint can only be used by target user or administrators.</remarks>
        /// <param name="id">The id of the user that should be updated.</param>
        /// <param name="resource"></param>
        /// <returns>The updated user</returns>
        [HttpPatch("{id}/assignments", Name = "Adds/Removes modules to a user (Only Students and only Optional)")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResource))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> PatchAssignmentsAsync(Guid id, [FromBody] PatchResource resource)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            if (!(userId == id.ToString() || User.HasClaim("authorization", EAuthorization.Administrator.ToString())))
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }
            var patch = _mapper.Map<PatchResource, PatchModel<Module>>(resource);

            // Validation
            {
                var user = await _userService.GetAsync(id);

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

            var updatedUser = await _userService.PatchAssignmentsAsync(id, patch);

            var updatedResource = _mapper.Map<User, UserResource>(updatedUser);
            return Ok(updatedResource);
        }

        /// <summary>
        /// Retrieves a specific User by his id
        /// </summary>
        /// <remarks>This endpoint can be used by any authenticated user.</remarks>
        /// <param name="id">The id of the user that should be retrieved.</param>
        /// <returns>The retrieved user</returns>
        [HttpGet("{id}", Name = "Get User By Id")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResource))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetAsync(Guid id)
        {
            var retrievedUser = await _userService.GetAsync(id);

            var retrievedResource = _mapper.Map<User, UserResource>(retrievedUser);
            return Ok(retrievedResource);
        }

        /// <summary>
        /// Retrieves all users matching filter
        /// </summary>
        /// <remarks>This endpoint can be used by any authenticated user.</remarks>
        /// <param name="authorization">The authorization the retrieved users should have. If left empty, will retrieve all users.</param>
        /// <returns>The retrieved users</returns>
        [HttpGet(Name = "Get all Users matching filter")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UserResource>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IEnumerable<UserResource>> GetFilteredAsync(EAuthorization? authorization)
        {
            var users = await _userService.ListAsync(authorization);
            var resources = _mapper.Map<IEnumerable<User>, IEnumerable<UserResource>>(users);
            return resources;
        }

        /// <summary>
        /// Retrieves lectures of a specific User by his id
        /// </summary>
        /// <remarks>This endpoint can be used by any authenticated user.</remarks>
        /// <param name="id">The id of the user that the lectures should be retrieved of.</param>
        /// <returns>The retrieved lectures</returns>
        [HttpGet("{id}/lectures", Name = "Get Lectures of User By Id")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ExtendedLectureResource>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IEnumerable<ExtendedLectureResource>> GetLecturesAsync(Guid id)
        {
            var semesterModules = await _semesterService.GetActiveSemesterModulesOfUser(id);
            var resources = semesterModules.SelectMany(_mapper.Map<SemesterModule, IEnumerable<ExtendedLectureResource>>);
            return resources;
        }

        /// <summary>
        /// Deletes a specific User by his id
        /// </summary>
        /// <remarks>This endpoint can only be used by Administrators.</remarks>
        /// <param name="id">The id of the user that should be deleted.</param>
        [HttpDelete("{id}", Name = "Delete User By Id")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [Permission(EAuthorization.Administrator)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await _userService.DeleteAsync(id);

            return NoContent();
        }

    }
}