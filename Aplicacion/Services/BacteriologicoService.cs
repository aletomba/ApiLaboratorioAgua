using Infrastructure.Dtos;
using Dominio.IRepository;
using Dominio.Entities;
using Infrastructure.MyExeptions;

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
            return entities.Select(b => new BacteriologicoDto
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
            }).ToList();
        }

        public async Task<BacteriologicoDto?> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;
            return new BacteriologicoDto
            {
                Id = entity.Id,
                Fecha = entity.Fecha,
                FechaLLegada = entity.FechaLLegada,
                FechaAnalisis = entity.FechaAnalisis,
                Procedencia = entity.Procedencia,
                ColiformesNmp = entity.ColiformesNmp,
                ColiformesFecalesNmp = entity.ColiformesFecalesNmp,
                ColoniasAgar = entity.ColoniasAgar,
                ColiFecalesUfc = entity.ColiFecalesUfc,
                Observaciones = entity.Observaciones,
                MuestraId = entity.MuestraId
            };
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

            return new BacteriologicoDto
            {
                Id = created.Id,
                Fecha = created.Fecha,
                FechaLLegada = created.FechaLLegada,
                FechaAnalisis = created.FechaAnalisis,
                Procedencia = created.Procedencia,
                ColiformesNmp = created.ColiformesNmp,
                ColiformesFecalesNmp = created.ColiformesFecalesNmp,
                ColoniasAgar = created.ColoniasAgar,
                ColiFecalesUfc = created.ColiFecalesUfc,
                Observaciones = created.Observaciones,
                MuestraId = created.MuestraId
            };
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
            await _repo.DeleteAsync(id);
        }
    }
}
