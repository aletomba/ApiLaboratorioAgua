using Infrastructure.Dtos;
using Dominio.IRepository;
using Dominio.Entities;
using Dominio.Exceptions;

namespace Aplicacion.Services
{    
    public class FisicoQuimicoService
    {
        private readonly ILibroFisicoQuimicoRepository _repo;

        public FisicoQuimicoService(ILibroFisicoQuimicoRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<FisicoQuimicoDto>> GetAllAsync()
        {
            var entities = await _repo.GetAllAsync();
            return entities.Select(MapToDto).ToList();
        }

        public async Task<PagedResultDto<FisicoQuimicoDto>> GetAllPagedAsync(int page, int pageSize)
        {
            var (items, totalCount) = await _repo.GetAllPagedAsync(page, pageSize);
            return new PagedResultDto<FisicoQuimicoDto>
            {
                Items = items.Select(MapToDto).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<PagedResultDto<FisicoQuimicoDto>> GetByFechaRangoPagedAsync(DateTime desde, DateTime hasta, int page, int pageSize)
        {
            var (items, totalCount) = await _repo.GetByFechaRangoPagedAsync(desde, hasta, page, pageSize);
            return new PagedResultDto<FisicoQuimicoDto>
            {
                Items = items.Select(MapToDto).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<PagedResultDto<FisicoQuimicoDto>> GetByClienteIdPagedAsync(int clienteId, int page, int pageSize)
        {
            var (items, totalCount) = await _repo.GetByClienteIdPagedAsync(clienteId, page, pageSize);
            return new PagedResultDto<FisicoQuimicoDto>
            {
                Items = items.Select(MapToDto).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        private static FisicoQuimicoDto MapToDto(FisicoQuimico fq) => new FisicoQuimicoDto
        {
            Id = fq.Id,
            Fecha = fq.Fecha,
            FechaLLegada = fq.FechaLLegada,
            FechaAnalisis = fq.FechaAnalisis,
            Procedencia = fq.Procedencia,
            Ph = fq.Ph,
            Turbidez = fq.Turbidez,
            Alcalinidad = fq.Alcalinidad,
            Dureza = fq.Dureza,
            Nitritos = fq.Nitritos,
            Cloruros = fq.Cloruros,
            Calcio = fq.Calcio,
            Magnesio = fq.Magnesio,
            Dbo5 = fq.Dbo5,
            Cloro = fq.Cloro,
            MuestraId = fq.MuestraId
        };

        public async Task<FisicoQuimicoDto?> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;
            return MapToDto(entity);
        }

        public async Task<FisicoQuimicoDto> CreateAsync(FisicoQuimicoDto dto)
        {
            var entity = new FisicoQuimico
            {
                Fecha = dto.Fecha == default ? DateTime.Now : dto.Fecha,
                FechaLLegada = dto.FechaLLegada == default ? DateTime.Now : dto.FechaLLegada,
                FechaAnalisis = dto.FechaAnalisis == default ? DateTime.Now : dto.FechaAnalisis,
                Procedencia = dto.Procedencia,
                Ph = dto.Ph,
                Turbidez = dto.Turbidez,
                Alcalinidad = dto.Alcalinidad,
                Dureza = dto.Dureza,
                Nitritos = dto.Nitritos,
                Cloruros = dto.Cloruros,
                Calcio = dto.Calcio,
                Magnesio = dto.Magnesio,
                Dbo5 = dto.Dbo5,
                Cloro = dto.Cloro,
                MuestraId = dto.MuestraId
            };

            var created = await _repo.AddAsync(entity);
            return MapToDto(created);
        }

        public async Task UpdateAsync(FisicoQuimicoEditDto dto)
        {
            var entity = await _repo.GetByIdAsync(dto.Id);
            if (entity == null)
                throw new NotFoundException($"FisicoQuimico con ID {dto.Id} no encontrado.");

            // Actualizar campos editables
            entity.Ph = dto.Ph;
            entity.Turbidez = dto.Turbidez;
            entity.Alcalinidad = dto.Alcalinidad;
            entity.Dureza = dto.Dureza;
            entity.Nitritos = dto.Nitritos;
            entity.Cloruros = dto.Cloruros;
            entity.Calcio = dto.Calcio;
            entity.Magnesio = dto.Magnesio;
            entity.Dbo5 = dto.Dbo5;
            entity.Cloro = dto.Cloro;

            await _repo.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                throw new NotFoundException($"FisicoQuimico con ID {id} no encontrado.");
            await _repo.DeleteAsync(id);
        }
    }
}
