using Dominio.Entities;
using Dominio.IRepository;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class MuestraRepository : IMuestraRepository
    {
        private readonly LabAguaDbContext _context;

        public MuestraRepository(LabAguaDbContext context)
        {
            _context = context;
        }
        public async Task<Muestra> AddAsync(Muestra muestra)
        {
            _context.Muestras.Add(muestra);
            await _context.SaveChangesAsync();
            return muestra;
        }

        public async Task DeleteAsync(int id)
        {
            var muestra = await _context.Muestras
                .Include(m => m.Bacteriologia)
                .Include(m => m.FisicoQuimico)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (muestra == null)
                return;

            // Si no tienes cascade delete, elimina manualmente las entidades hijas:
            // if (muestra.Bacteriologia != null)
            //     _context.Bacteriologicos.Remove(muestra.Bacteriologia);
            // if (muestra.FisicoQuimico != null)
            //     _context.FisicoQuimicos.Remove(muestra.FisicoQuimico);

            _context.Muestras.Remove(muestra);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Muestra>> GetByClienteIdAsync(int clienteId)
        {
            return await _context.Muestras
              .Where(m => m.ClienteId == clienteId)
              .Include(m => m.Cliente)
              .Include(m => m.LibroEntrada)
              .Include(m => m.Bacteriologia)
              .Include(m => m.FisicoQuimico)
              .ToListAsync();
        }

        public async Task<Muestra> GetByIdAsync(int id)
        {
            return await _context.Muestras
            .Include(m => m.Cliente)
            .Include(m => m.LibroEntrada)
            .Include(m => m.Bacteriologia)
            .Include(m => m.FisicoQuimico)
            .FirstOrDefaultAsync(m => m.Id == id);
        }
    }
}
