using Dominio.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    /// <summary>
    /// Extensiones de IQueryable para reutilizar cadenas de Include comunes.
    /// </summary>
    internal static class QueryExtensions
    {
        /// <summary>
        /// Incluye las Muestras completas (con Cliente, Bacteriologia y FisicoQuimico).
        /// </summary>
        internal static IQueryable<LibroDeEntrada> WithFullMuestras(this IQueryable<LibroDeEntrada> query)
            => query
                .Include(le => le.Muestras)
                    .ThenInclude(m => m.Cliente)
                .Include(le => le.Muestras)
                    .ThenInclude(m => m.Bacteriologia)
                .Include(le => le.Muestras)
                    .ThenInclude(m => m.FisicoQuimico);
    }
}
