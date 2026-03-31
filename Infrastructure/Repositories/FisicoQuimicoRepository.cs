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

        public async Task<(List<FisicoQuimico> Items, int TotalCount)> GetAllPagedAsync(int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 50;
            if (pageSize > 500) pageSize = 500;

            var query = _context.FisicoQuimicos
                .AsNoTracking()
                .Include(f => f.Muestra);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(f => f.Fecha)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<(List<FisicoQuimico> Items, int TotalCount)> GetByFechaRangoPagedAsync(DateTime desde, DateTime hasta, int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 50;
            if (pageSize > 500) pageSize = 500;

            var desdeDate = desde.Date;
            var hastaDate = hasta.Date.AddDays(1);

            var query = _context.FisicoQuimicos
                .AsNoTracking()
                .Include(f => f.Muestra)
                .Where(f => f.Fecha >= desdeDate && f.Fecha < hastaDate);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(f => f.Fecha)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<(List<FisicoQuimico> Items, int TotalCount)> GetByClienteIdPagedAsync(int clienteId, int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 50;
            if (pageSize > 500) pageSize = 500;

            var query = _context.FisicoQuimicos
                .AsNoTracking()
                .Include(f => f.Muestra)
                .Where(f => f.Muestra != null && f.Muestra.ClienteId == clienteId);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(f => f.Fecha)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
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
