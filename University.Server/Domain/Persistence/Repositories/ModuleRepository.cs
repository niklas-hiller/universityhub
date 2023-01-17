using Microsoft.EntityFrameworkCore;
using University.Server.Domain.Models;
using University.Server.Domain.Persistence.Contexts;
using University.Server.Domain.Repositories;

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

        public async Task<Module?> GetAsync(Guid id)
        {
            return await _context.Modules.FindAsync(id);
        }

        public async Task<IEnumerable<Module>> ListAsync()
        {
            return await _context.Modules.ToListAsync();
        }

        public void Update(Module module)
        {
            _context.Modules.Update(module);
        }
    }
}
