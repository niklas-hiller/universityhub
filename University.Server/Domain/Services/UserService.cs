using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using University.Server.Domain.Models;
using University.Server.Domain.Persistence.Entities;
using University.Server.Domain.Repositories;
using University.Server.Domain.Services.Communication;

namespace University.Server.Domain.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly ICosmosDbRepository<User, UserEntity> _userRepository;
        private readonly JwtSecurityTokenHandler _jwtHandler;
        private readonly SigningCredentials _signingCredentials;
        private readonly string _issuer;
        private readonly string _audience;

        public UserService(IConfiguration config, ILogger<UserService> logger, ICosmosDbRepository<User, UserEntity> userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;

            _issuer = config["Jwt:Issuer"] ?? throw new ArgumentNullException("Jwt:Issuer");
            _audience = config["Jwt:Audience"] ?? throw new ArgumentNullException("Jwt:Audience");
            var secret = config["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key");
            var securityKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(secret));
            _signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            _jwtHandler = new JwtSecurityTokenHandler();
        }

        private string Sha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public async Task<Response<Token>> LoginAsync(string email, string password)
        {
            // Retrieve User
            var users = await _userRepository.GetItemsAsync($"SELECT * FROM c WHERE c.Email = '{email}' AND c.Password = '{Sha256Hash(password)}'");
            var user = users.FirstOrDefault();
            if (user == null)
            {
                return new Response<Token>(StatusCodes.Status400BadRequest, "Invalid Credentials");
            }
            var claims = new List<Claim>()
            {
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

        public async Task<Response<User>> SaveAsync(User user)
        {
            _logger.LogInformation("Attempting to save new user...");
            user.Password = Sha256Hash("testpassword123");

            try
            {
                await _userRepository.AddItemAsync(user);

                return new Response<User>(StatusCodes.Status201Created, user);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new Response<User>(StatusCodes.Status400BadRequest, $"An error occurred when saving the user: {ex.Message}");
            }
        }

        public async Task<User?> GetAsync(Guid id)
        {
            _logger.LogInformation("Attempting to retrieve existing user...");

            return await _userRepository.GetItemAsync(id);
        }

        public async Task<IEnumerable<User>> ListAsync(EAuthorization? authorization)
        {
            _logger.LogInformation("Attempting to retrieve existing users...");

            if (authorization != null)
            {
                return await _userRepository.GetItemsAsync($"SELECT * FROM c WHERE c.Authorization = '{authorization}'");
            }
            else
            {
                return await _userRepository.GetItemsAsync($"SELECT * FROM c");
            }
        }

        public async Task<Response<User>> UpdateAsync(Guid id, User user)
        {
            _logger.LogInformation("Attempting to update existing user...");

            var existingUser = await _userRepository.GetItemAsync(id);

            if (existingUser == null)
                return new Response<User>(StatusCodes.Status404NotFound, "User not found.");

            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Email = user.Email;
            existingUser.Password = Sha256Hash(user.Password);

            try
            {
                await _userRepository.UpdateItemAsync(existingUser.Id, existingUser);

                return new Response<User>(StatusCodes.Status200OK, existingUser);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new Response<User>(StatusCodes.Status400BadRequest, $"An error occurred when updating the user: {ex.Message}");
            }
        }

        public async Task<Response<User>> DeleteAsync(Guid id)
        {
            _logger.LogInformation("Attempting to delete existing user...");

            var existingUser = await _userRepository.GetItemAsync(id);

            if (existingUser == null)
                return new Response<User>(StatusCodes.Status404NotFound, "User not found.");

            try
            {
                await _userRepository.DeleteItemAsync(id);

                return new Response<User>(StatusCodes.Status204NoContent);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new Response<User>(StatusCodes.Status400BadRequest, $"An error occurred when deleting the user: {ex.Message}");
            }
        }
    }
}
