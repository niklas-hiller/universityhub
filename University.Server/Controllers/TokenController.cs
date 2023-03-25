using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using University.Server.Domain.Models;
using University.Server.Domain.Services;
using University.Server.Extensions;
using University.Server.Resources;

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
        /// <param name="resource"></param>
        /// <returns>The new created jwt</returns>
        [HttpPost(Name = "Create JWT Token")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(TokenResource))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostAsync([FromBody] LoginResource resource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }

            var result = await _jwtService.LoginAsync(resource.Email, resource.Password);

            switch (result.StatusCode)
            {
                case StatusCodes.Status201Created:
                    if (result.ResponseEntity == null)
                    {
                        return StatusCode(500);
                    }
                    var createdResource = _mapper.Map<Token, TokenResource>(result.ResponseEntity);
                    return Created("", value: createdResource);
                default:
                    return StatusCode(result.StatusCode, result.Message);
            }
        }
    }
}