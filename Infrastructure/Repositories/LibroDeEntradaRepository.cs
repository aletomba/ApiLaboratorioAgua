using Dominio.Constants;
using Dominio.Entities;
using Dominio.IRepository;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class LibroDeEntradaRepository: ILibroEntradaRepository, ILibroEntradaQueryRepository
    {
        private readonly LabAguaDbContext _context;

        public LibroDeEntradaRepository(LabAguaDbContext context)
        {
            _context = context;
        }

        public async Task<LibroDeEntrada> AddAsync(LibroDeEntrada libroEntrada)
        {
            _context.LibroEntradas.Add(libroEntrada);
            await _context.SaveChangesAsync();
            return libroEntrada;
        }

        public async Task DeleteAsync(int id)
        {
            var libro = await _context.LibroEntradas
                .Include(le => le.Muestras)
                    .ThenInclude(m => m.Bacteriologia)
                .Include(le => le.Muestras)
                    .ThenInclude(m => m.FisicoQuimico)
                .FirstOrDefaultAsync(le => le.Id == id);

            if (libro != null)
            {
                // Eliminar PlanillasDiarias asociadas (FK Restrict → bloquea la eliminación del libro)
                var planillas = await _context.PlanillasDiarias
                    .Where(p => p.LibroEntradaId == id)
                    .ToListAsync();
                _context.PlanillasDiarias.RemoveRange(planillas);

                _context.Muestras.RemoveRange(libro.Muestras);
                _context.LibroEntradas.Remove(libro);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<LibroDeEntrada>> GetAllAsync()
        {
            return await _context.LibroEntradas
                .AsNoTracking()
                .WithFullMuestras()
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene libros de entrada con paginaci�n
        /// </summary>
public async Task<(List<LibroDeEntrada> Items, int TotalCount)> GetAllPagedAsync(int page, int pageSize)
        {
            (page, pageSize) = PaginationDefaults.Normalize(page, pageSize);

            var query = _context.LibroEntradas
                .AsNoTracking()
                .WithFullMuestras();

            // Contar total de registros (antes de paginar)
            var totalCount = await query.CountAsync();

            // Aplicar paginaci�n
            var items = await query
                .OrderByDescending(le => le.Fecha)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        /// <summary>
        /// Obtiene libros de entrada filtrados por procedencia con paginaci�n
        /// </summary>
        public async Task<(List<LibroDeEntrada> Items, int TotalCount)> GetByProcedenciaPagedAsync(string procedencia, int page, int pageSize)
        {
            (page, pageSize) = PaginationDefaults.Normalize(page, pageSize);

            var query = _context.LibroEntradas
                .AsNoTracking()
                .Where(le => le.Procedencia != null && le.Procedencia.Contains(procedencia))
                .WithFullMuestras();

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(le => le.Fecha)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<(List<LibroDeEntrada> Items, int TotalCount)> GetByFechaRangoPagedAsync(DateTime desde, DateTime hasta, int page, int pageSize)
        {
            (page, pageSize) = PaginationDefaults.Normalize(page, pageSize);

            var desdeDate = desde.Date;
            var hastaDate = hasta.Date;

            var query = _context.LibroEntradas
                .AsNoTracking()
                .Where(le => le.Fecha.Date >= desdeDate && le.Fecha.Date <= hastaDate)
                .WithFullMuestras();

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(le => le.Fecha)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<LibroDeEntrada?> GetByIdAsync(int id)
        {
            return await _context.LibroEntradas
                .AsNoTracking()
                .WithFullMuestras()
                .FirstOrDefaultAsync(le => le.Id == id);
        }

        public async Task<List<LibroDeEntrada>> GetByProcedenciaAsync(string procedencia)
        {
            return await _context.LibroEntradas
                .AsNoTracking()
                .WithFullMuestras()
                .Where(le => le.Procedencia != null && le.Procedencia.Contains(procedencia))
                .ToListAsync();
        }

        public async Task<List<LibroDeEntrada>> GetByMuestraIdAsync(int muestraId)
        {
            return await _context.LibroEntradas
                .AsNoTracking()
                .WithFullMuestras()
                .Where(le => le.Muestras.Any(m => m.Id == muestraId))
                .ToListAsync();
        }

        public async Task UpdateAsync(LibroDeEntrada libroEntrada)
        {
            var existing = await _context.LibroEntradas
                .Include(l => l.Muestras)
                .ThenInclude(m => m.Bacteriologia)
                .Include(l => l.Muestras)
                .ThenInclude(m => m.FisicoQuimico)
                .FirstOrDefaultAsync(l => l.Id == libroEntrada.Id);

            if (existing != null)
            {
                _context.Entry(existing).CurrentValues.SetValues(libroEntrada);
            }
            else
            {
                _context.LibroEntradas.Update(libroEntrada);
            }
            await _context.SaveChangesAsync();
        }
    }
}
