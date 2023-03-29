using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using University.Server.Domain.Models;
using University.Server.Domain.Services.Communication;

namespace University.Server.Domain.Services
{
    public class JwtService : IJwtService
    {
        private readonly ILogger<JwtService> _logger;
        private readonly JwtSecurityTokenHandler _jwtHandler;
        private readonly SigningCredentials _signingCredentials;
        private readonly IUserService _userService;
        private readonly string _issuer;
        private readonly string _audience;

        public JwtService(IConfiguration config, ILogger<JwtService> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;

            _issuer = config["Jwt:Issuer"] ?? throw new ArgumentNullException("Jwt:Issuer");
            _audience = config["Jwt:Audience"] ?? throw new ArgumentNullException("Jwt:Audience");
            var secret = config["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key");
            var securityKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(secret));
            _signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            _jwtHandler = new JwtSecurityTokenHandler();
        }

        public bool IsSelf(ClaimsPrincipal user, User expectedUser)
        {
            return user.HasClaim("firstName", expectedUser.FirstName)
                && user.HasClaim("lastName", expectedUser.LastName)
                && user.HasClaim("email", expectedUser.Email)
                && user.HasClaim("authorization", expectedUser.Authorization.ToString());
        }

        public async Task<Response<Token>> LoginAsync(string email, string password)
        {
            // Retrieve User
            var user = await _userService.GetUserByCredentials(email, password);
            if (user == null)
            {
                return new Response<Token>(StatusCodes.Status400BadRequest, "Invalid Credentials");
            }
            var claims = new List<Claim>()
            {
                new Claim("sub", user.Id.ToString()),
                new Claim("firstName", user.FirstName),
                new Claim("lastName", user.LastName),
                new Claim("email", user.Email),
                new Claim("authorization", user.Authorization.ToString())
            };

            var jwtSecurityToken = _jwtHandler.CreateJwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                subject: new ClaimsIdentity(claims),
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddHours(6),
                issuedAt: DateTime.Now,
                signingCredentials: _signingCredentials);


            var login = new Token(_jwtHandler.WriteToken(jwtSecurityToken));
            return new Response<Token>(StatusCodes.Status201Created, login);
        }
    }
}
