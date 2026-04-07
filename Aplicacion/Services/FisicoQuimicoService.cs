using Aplicacion.Factories;
using Aplicacion.Mappers;
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
            return entities.Select(fq => fq.ToDto()).ToList();
        }

        public async Task<PagedResultDto<FisicoQuimicoDto>> GetAllPagedAsync(int page, int pageSize)
        {
            var (items, totalCount) = await _repo.GetAllPagedAsync(page, pageSize);
            return new PagedResultDto<FisicoQuimicoDto>
            {
                Items = items.Select(fq => fq.ToDto()).ToList(),
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
                Items = items.Select(fq => fq.ToDto()).ToList(),
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
                Items = items.Select(fq => fq.ToDto()).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<FisicoQuimicoDto?> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;
            return entity.ToDto();
        }

        public async Task<FisicoQuimicoDto> CreateAsync(FisicoQuimicoDto dto)
        {
            var entity = FisicoQuimicoFactory.Create(dto);
            var created = await _repo.AddAsync(entity);
            return created.ToDto();
        }

        public async Task UpdateAsync(FisicoQuimicoEditDto dto)
        {
            var entity = await _repo.GetByIdAsync(dto.Id);
            if (entity == null)
                throw new NotFoundException($"FisicoQuimico con ID {dto.Id} no encontrado.");

            FisicoQuimicoFactory.Update(entity, dto);

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
