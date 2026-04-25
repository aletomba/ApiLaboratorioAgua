using Aplicacion.Factories;
using Aplicacion.Mappers;
using Infrastructure.Dtos;
using Dominio;
using Dominio.Entities;
using Dominio.IRepository;

namespace Aplicacion.Services
{
    public class PlanillaDiariaService
    {
        private readonly IPlanillaDiariaRepository _planillaRepo;
        private readonly ILibroEntradaRepository _libroEntradaRepo;
        private readonly IClienteRepository _clienteRepo;

        public PlanillaDiariaService(
            IPlanillaDiariaRepository planillaRepo,
            ILibroEntradaRepository libroEntradaRepo,
            IClienteRepository clienteRepo)
        {
            _planillaRepo = planillaRepo;
            _libroEntradaRepo = libroEntradaRepo;
            _clienteRepo = clienteRepo;
        }

        public async Task<PagedResultDto<PlanillaDiariaResponseDto>> GetAllPagedAsync(int page = 1, int pageSize = 50)
        {
            var (items, total) = await _planillaRepo.GetAllPagedAsync(page, pageSize);
            return new PagedResultDto<PlanillaDiariaResponseDto>
            {
                Items = items.Select(p => p.ToDto()).ToList(),
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<Result<PlanillaDiariaResponseDto>> GetByIdAsync(int id)
        {
            var planilla = await _planillaRepo.GetByIdAsync(id);
            if (planilla == null)
                return Result<PlanillaDiariaResponseDto>.Failure($"Planilla con ID {id} no encontrada.");
            return Result<PlanillaDiariaResponseDto>.Success(planilla.ToDto());
        }

        public async Task<Result<PlanillaDiariaResponseDto>> GetByFechaAsync(DateTime fecha)
        {
            var planilla = await _planillaRepo.GetByFechaAsync(fecha);
            if (planilla == null)
                return Result<PlanillaDiariaResponseDto>.Failure($"No existe planilla para la fecha {fecha:yyyy-MM-dd}.");
            return Result<PlanillaDiariaResponseDto>.Success(planilla.ToDto());
        }

        public async Task<PagedResultDto<PlanillaDiariaResponseDto>> GetByFechaRangoAsync(
            DateTime desde, DateTime hasta, int page = 1, int pageSize = 50)
        {
            var (items, total) = await _planillaRepo.GetByFechaRangoPagedAsync(desde, hasta, page, pageSize);
            return new PagedResultDto<PlanillaDiariaResponseDto>
            {
                Items = items.Select(p => p.ToDto()).ToList(),
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// Crea la planilla diaria con su LibroDeEntrada, 
        /// las muestras por punto de muestreo y el ensayo de jarras.
        /// </summary>
        public async Task<PlanillaDiariaResponseDto> RegistrarAsync(PlanillaDiariaDto dto, int clienteId)
        {
            var cliente = await _clienteRepo.GetByIdAsync(clienteId)
                ?? throw new InvalidOperationException($"Cliente con ID {clienteId} no encontrado.");

            var existente = await _planillaRepo.GetByFechaAsync(dto.Fecha.Date);
            if (existente != null)
            {
                await UpdateAsync(existente.Id, dto);
                return (await _planillaRepo.GetByIdAsync(existente.Id)
                    ?? throw new InvalidOperationException("Error al recuperar la planilla actualizada.")).ToDto();
            }

            var muestras = dto.AnalisisPorPunto
                .Select(a => PlanillaDiariaFactory.CreateMuestra(a, dto, clienteId))
                .ToList();

            var libro = PlanillaDiariaFactory.CreateLibroEntrada(dto, muestras);
            await _libroEntradaRepo.AddAsync(libro);

            var ensayo = dto.EnsayoJarras != null
                ? PlanillaDiariaFactory.CreateEnsayoJarras(dto.EnsayoJarras)
                : null;

            var planilla = PlanillaDiariaFactory.CreatePlanilla(dto, libro.Id, ensayo);

            var creada = await _planillaRepo.AddAsync(planilla);
            return (await _planillaRepo.GetByIdAsync(creada.Id)
                ?? throw new InvalidOperationException("Error al recuperar la planilla creada.")).ToDto();
        }

        public async Task<Result> UpdateAsync(int id, PlanillaDiariaDto dto)
        {
            var planilla = await _planillaRepo.GetByIdAsync(id);
            if (planilla == null)
                return Result.Failure($"Planilla con ID {id} no encontrada.");

            planilla.Operador = dto.Operador;
            planilla.Observaciones = dto.Observaciones;

            if (dto.EnsayoJarras != null && planilla.EnsayoJarras != null)
            {
                PlanillaDiariaFactory.UpdateEnsayoJarras(planilla.EnsayoJarras, dto.EnsayoJarras);
            }
            else if (dto.EnsayoJarras != null && planilla.EnsayoJarras == null)
            {
                planilla.EnsayoJarras = PlanillaDiariaFactory.CreateEnsayoJarras(dto.EnsayoJarras);
            }

            if (planilla.LibroEntrada?.Muestras != null)
            {
                foreach (var analisis in dto.AnalisisPorPunto)
                {
                    var puntoEntity = PlanillaDiariaFactory.ParsePuntoMuestreo(analisis.PuntoMuestreo);
                    var muestra = planilla.LibroEntrada.Muestras
                        .FirstOrDefault(m => m.PuntoMuestreo == puntoEntity);

                    if (muestra?.FisicoQuimico != null)
                    {
                        PlanillaDiariaFactory.UpdateFisicoQuimico(muestra.FisicoQuimico, analisis);
                    }
                }
            }

            await _planillaRepo.UpdateAsync(planilla);
            return Result.Success();
        }

        public async Task<Result> DeleteAsync(int id)
        {
            var planilla = await _planillaRepo.GetByIdAsync(id);
            if (planilla == null)
                return Result.Failure($"Planilla con ID {id} no encontrada.");
            await _planillaRepo.DeleteAsync(id);
            return Result.Success();
        }

    }
}
