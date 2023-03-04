using Microsoft.EntityFrameworkCore;
using University.Server.Domain.Models;
using University.Server.Domain.Persistence.Contexts;
using University.Server.Domain.Repositories;

namespace University.Server.Domain.Persistence.Repositories
{
    public class CourseRepository : BaseRepository, ICourseRepository
    {
        private readonly ILogger<CourseRepository> _logger;

        public CourseRepository(AppDbContext context, ILogger<CourseRepository> logger) : base(context)
        {
            _logger = logger;
        }

        public async Task AddAsync(Course course)
        {
            await _context.Courses.AddAsync(course);
        }

        public async Task<Course?> GetAsync(Guid id)
        {
            return await _context.Courses.FindAsync(id);
        }

        public async Task<IEnumerable<Course>> ListAsync()
        {
            return await _context.Courses.ToListAsync();
        }

        public void Update(Course course)
        {
            _context.Courses.Update(course);
        }

        public void Remove(Course course)
        {
            _context.Courses.Remove(course);
        }
    }
}
