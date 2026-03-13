using Dominio.Entities;
using Dominio.IRepository;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class BacteriologicoRepository : ILibroBacteriologiaRepository
    {
        private readonly LabAguaDbContext _context;

        public BacteriologicoRepository(LabAguaDbContext context)
        {
            _context = context;
        }

        public async Task<Bacteriologico> AddAsync(Bacteriologico libroBacteriologia)
        {
            _context.Bacteriologicos.Add(libroBacteriologia);
            await _context.SaveChangesAsync();
            return libroBacteriologia;
        }
        public async Task<List<Bacteriologico>> GetAllAsync()
        {
            return await _context.Bacteriologicos
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Bacteriologico?> GetByIdAsync(int id)
        {
            return await _context.Bacteriologicos
                .AsNoTracking()
                .Include(b => b.Muestra)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task UpdateAsync(Bacteriologico bacteriologico)
        {
            _context.Bacteriologicos.Update(bacteriologico);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Bacteriologicos.FindAsync(id);
            if (entity == null) return;
            _context.Bacteriologicos.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
