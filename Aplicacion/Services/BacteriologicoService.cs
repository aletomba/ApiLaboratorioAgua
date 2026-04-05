using Infrastructure.Dtos;
using Dominio.IRepository;
using Dominio.Entities;
using Dominio.Exceptions;

namespace Aplicacion.Services
{
    public class BacteriologicoService
    {
        private readonly ILibroBacteriologiaRepository _repo;

        public BacteriologicoService(ILibroBacteriologiaRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<BacteriologicoDto>> GetAllAsync()
        {
            var entities = await _repo.GetAllAsync();
            return entities.Select(MapToDto).ToList();
        }

        public async Task<PagedResultDto<BacteriologicoDto>> GetAllPagedAsync(int page, int pageSize)
        {
            var (items, totalCount) = await _repo.GetAllPagedAsync(page, pageSize);
            return new PagedResultDto<BacteriologicoDto>
            {
                Items = items.Select(MapToDto).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<PagedResultDto<BacteriologicoDto>> GetByFechaRangoPagedAsync(DateTime desde, DateTime hasta, int page, int pageSize)
        {
            var (items, totalCount) = await _repo.GetByFechaRangoPagedAsync(desde, hasta, page, pageSize);
            return new PagedResultDto<BacteriologicoDto>
            {
                Items = items.Select(MapToDto).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<PagedResultDto<BacteriologicoDto>> GetByClienteIdPagedAsync(int clienteId, int page, int pageSize)
        {
            var (items, totalCount) = await _repo.GetByClienteIdPagedAsync(clienteId, page, pageSize);
            return new PagedResultDto<BacteriologicoDto>
            {
                Items = items.Select(MapToDto).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        private static BacteriologicoDto MapToDto(Bacteriologico b) => new BacteriologicoDto
        {
            Id = b.Id,
            Fecha = b.Fecha,
            FechaLLegada = b.FechaLLegada,
            FechaAnalisis = b.FechaAnalisis,
            Procedencia = b.Procedencia,
            ColiformesNmp = b.ColiformesNmp,
            ColiformesFecalesNmp = b.ColiformesFecalesNmp,
            ColoniasAgar = b.ColoniasAgar,
            ColiFecalesUfc = b.ColiFecalesUfc,
            Observaciones = b.Observaciones,
            MuestraId = b.MuestraId
        };

        public async Task<BacteriologicoDto?> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;
            return MapToDto(entity);
        }

        public async Task<BacteriologicoDto> CreateAsync(BacteriologicoDto dto)
        {
            var entity = new Bacteriologico
            {
                Fecha = dto.Fecha == default ? DateTime.Now : dto.Fecha,
                FechaLLegada = dto.FechaLLegada == default ? DateTime.Now : dto.FechaLLegada,
                FechaAnalisis = dto.FechaAnalisis == default ? DateTime.Now : dto.FechaAnalisis,
                Procedencia = dto.Procedencia,
                ColiformesNmp = dto.ColiformesNmp,
                ColiformesFecalesNmp = dto.ColiformesFecalesNmp,
                ColoniasAgar = dto.ColoniasAgar,
                ColiFecalesUfc = dto.ColiFecalesUfc,
                Observaciones = dto.Observaciones,
                MuestraId = dto.MuestraId
            };

            var created = await _repo.AddAsync(entity);
            return MapToDto(created);
        }

        public async Task UpdateAsync(BacteriologicoDto dto)
        {
            var entity = await _repo.GetByIdAsync(dto.Id);
            if (entity == null)
                throw new NotFoundException($"Bacteriologico con ID {dto.Id} no encontrado.");

            entity.ColiformesNmp = dto.ColiformesNmp;
            entity.ColiformesFecalesNmp = dto.ColiformesFecalesNmp;
            entity.ColoniasAgar = dto.ColoniasAgar;
            entity.ColiFecalesUfc = dto.ColiFecalesUfc;
            entity.Observaciones = dto.Observaciones;

            await _repo.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                throw new NotFoundException($"Bacteriologico con ID {id} no encontrado.");
            await _repo.DeleteAsync(id);
        }
    }
}
