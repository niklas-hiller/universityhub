//using University.Server.Domain.Models;
//using University.Server.Domain.Repositories;
//using University.Server.Domain.Services.Communication;

//namespace University.Server.Domain.Services
//{
//    public class TimeTableService : ITimeTableService
//    {
//        private readonly ILogger<TimeTableService> _logger;

//        public TimeTableService(ILogger<TimeTableService> logger)
//        {
//            _logger = logger;
//        }

//        public Dictionary<User, List<Module>> ConstructProfessorModuleMatrix(List<Module> availableModules)
//        {
//            // Create Professor x Module Matrix
//            Dictionary<User, List<Module>> professorModules = new();
//            availableModules.ForEach(module =>
//            {
//                module.AvailableProfessors.ForEach(professor =>
//                {
//                    if (!professorModules.ContainsKey(professor))
//                    {
//                        professorModules[professor] = new List<Module>();
//                    }
//                    if (!professorModules[professor].Contains(module))
//                    {
//                        professorModules[professor].Add(module);
//                    }
//                });
//            });
//            return professorModules;
//        }

//        public List<SemesterModule> CalculateProfessors(List<Module> availableModules)
//        {
//            // Create Professor x Module Matrix
//            Dictionary<User, List<Module>> professorModules = ConstructProfessorModuleMatrix(availableModules);

//            // Calculate which Professor does each Module
//            Dictionary<User, int> professorAssignedModules = new();
//            foreach (KeyValuePair<User, List<Module>> professorModule in professorModules)
//            {
//                professorAssignedModules.Add(professorModule.Key, 0);
//            }

//            List<SemesterModule> semesterModules = new();
//            availableModules.ForEach(module =>
//            {
//                var sortedProfessorModules = professorModules
//                    .OrderBy(p => p.Value.Count())
//                    .OrderBy(p => professorAssignedModules[p.Key]);
//                foreach (KeyValuePair<User, List<Module>> professorModule in sortedProfessorModules)
//                {
//                    if (professorModule.Value.Contains(module))
//                    {
//                        SemesterModule semesterModule = new SemesterModule() 
//                        { 
//                            Professor = professorModule.Key,
//                            ReferenceModule = module
//                        };
//                        professorAssignedModules[professorModule.Key] += 1;
//                        break;
//                    };
//                }
//            });

//            return semesterModules;
//        }

//        public List<SemesterModule> CalculateLectures(List<SemesterModule> semesterModules, List<Course> courses)
//        {
//            // Take CompulsoryModules of Courses in consideration
//        }

//        public Semester CalculateSemester(List<Module> modules, List<Course> courses)
//        {
//            Semester semester = new Semester()
//            {
//                StartDate = new DateTime(2022, 10, 1),
//                EndDate = new DateTime(2023, 3, 31)
//            };
//            List<SemesterModule> semesterModules = CalculateProfessors(modules);
//            List<SemesterModule> semesterModules2 = CalculateLectures(semesterModules, courses);
//            semester.Modules = semesterModules2;
//            return semester;
//        }

//        public Dictionary<Module, User> CalculateTimetable(List<Module> availableModules, List<Course> courses)
//        {
//            // Create Module x Module Matrix
//            Dictionary<Module, List<Module>> simultaneouslyModules = new();
//            availableModules.ForEach(module =>
//            {
//                simultaneouslyModules[module] = new List<Module>(availableModules);
//                courses.ForEach(course =>
//                {
//                    if (course.CompulsoryModules.Contains(module))
//                    {
//                        course.CompulsoryModules.ForEach(compulsoryModule => 
//                        {
//                            simultaneouslyModules[module].Remove(compulsoryModule);
//                        });
//                    }
//                });
//            });

//            // Create Professor x Module Matrix
//            Dictionary<User, List<Module>> professorModules = new();
//            availableModules.ForEach(module =>
//            {
//                module.AvailableProfessors.ForEach(professor => 
//                {
//                    if (!professorModules.ContainsKey(professor))
//                    {
//                        professorModules[professor] = new List<Module>();
//                    }
//                    if (!professorModules[professor].Contains(module))
//                    {
//                        professorModules[professor].Add(module);
//                    }
//                });
//            });

//            // Create Professor (Module) x Module Matrix (removed)
//            Dictionary <User, Dictionary<Module, List<Module>>> fusedModules = new();
//            foreach(KeyValuePair<User, List<Module>> professorModule in professorModules) 
//            {
//                fusedModules[professorModule.Key] = new();
//                professorModule.Value.ForEach(module =>
//                {
//                    fusedModules[professorModule.Key][module] = simultaneouslyModules[module];
//                });
//            }

//            // Calculate which Professor does each Module
//            Dictionary<User, int> professorAssignedModules = new();
//            foreach (KeyValuePair<User, List<Module>> professorModule in professorModules)
//            {
//                professorAssignedModules.Add(professorModule.Key, 0);
//            }

//            Dictionary<Module, User> moduleProfessors = new();
//            availableModules.ForEach(module =>
//            {
//                var sortedProfessorModules = professorModules
//                    .OrderBy(p => p.Value.Count())
//                    .OrderBy(p => professorAssignedModules[p.Key]);
//                foreach(KeyValuePair<User, List<Module>> professorModule in sortedProfessorModules)
//                {
//                    if (professorModule.Value.Contains(module))
//                    {
//                        moduleProfessors.Add(module, professorModule.Key);
//                        professorAssignedModules[professorModule.Key] += 1;
//                        break;
//                    };
//                }
//            });

//            return moduleProfessors;
//        }
//    }
//}
