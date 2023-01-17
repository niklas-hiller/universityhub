using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using University.Server.Controllers;
using University.Server.Domain.Models;
using University.Server.Domain.Services;
using University.Server.Domain.Services.Communication;
using University.Server.Mapping;
using University.Server.Resources;

namespace University.Test
{
    public class TimeTableServiceTest
    {
        private readonly TimeTableService _service;

        public TimeTableServiceTest()
        {
            var _logger = new Mock<ILogger<TimeTableService>>();
            _service = new TimeTableService(
                logger: _logger.Object
            );
        }

        [Fact]
        public void CalculateTimeTable()
        {
            // Prepare
            var profA = new User()
            {
                FirstName = "Prof A"
            };
            var profB = new User()
            {
                FirstName = "Prof B"
            };

            var moduleA = new Module()
            {
                Name = "Module A",
                AvailableProfessors = new()
                {
                    profB
                }
            };
            var moduleB = new Module()
            {
                Name = "Module B",
                AvailableProfessors = new()
                {
                    profA,
                    profB
                }
            };
            var moduleC = new Module()
            {
                Name = "Module C",
                AvailableProfessors = new()
                {
                    profA
                }
            };
            var courseA = new Course()
            {
                CompulsoryModules = new()
                {
                    moduleB,
                    moduleC
                }
            };
            var courseB = new Course()
            {
                CompulsoryModules = new()
                {
                    moduleA
                }
            };

            List<Module> availableModules = new List<Module>()
            {
                moduleA,
                moduleB,
                moduleC
            };
            List<Course> courses = new List<Course>()
            {
                courseA,
                courseB
            };

            // Act
            Dictionary<Module, User> result = _service.CalculateTimetable(availableModules, courses);

            Assert.NotNull(result);
        }
    }
}