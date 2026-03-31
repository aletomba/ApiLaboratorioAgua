using Dominio.Entities;

namespace Dominio.IRepository
{
    public interface IClienteRepository
    {
        Task<Cliente> AddAsync(Cliente cliente);
        Task<Cliente> GetByIdAsync(int id);       
        Task<List<Cliente>> GetAllAsync();
        Task UpdateAsync(Cliente cliente);
        Task DeleteAsync(int id);
    }

    public interface IMuestraRepository
    {
        Task<Muestra> AddAsync(Muestra muestra);
        Task<Muestra> GetByIdAsync(int id);
        Task<List<Muestra>> GetByClienteIdAsync(int clienteId); // Nuevo método
        Task DeleteAsync(int id);
    }


    public interface ILibroEntradaRepository
    {
        Task<LibroDeEntrada> AddAsync(LibroDeEntrada libroEntrada);
        Task DeleteAsync(int id);
        Task<List<LibroDeEntrada>> GetAllAsync();
        
        // ✨ NUEVOS - Paginados
        Task<(List<LibroDeEntrada> Items, int TotalCount)> GetAllPagedAsync(int page, int pageSize);
        Task<(List<LibroDeEntrada> Items, int TotalCount)> GetByProcedenciaPagedAsync(string procedencia, int page, int pageSize);
        Task<(List<LibroDeEntrada> Items, int TotalCount)> GetByFechaRangoPagedAsync(DateTime desde, DateTime hasta, int page, int pageSize);

        Task<LibroDeEntrada> GetByIdAsync(int id);
        Task<List<LibroDeEntrada>> GetByProcedenciaAsync(string procedencia);
        Task<List<LibroDeEntrada>> GetByMuestraIdAsync(int muestraId);
        Task UpdateAsync(LibroDeEntrada libroEntrada);
    }

    public interface ILibroBacteriologiaRepository
    {
        Task<Bacteriologico> AddAsync(Bacteriologico Bacteriologia);
        Task<List<Bacteriologico>> GetAllAsync();
        Task<(List<Bacteriologico> Items, int TotalCount)> GetAllPagedAsync(int page, int pageSize);
        Task<(List<Bacteriologico> Items, int TotalCount)> GetByFechaRangoPagedAsync(DateTime desde, DateTime hasta, int page, int pageSize);
        Task<(List<Bacteriologico> Items, int TotalCount)> GetByClienteIdPagedAsync(int clienteId, int page, int pageSize);
        Task<Bacteriologico?> GetByIdAsync(int id);
        Task UpdateAsync(Bacteriologico bacteriologico);
        Task DeleteAsync(int id);
    }

    public interface ILibroFisicoQuimicoRepository
    {
        Task<FisicoQuimico> AddAsync(FisicoQuimico FisicoQuimico);
        Task<List<FisicoQuimico>> GetAllAsync();
        Task<(List<FisicoQuimico> Items, int TotalCount)> GetAllPagedAsync(int page, int pageSize);
        Task<(List<FisicoQuimico> Items, int TotalCount)> GetByFechaRangoPagedAsync(DateTime desde, DateTime hasta, int page, int pageSize);
        Task<(List<FisicoQuimico> Items, int TotalCount)> GetByClienteIdPagedAsync(int clienteId, int page, int pageSize);
        Task<FisicoQuimico?> GetByIdAsync(int id);
        Task UpdateAsync(FisicoQuimico fisicoQuimico);
        Task DeleteAsync(int id);
    }

    public interface IPlanillaDiariaRepository
    {
        Task<PlanillaDiaria> AddAsync(PlanillaDiaria planilla);
        Task<PlanillaDiaria?> GetByIdAsync(int id);
        Task<PlanillaDiaria?> GetByFechaAsync(DateTime fecha);
        Task<(List<PlanillaDiaria> Items, int TotalCount)> GetAllPagedAsync(int page, int pageSize);
        Task<(List<PlanillaDiaria> Items, int TotalCount)> GetByFechaRangoPagedAsync(DateTime desde, DateTime hasta, int page, int pageSize);
        Task UpdateAsync(PlanillaDiaria planilla);
        Task DeleteAsync(int id);
    }
}
