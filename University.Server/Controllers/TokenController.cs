using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using University.Server.Domain.Models;
using University.Server.Domain.Services;
using University.Server.Extensions;
using University.Server.Resources.Request;
using University.Server.Resources.Response;

namespace JWTAuth.WebApi.Controllers
{
    [ApiController]
    [Route("token")]
    public class TokenController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;

        public TokenController(IJwtService jwtService, IMapper mapper)
        {
            _jwtService = jwtService;
            _mapper = mapper;
        }

        /// <summary>
        /// Creates a new JWT Token
        /// </summary>
        /// <remarks>This endpoint can be used by any user (also unauthenticated) to retrieve a token for authentication.</remarks>
        /// <param name="resource"></param>
        /// <returns>The new created jwt</returns>
        [HttpPost(Name = "Create JWT Token")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(TokenResource))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> PostAsync([FromBody] LoginResource resource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }

            var createdToken = await _jwtService.LoginAsync(resource.Email, resource.Password);
            var createdResource = _mapper.Map<Token, TokenResource>(createdToken);

            return Created("", value: createdResource);
        }
    }
}