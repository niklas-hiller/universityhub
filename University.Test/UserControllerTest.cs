using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using University.Server.Controllers;
using University.Server.Domain.Models;
using University.Server.Domain.Services;
using University.Server.Mapping;
using University.Server.Resources;

namespace University.Test
{
    public class UserControllerTest
    {
        private readonly UserController _controller;
        private readonly Mock<IUserService> _service;
        private readonly IMapper _mapper;

        public UserControllerTest()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ModelToResourceProfile>();
                cfg.AddProfile<ResourceToModelProfile>();
            });
            _mapper = config.CreateMapper();

            using var loggerFactory = LoggerFactory.Create(loggingBuilder =>
                loggingBuilder
                .SetMinimumLevel(LogLevel.Trace)
                .AddConsole());
            
            _service = new Mock<IUserService>();
            _service
                .Setup(x => x.ListAsync())
                .ReturnsAsync(new List<User>
                {
                    new User {
                        Id = Guid.NewGuid(),
                        FirstName = "Max",
                        LastName = "Mustermann",
                        Authorization = EAuthorization.Administrator
                    },
                    new User {
                        Id = Guid.NewGuid(),
                        FirstName = "Bob",
                        LastName = "Mustermann",
                        Authorization = EAuthorization.Student
                    },
                    new User {
                        Id = Guid.NewGuid(),
                        FirstName = "Frank",
                        LastName = "Mustermann",
                        Authorization = EAuthorization.Student
                    },
                }.AsEnumerable());

            _controller = new UserController(
                logger: loggerFactory.CreateLogger<UserController>(),
                userService: _service.Object,
                mapper: _mapper);
        }

        [Fact]
        public async Task Get_WhenCalled_ReturnsAllUsers()
        {
            // Act
            var users = await _controller.GetAllAsync();

            // Assert
            Assert.NotNull(users);
            Assert.IsAssignableFrom<IEnumerable<UserResource>>(users);
            Assert.Equal(3, users.ToList().Count);
        }
    }
}