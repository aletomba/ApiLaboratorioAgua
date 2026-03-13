using Dominio.Entities;
using Dominio.IRepository;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class FisicoQuimicoRepository:ILibroFisicoQuimicoRepository
    {
        private readonly LabAguaDbContext _context;       

        public FisicoQuimicoRepository(LabAguaDbContext context)
        {
            _context = context;
        }

        public async Task<FisicoQuimico> AddAsync(FisicoQuimico libroFisicoQuimico)
        {
            _context.FisicoQuimicos.Add(libroFisicoQuimico);
            await _context.SaveChangesAsync();
            return libroFisicoQuimico;
        }

        public async Task<List<FisicoQuimico>> GetAllAsync()
        {
            return await _context.FisicoQuimicos
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<FisicoQuimico?> GetByIdAsync(int id)
        {
            return await _context.FisicoQuimicos
                .AsNoTracking()
                .Include(f => f.Muestra)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task UpdateAsync(FisicoQuimico fisicoQuimico)
        {
            _context.FisicoQuimicos.Update(fisicoQuimico);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.FisicoQuimicos.FindAsync(id);
            if (entity != null)
            {
                _context.FisicoQuimicos.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
