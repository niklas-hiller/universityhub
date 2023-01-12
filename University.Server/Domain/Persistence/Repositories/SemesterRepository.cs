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
    }
}
