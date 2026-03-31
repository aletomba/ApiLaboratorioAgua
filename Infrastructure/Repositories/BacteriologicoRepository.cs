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

        public async Task<(List<Bacteriologico> Items, int TotalCount)> GetAllPagedAsync(int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 50;
            if (pageSize > 500) pageSize = 500;

            var query = _context.Bacteriologicos
                .AsNoTracking()
                .Include(b => b.Muestra);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(b => b.Fecha)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<(List<Bacteriologico> Items, int TotalCount)> GetByFechaRangoPagedAsync(DateTime desde, DateTime hasta, int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 50;
            if (pageSize > 500) pageSize = 500;

            var desdeDate = desde.Date;
            var hastaDate = hasta.Date.AddDays(1);

            var query = _context.Bacteriologicos
                .AsNoTracking()
                .Include(b => b.Muestra)
                .Where(b => b.Fecha >= desdeDate && b.Fecha < hastaDate);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(b => b.Fecha)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<(List<Bacteriologico> Items, int TotalCount)> GetByClienteIdPagedAsync(int clienteId, int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 50;
            if (pageSize > 500) pageSize = 500;

            var query = _context.Bacteriologicos
                .AsNoTracking()
                .Include(b => b.Muestra)
                .Where(b => b.Muestra != null && b.Muestra.ClienteId == clienteId);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(b => b.Fecha)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
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
