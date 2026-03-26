using Infrastructure.Dtos;
using Dominio.IRepository;
using Dominio.Entities;

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
            return entities.Select(fq => new FisicoQuimicoDto
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
            }).ToList();
        }

        public async Task<FisicoQuimicoDto?> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;
            return new FisicoQuimicoDto
            {
                Id = entity.Id,
                Fecha = entity.Fecha,
                FechaLLegada = entity.FechaLLegada,
                FechaAnalisis = entity.FechaAnalisis,
                Procedencia = entity.Procedencia,
                Ph = entity.Ph,
                Turbidez = entity.Turbidez,
                Alcalinidad = entity.Alcalinidad,
                Dureza = entity.Dureza,
                Nitritos = entity.Nitritos,
                Cloruros = entity.Cloruros,
                Calcio = entity.Calcio,
                Magnesio = entity.Magnesio,
                Dbo5 = entity.Dbo5,
                Cloro = entity.Cloro,
                MuestraId = entity.MuestraId
            };
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

            return new FisicoQuimicoDto
            {
                Id = created.Id,
                Fecha = created.Fecha,
                FechaLLegada = created.FechaLLegada,
                FechaAnalisis = created.FechaAnalisis,
                Procedencia = created.Procedencia,
                Ph = created.Ph,
                Turbidez = created.Turbidez,
                Alcalinidad = created.Alcalinidad,
                Dureza = created.Dureza,
                Nitritos = created.Nitritos,
                Cloruros = created.Cloruros,
                Calcio = created.Calcio,
                Magnesio = created.Magnesio,
                Dbo5 = created.Dbo5,
                Cloro = created.Cloro,
                MuestraId = created.MuestraId
            };
        }

        public async Task UpdateAsync(FisicoQuimicoEditDto dto)
        {
            var entity = await _repo.GetByIdAsync(dto.Id);
            if (entity == null)
                throw new Infrastructure.MyExeptions.NotFoundException($"FisicoQuimico con ID {dto.Id} no encontrado.");

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
                throw new Infrastructure.MyExeptions.NotFoundException($"FisicoQuimico con ID {id} no encontrado.");
            await _repo.DeleteAsync(id);
        }
    }
}
