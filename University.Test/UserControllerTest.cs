using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
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
            _service
                .Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User?)null);
            _service
                .Setup(x => x.GetAsync(Guid.Parse("02051bb9-7a52-48df-a6f4-069b80277a69")))
                .ReturnsAsync(new User
                {
                    Id = Guid.Parse("02051bb9-7a52-48df-a6f4-069b80277a69"),
                    FirstName = "Max",
                    LastName = "Mustermann",
                    Authorization = EAuthorization.Administrator
                });

            var _logger = new Mock<ILogger<UserController>>();
            _controller = new UserController(
                logger: _logger.Object,
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

        [Fact]
        public async Task Get_WhenCalled_ReturnsUserById_200()
        {
            // Act
            var response = await _controller.GetAsync(Guid.Parse("02051bb9-7a52-48df-a6f4-069b80277a69"));
            var result = response as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async Task Get_WhenCalled_ReturnsUserById_404()
        {
            // Act
            var response = await _controller.GetAsync(Guid.Parse("76f7735e-66ed-4751-ba38-448560fb4d96"));
            var result = response as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
        }
    }
}