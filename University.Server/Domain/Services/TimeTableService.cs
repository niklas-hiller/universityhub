using University.Server.Domain.Models;
using University.Server.Domain.Repositories;
using University.Server.Domain.Services.Communication;

namespace University.Server.Domain.Services
{
    public class TimeTableService : ITimeTableService
    {
        private readonly ILogger<TimeTableService> _logger;

        public TimeTableService(ILogger<TimeTableService> logger)
        {
            _logger = logger;
        }

        public Dictionary<Module, User> CalculateTimetable(List<Module> availableModules, List<Course> courses)
        {
            // Create Module x Module Matrix
            Dictionary<Module, List<Module>> simultaneouslyModules = new();
            availableModules.ForEach(module =>
            {
                simultaneouslyModules[module] = new List<Module>(availableModules);
                courses.ForEach(course =>
                {
                    if (course.CompulsoryModules.Contains(module))
                    {
                        course.CompulsoryModules.ForEach(compulsoryModule => 
                        {
                            simultaneouslyModules[module].Remove(compulsoryModule);
                        });
                    }
                });
            });

            // Create Professor x Module Matrix
            Dictionary<User, List<Module>> professorModules = new();
            availableModules.ForEach(module =>
            {
                module.AvailableProfessors.ForEach(professor => 
                {
                    if (!professorModules.ContainsKey(professor))
                    {
                        professorModules[professor] = new List<Module>();
                    }
                    if (!professorModules[professor].Contains(module))
                    {
                        professorModules[professor].Add(module);
                    }
                });
            });
            var sortedProfessorModules = 
                from entry in professorModules 
                orderby entry.Value.Count()
                ascending select entry;

            // Create Professor (Module) x Module Matrix (removed)
            Dictionary <User, Dictionary<Module, List<Module>>> fusedModules = new();
            foreach(KeyValuePair<User, List<Module>> professorModule in sortedProfessorModules) 
            {
                fusedModules[professorModule.Key] = new();
                professorModule.Value.ForEach(module =>
                {
                    fusedModules[professorModule.Key][module] = simultaneouslyModules[module];
                });
            }

            // Calculate which Professor does each Module
            Dictionary<User, int> professorAssignedModules = new();
            foreach (KeyValuePair<User, List<Module>> professorModule in sortedProfessorModules)
            {
                professorAssignedModules[professorModule.Key] = new();
            }

            Dictionary<Module, User> moduleProfessors = new();
            availableModules.ForEach(module =>
            {
                foreach(KeyValuePair<User, List<Module>> professorModule in sortedProfessorModules)
                {
                    if (professorModule.Value.Contains(module))
                    {
                        moduleProfessors.Add(module, professorModule.Key);
                        professorAssignedModules[professorModule.Key] += 1;
                        break;
                    };
                }
                sortedProfessorModules =
                    from entry in sortedProfessorModules
                    orderby entry.Value.Count()
                    ascending
                    select entry;
                sortedProfessorModules = sortedProfessorModules.OrderBy(p => professorAssignedModules[p.Key]);
            });

            return moduleProfessors;
        }
    }
}
