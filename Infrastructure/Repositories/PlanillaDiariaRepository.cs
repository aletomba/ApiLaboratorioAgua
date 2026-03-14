using Dominio.Entities;
using Dominio.IRepository;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PlanillaDiariaRepository : IPlanillaDiariaRepository
    {
        private readonly LabAguaDbContext _context;

        public PlanillaDiariaRepository(LabAguaDbContext context)
        {
            _context = context;
        }

        public async Task<PlanillaDiaria> AddAsync(PlanillaDiaria planilla)
        {
            _context.PlanillasDiarias.Add(planilla);
            await _context.SaveChangesAsync();
            return planilla;
        }

        public async Task<PlanillaDiaria?> GetByIdAsync(int id)
        {
            return await _context.PlanillasDiarias
                .Include(p => p.EnsayoJarras)
                .Include(p => p.LibroEntrada)
                    .ThenInclude(le => le!.Muestras)
                        .ThenInclude(m => m.FisicoQuimico)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<PlanillaDiaria?> GetByFechaAsync(DateTime fecha)
        {
            var fechaSolo = fecha.Date;
            return await _context.PlanillasDiarias
                .Include(p => p.EnsayoJarras)
                .Include(p => p.LibroEntrada)
                    .ThenInclude(le => le!.Muestras)
                        .ThenInclude(m => m.FisicoQuimico)
                .FirstOrDefaultAsync(p => p.Fecha.Date == fechaSolo);
        }

        public async Task<(List<PlanillaDiaria> Items, int TotalCount)> GetAllPagedAsync(int page, int pageSize)
        {
            var query = _context.PlanillasDiarias
                .Include(p => p.EnsayoJarras)
                .OrderByDescending(p => p.Fecha);

            var total = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public async Task<(List<PlanillaDiaria> Items, int TotalCount)> GetByFechaRangoPagedAsync(DateTime desde, DateTime hasta, int page, int pageSize)
        {
            var desdeDate = desde.Date;
            var hastaDate = hasta.Date;

            var query = _context.PlanillasDiarias
                .Include(p => p.EnsayoJarras)
                .Where(p => p.Fecha.Date >= desdeDate && p.Fecha.Date <= hastaDate)
                .OrderByDescending(p => p.Fecha);

            var total = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public async Task UpdateAsync(PlanillaDiaria planilla)
        {
            _context.PlanillasDiarias.Update(planilla);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var planilla = await _context.PlanillasDiarias
                .Include(p => p.EnsayoJarras)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (planilla != null)
            {
                if (planilla.EnsayoJarras != null)
                    _context.EnsayosJarras.Remove(planilla.EnsayoJarras);

                _context.PlanillasDiarias.Remove(planilla);
                await _context.SaveChangesAsync();
            }
        }
    }
}
