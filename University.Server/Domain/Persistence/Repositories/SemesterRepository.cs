using Microsoft.EntityFrameworkCore;
using University.Server.Domain.Models;
using University.Server.Domain.Persistence.Contexts;
using University.Server.Domain.Repositories;

namespace University.Server.Domain.Persistence.Repositories
{
    public class SemesterRepository : BaseRepository, ISemesterRepository
    {
        private readonly ILogger<SemesterRepository> _logger;

        public SemesterRepository(AppDbContext context, ILogger<SemesterRepository> logger) : base(context)
        {
            _logger = logger;
        }

        public async Task AddAsync(Semester semester)
        {
            await _context.Semesters.AddAsync(semester);
        }

        public async Task<Semester?> GetAsync(Guid id)
        {
            return await _context.Semesters.FindAsync(id);
        }

        public async Task<IEnumerable<Semester>> ListAsync()
        {
            return await _context.Semesters.ToListAsync();
        }

        public void Remove(Semester semester)
        {
            _context.Semesters.Remove(semester);
        }
    }
}
