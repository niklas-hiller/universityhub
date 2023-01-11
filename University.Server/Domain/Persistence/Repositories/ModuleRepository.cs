using Microsoft.EntityFrameworkCore;
using University.Server.Domain.Models;
using University.Server.Domain.Persistence.Contexts;


namespace University.Server.Domain.Persistence.Repositories
{
    public class ModuleRepository : BaseRepository, IModuleRepository
    {
        private readonly ILogger<ModuleRepository> _logger;

        public ModuleRepository(AppDbContext context, ILogger<ModuleRepository> logger) : base(context)
        {
            _logger = logger;
        }

        public async Task AddAsync(Module module)
        {
            await _context.Modules.AddAsync(module);
        }
    }
}
