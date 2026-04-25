using Aplicacion.Factories;
using Aplicacion.Mappers;
using Infrastructure.Dtos;
using Dominio;
using Dominio.IRepository;
using Dominio.Entities;

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
            return entities.Select(b => b.ToDto()).ToList();
        }

        public async Task<PagedResultDto<BacteriologicoDto>> GetAllPagedAsync(int page, int pageSize)
        {
            var (items, totalCount) = await _repo.GetAllPagedAsync(page, pageSize);
            return new PagedResultDto<BacteriologicoDto>
            {
                Items = items.Select(b => b.ToDto()).ToList(),
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
                Items = items.Select(b => b.ToDto()).ToList(),
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
                Items = items.Select(b => b.ToDto()).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<Result<BacteriologicoDto>> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                return Result<BacteriologicoDto>.Failure($"Bacteriologico con ID {id} no encontrado.");
            return Result<BacteriologicoDto>.Success(entity.ToDto());
        }

        public async Task<BacteriologicoDto> CreateAsync(BacteriologicoDto dto)
        {
            var entity = BacteriologicoFactory.Create(dto);
            var created = await _repo.AddAsync(entity);
            return created.ToDto();
        }

        public async Task<Result> UpdateAsync(BacteriologicoDto dto)
        {
            var entity = await _repo.GetByIdAsync(dto.Id);
            if (entity == null)
                return Result.Failure($"Bacteriologico con ID {dto.Id} no encontrado.");

            BacteriologicoFactory.Update(entity, dto);
            await _repo.UpdateAsync(entity);
            return Result.Success();
        }

        public async Task<Result> DeleteAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                return Result.Failure($"Bacteriologico con ID {id} no encontrado.");
            await _repo.DeleteAsync(id);
            return Result.Success();
        }
    }
}
