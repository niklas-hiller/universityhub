using Microsoft.IdentityModel.Tokens;
using University.Server.Domain.Models;
using University.Server.Domain.Persistence.Entities;
using University.Server.Domain.Repositories;
using University.Server.Exceptions;

namespace University.Server.Domain.Services
{
    public class SemesterService : ISemesterService
    {
        private readonly ILogger<SemesterService> _logger;
        private readonly ICosmosDbRepository<Semester, SemesterEntity> _semesterRepository;
        private readonly IUserService _userService;
        private readonly ILocationService _locationService;
        private readonly ICourseService _courseService;

        public SemesterService(ILogger<SemesterService> logger, ICosmosDbRepository<Semester, SemesterEntity> semesterRepository,
            IUserService userService, ILocationService locationService, ICourseService courseService)
        {
            _logger = logger;
            _semesterRepository = semesterRepository;
            _userService = userService;
            _locationService = locationService;
            _courseService = courseService;
        }

        private async Task<Dictionary<SemesterModule, User>> CalculateProfessors(Semester semester)
        {
            CalculationTable<User, Module> calculationTable = new CalculationTable<User, Module>();
            IEnumerable<User> professors = await _userService.GetManyAsync(semester.Modules.SelectMany(module => module.ReferenceModule.ProfessorIds).ToList());

            // Insert Professor + Module in Calculation Table
            foreach (var semesterModule in semester.Modules)
            {
                semesterModule.ReferenceModule.ProfessorIds.ToList().ForEach(id =>
                {
                    User professor = professors.First(professor => professor.Id == id) ?? throw new NullReferenceException();
                    calculationTable.InsertData(professor, semesterModule.ReferenceModule);
                });
            }

            // Start Calculation
            bool success = calculationTable.Calculate(semester.Modules.Select(module => module.ReferenceModule).ToList());
            if (!success)
            {
                throw new InvalidDataException("There was an error when calculating which professor to assign to each module of the semester. Does every module have atleast one professor?");
            }

            // Assign Professor to the actual semester module
            Dictionary<SemesterModule, User> result = new Dictionary<SemesterModule, User>();
            calculationTable.Rows.ForEach(row =>
            {
                foreach (var assigned in row.Assigned)
                {
                    var semesterModule = semester.Modules.First(module => module.ReferenceModule == assigned);
                    var professor = row.Target;
                    result.Add(semesterModule, professor);
                }
            });

            return result;
        }

        private async Task<Dictionary<SemesterModule, Location>> CalculateLocations(Semester semester)
        {
            CalculationTable<Location, Module> calculationTable = new CalculationTable<Location, Module>();
            List<Location> locations = (await _locationService.ListAsync()).ToList();

            // Insert Location + Module in Calculation Table
            foreach (var semesterModule in semester.Modules)
            {
                locations.ForEach(location =>
                {
                    if (location.Size >= semesterModule.ReferenceModule.MaxSize)
                    {
                        calculationTable.InsertData(location, semesterModule.ReferenceModule);
                    };
                });
            }

            // Start Calculation
            bool success = calculationTable.Calculate(semester.Modules.Select(module => module.ReferenceModule).ToList());
            if (!success)
            {
                throw new InvalidDataException("There was an error when calculating which location to assign to each module of the semester. Does every module have atleast one location with enough size?");
            }

            // Assign Location to the actual semester module
            Dictionary<SemesterModule, Location> result = new Dictionary<SemesterModule, Location>();
            calculationTable.Rows.ForEach(row =>
            {
                foreach (var assigned in row.Assigned)
                {
                    var semesterModule = semester.Modules.First(module => module.ReferenceModule == assigned);
                    var location = row.Target;
                    result.Add(semesterModule, location);
                }
            });

            return result;
        }

        private async Task<Dictionary<SemesterModule, IEnumerable<Lecture>>> CalculateLectures(Semester semester,
            Dictionary<SemesterModule, User> professorAssignments, Dictionary<SemesterModule, Location> locationAssignments)
        {
            const int CREDIT_POINT_FACTOR = 3;
            const int LECTURE_DURATION = 180;

            CalculationTable<SemesterModule, Lecture> calculationTable = new CalculationTable<SemesterModule, Lecture>();

            var courses = await _courseService.ListAsync();

            var totalLectures = semester.Modules.Sum(module => module.ReferenceModule.CreditPoints) * CREDIT_POINT_FACTOR;
            List<Lecture> lectures = new List<Lecture>();
            for (int i = 0; i < totalLectures; i++)
            {
                lectures.Add(new Lecture());
            }
            _logger.LogInformation($"Generating {lectures.Count} Lectures...");

            // Insert Location + Module in Calculation Table
            foreach (var semesterModule in semester.Modules)
            {
                lectures.ForEach(lecture =>
                {
                    calculationTable.InsertData(semesterModule, lecture);
                });
            }

            // Start Calculation
            var currentModules = new List<SemesterModule>(); // List of Modules that are already used in current timeframe
            var currentTime = semester.StartDate.Date;   // Current timeframe
            var lectureDuration = new TimeSpan(0, LECTURE_DURATION, 0);
            var lectureOffset = new TimeSpan(0, 10, 0); // Time between lecture timeslots
            var startDay = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 9, 0, 0); // First lecture starts at 9 AM
            var endDay = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 17, 0, 0); // Last lecture starts at 5 PM
            _logger.LogInformation($"Semester start at: {semester.StartDate}");
            _logger.LogInformation($"Semester ends at: {semester.EndDate}");
            bool success = calculationTable.Calculate(lectures, (row, lecture) =>
            {
                // Confirm that the module does not have all it's lectures yet
                if (row.Assigned.Count >= row.Target.ReferenceModule.CreditPoints * CREDIT_POINT_FACTOR)
                {
                    return false;
                }
                // Confirm that the professor is available
                if (currentModules.Any(currentModule => professorAssignments[currentModule] == professorAssignments[row.Target]))
                {
                    return false;
                }
                // Confirm that the room is available
                if (currentModules.Any(currentModule => locationAssignments[currentModule] == locationAssignments[row.Target]))
                {
                    return false;
                }
                // Confirm that the course that has this module, is available
                if (currentModules.Any(currentModule => courses.Any(course => course.Modules.Contains(row.Target.ReferenceModule) && course.Modules.Contains(currentModule.ReferenceModule))))
                {
                    return false;
                }
                currentModules.Add(row.Target);
                return true;
            }, (assignableObject, iterationSuccess) =>
            {
                if (iterationSuccess)
                {
                    _logger.LogInformation($"Found possible module for this timeframe! ({currentTime})");
                    assignableObject.Duration = LECTURE_DURATION;
                    assignableObject.Date = currentTime.Date;
                }
                else
                {
                    _logger.LogInformation($"Couldn't find any more possible modules for this timeframe. ({currentTime})");
                    // Clean up list
                    currentModules.Clear();
                    // Go to next timeslot
                    currentTime = currentTime.Add(lectureDuration);
                    currentTime = currentTime.Add(lectureOffset);
                    if (currentTime.TimeOfDay > endDay.TimeOfDay)
                    {
                        // Add one day, and then substract again the difference to the start of day.
                        currentTime = currentTime.AddDays(1).Subtract(currentTime.TimeOfDay - startDay.TimeOfDay);
                    }
                }

                // If the iteration was successful, there are no more lectures, if it failed, there are still lectures
                // true = assign again, false = do not assign again
                return !iterationSuccess;
            });
            if (!success)
            {
                throw new InvalidDataException("There was an error when calculating the lectures of each module this semester.");
            }

            // Assign Location to the actual semester module
            Dictionary<SemesterModule, IEnumerable<Lecture>> result = new Dictionary<SemesterModule, IEnumerable<Lecture>>();
            calculationTable.Rows.ForEach(row =>
            {
                result.Add(row.Target, row.Assigned);
            });

            return result;
        }

        public async Task<Semester> CalculateAsync(Guid id)
        {
            var semester = await GetAsync(id);

            if (semester.Active)
                throw new BadRequestException("Semester already active.");

            if (semester.Modules.Count == 0)
                throw new BadRequestException("Semester has no modules.");

            try
            {
                // Calculation of Professors and Locations
                Dictionary<SemesterModule, User> professorAssignments = await CalculateProfessors(semester);
                Dictionary<SemesterModule, Location> locationAssignments = await CalculateLocations(semester);
                _logger.LogInformation($"Calculated following professor assignments: {professorAssignments}");
                _logger.LogInformation($"Calculated following location assignments: {locationAssignments}");

                // Calculation of Lecture Date (Requires Professors and Locations)
                Dictionary<SemesterModule, IEnumerable<Lecture>> lectureAssignments = await CalculateLectures(semester, professorAssignments, locationAssignments);
                _logger.LogInformation($"Calculated following lecture assignments: {lectureAssignments}");

                // Update Semester Object
                semester.Active = true;
                foreach (var semesterModule in semester.Modules)
                {
                    // Assing professors
                    semesterModule.Professor = professorAssignments[semesterModule];
                    // Assign lectures
                    semesterModule.Lectures = lectureAssignments[semesterModule].ToList();
                    // Assign locations
                    foreach (var lecture in semesterModule.Lectures)
                    {
                        lecture.CreatedAt = DateTime.Now;
                        lecture.UpdatedAt = DateTime.Now;
                        lecture.Location = locationAssignments[semesterModule];
                    }
                }
            }
            catch (InvalidDataException ex)
            {
                throw new BadRequestException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new InternalServerException(ex.Message);
            }

            // Update Database
            try
            {
                await _semesterRepository.UpdateItemAsync(semester.Id, semester);

                return semester;
            }
            catch (Microsoft.Azure.Cosmos.CosmosException ex)
            {
                throw RequestException.ResolveCosmosException(ex, id);
            }
            catch (Exception ex)
            {
                throw new InternalServerException($"An error occurred when activating the semester: {ex.Message}");
            }
        }

        public async Task<Semester> SaveAsync(Semester semester)
        {
            _logger.LogInformation("Attempting to save new semester...");

            semester.Modules = new List<SemesterModule>();

            try
            {
                await _semesterRepository.AddItemAsync(semester);

                return semester;
            }
            catch (Microsoft.Azure.Cosmos.CosmosException ex)
            {
                throw RequestException.ResolveCosmosException(ex);
            }
            catch (Exception ex)
            {
                throw new InternalServerException($"An error occurred when saving the semester: {ex.Message}");
            }
        }

        public async Task<IEnumerable<Semester>> GetManyAsync(IEnumerable<Guid> ids)
        {
            if (ids.IsNullOrEmpty())
            {
                return Enumerable.Empty<Semester>();
            }
            var query = $"SELECT * FROM c WHERE c.id IN ('{string.Join("', '", ids)}')";
            try
            {
                return await _semesterRepository.GetItemsAsync(query);
            }
            catch (Microsoft.Azure.Cosmos.CosmosException ex)
            {
                _logger.LogInformation($"Cosmos DB Exception for: {query})");
                throw ex;
            }
        }

        public async Task<IEnumerable<Semester>> GetManyAsyncByTime(DateTime containsDate, TimeSpan? delta = null)
        {
            TimeSpan offset = delta ?? TimeSpan.Zero;
            var query = $"SELECT * FROM c WHERE c.StartDate <= '{containsDate.Add(offset).ToString("yyyy-MM-dd HH:mm:ss.fff")}' AND c.EndDate >= '{containsDate.Add(offset.Negate()).ToString("yyyy-MM-dd HH:mm:ss.fff")}'";
            try
            {
                return await _semesterRepository.GetItemsAsync(query);
            }
            catch (Microsoft.Azure.Cosmos.CosmosException ex)
            {
                _logger.LogInformation($"Cosmos DB Exception for: {query})");
                throw ex;
            }
        }

        public async Task<Semester?> GetAsyncNullable(Guid id)
        {
            try
            {
                var semester = await _semesterRepository.GetItemAsync(id);

                return semester;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<Semester> GetAsync(Guid id)
        {
            _logger.LogInformation("Attempting to retrieve existing semester...");

            try
            {
                var semester = await _semesterRepository.GetItemAsync(id);

                return semester;
            }
            catch (Microsoft.Azure.Cosmos.CosmosException ex)
            {
                throw RequestException.ResolveCosmosException(ex, id);
            }
            catch (Exception ex)
            {
                throw new InternalServerException($"An error occurred when retrieving the semester: {ex.Message}");
            }
        }

        public async Task<IEnumerable<Semester>> ListAsync()
        {
            _logger.LogInformation("Attempting to retrieve existing semester...");

            return await _semesterRepository.GetItemsAsync("SELECT * FROM c");
        }

        public async Task<Semester> PatchModulesAsync(Guid id, PatchModel<Module> patch)
        {
            var existingSemester = await GetAsync(id);

            if (existingSemester.Active)
                throw new BadRequestException("Can't add/remove modules to active semester.");

            _logger.LogInformation($"Initiating adding {patch.AddEntity.Count} modules to semester...");
            foreach (var add in patch.AddEntity)
            {
                if (!existingSemester.Modules.Any(x => x.ReferenceModule.Id == add.Id))
                {
                    var semesterModule = new SemesterModule()
                    {
                        Id = Guid.NewGuid(),
                        Professor = null,
                        ReferenceModule = add,
                    };
                    existingSemester.Modules.Add(semesterModule);
                }
            }

            _logger.LogInformation($"Initiating removing {patch.RemoveEntity.Count} modules from semester...");
            foreach (var remove in patch.RemoveEntity)
            {
                if (existingSemester.Modules.Any(x => x.Id == remove.Id))
                {
                    var semesterModule = existingSemester.Modules.First(x => x.ReferenceModule.Id == remove.Id);
                    existingSemester.Modules.Remove(semesterModule);
                }
            }

            try
            {
                await _semesterRepository.UpdateItemAsync(existingSemester.Id, existingSemester);

                return existingSemester;
            }
            catch (Microsoft.Azure.Cosmos.CosmosException ex)
            {
                throw RequestException.ResolveCosmosException(ex, id);
            }
            catch (Exception ex)
            {
                throw new InternalServerException($"An error occurred when updating the semester: {ex.Message}");
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            _logger.LogInformation("Attempting to delete existing semester...");

            try
            {
                await _semesterRepository.DeleteItemAsync(id);
            }
            catch (Microsoft.Azure.Cosmos.CosmosException ex)
            {
                throw RequestException.ResolveCosmosException(ex, id);
            }
            catch (Exception ex)
            {
                throw new InternalServerException($"An error occurred when deleting the semester: {ex.Message}");
            }
        }

        public async Task<IEnumerable<SemesterModule>> GetActiveSemesterModulesOfUser(Guid id)
        {
            var existingUser = await _userService.GetAsync(id);

            if (existingUser.Assignments.IsNullOrEmpty())
            {
                _logger.LogInformation($"User {id} did not have any assignments!");
                return Enumerable.Empty<SemesterModule>();
            }

            var activeAssignmentIds = existingUser.Assignments
                .Where(assignment => assignment.Status == EModuleStatus.Enrolled || assignment.Status == EModuleStatus.Educates)
                .Select(assignment => assignment.ReferenceModule.Id);

            var activeSemesters = await GetManyAsyncByTime(DateTime.Now, new TimeSpan(30, 0, 0, 0));
            if (activeSemesters.IsNullOrEmpty())
            {
                _logger.LogInformation("Couldn't find any active semester!");
                return Enumerable.Empty<SemesterModule>();
            }

            var semesterModules = activeSemesters
                .SelectMany(semester => semester.Modules)
                .Where(semesterModule => activeAssignmentIds.Contains(semesterModule.ReferenceModule.Id));

            return semesterModules;
        }
    }
}