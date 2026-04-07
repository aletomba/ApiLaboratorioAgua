using Aplicacion.Mappers;
using Infrastructure.Dtos;
using Dominio.Exceptions;
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

        public async Task<PlanillaDiariaResponseDto> GetByIdAsync(int id)
        {
            var planilla = await _planillaRepo.GetByIdAsync(id)
                ?? throw new NotFoundException($"Planilla con ID {id} no encontrada.");
            return planilla.ToDto();
        }

        public async Task<PlanillaDiariaResponseDto> GetByFechaAsync(DateTime fecha)
        {
            var planilla = await _planillaRepo.GetByFechaAsync(fecha)
                ?? throw new NotFoundException($"No existe planilla para la fecha {fecha:yyyy-MM-dd}.");
            return planilla.ToDto();
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
                ?? throw new NotFoundException($"Cliente con ID {clienteId} no encontrado.");

            // Upsert: si ya existe una planilla para esa fecha, actualizar en lugar de insertar
            var existente = await _planillaRepo.GetByFechaAsync(dto.Fecha.Date);
            if (existente != null)
            {
                await UpdateAsync(existente.Id, dto);
                return (await _planillaRepo.GetByIdAsync(existente.Id)
                    ?? throw new Exception("Error al recuperar la planilla actualizada.")).ToDto();
            }

            // Crear las muestras con sus análisis fisicoquímicos
            var muestras = new List<Muestra>();
            foreach (var analisis in dto.AnalisisPorPunto)
            {
                var puntoEntity = MapPuntoMuestreo(analisis.PuntoMuestreo);
                var muestra = new Muestra
                {
                    Procedencia = analisis.PuntoMuestreo.ToString(),
                    NombreMuestreador = dto.Operador ?? string.Empty,
                    Latitud = 0,
                    Longitud = 0,
                    FechaExtraccion = dto.Fecha,
                    HoraExtraccion = TimeSpan.Zero,
                    TipoMuestra = TipoMuestra.FisicoQuimica,
                    PuntoMuestreo = puntoEntity,
                    ClienteId = clienteId,
                    FisicoQuimico = new FisicoQuimico
                    {
                        Fecha = dto.Fecha,
                        FechaLLegada = dto.Fecha,
                        FechaAnalisis = dto.Fecha,
                        Procedencia = analisis.PuntoMuestreo.ToString(),
                        SitioExtraccion = analisis.PuntoMuestreo.ToString(),
                        Ph = analisis.Ph,
                        Turbidez = analisis.Turbidez,
                        Alcalinidad = analisis.Alcalinidad,
                        Dureza = analisis.Dureza,
                        Nitritos = analisis.Nitritos,
                        Cloruros = analisis.Cloruros,
                        Calcio = analisis.Calcio,
                        Magnesio = analisis.Magnesio,
                        Dbo5 = analisis.Dbo5,
                        Cloro = analisis.Cloro,
                    }
                };
                muestras.Add(muestra);
            }

            // Crear LibroDeEntrada con todas las muestras
            var libro = new LibroDeEntrada
            {
                Fecha = dto.Fecha,
                FechaLLegada = dto.Fecha,
                FechaAnalisis = dto.Fecha,
                Procedencia = "Planta Potabilizadora",
                SitioExtraccion = "Planta",
                Observaciones = dto.Observaciones ?? string.Empty,
                Muestras = muestras
            };
            await _libroEntradaRepo.AddAsync(libro);

            // Armar EnsayoJarras si viene en el DTO
            EnsayoJarras? ensayo = null;
            if (dto.EnsayoJarras != null)
            {
                ensayo = new EnsayoJarras
                {
                    Dosis1 = dto.EnsayoJarras.Dosis1,
                    Dosis2 = dto.EnsayoJarras.Dosis2,
                    Dosis3 = dto.EnsayoJarras.Dosis3,
                    Dosis4 = dto.EnsayoJarras.Dosis4,
                    Dosis5 = dto.EnsayoJarras.Dosis5,
                    DosisSeleccionada = dto.EnsayoJarras.DosisSeleccionada,
                    PreCal = dto.EnsayoJarras.PreCal,
                    PostCal = dto.EnsayoJarras.PostCal,
                    UnidadMedida = "mg/L"
                };
            }

            // Crear la planilla
            var planilla = new PlanillaDiaria
            {
                Fecha = dto.Fecha.Date,
                Operador = dto.Operador,
                Observaciones = dto.Observaciones,
                LibroEntradaId = libro.Id,
                EnsayoJarras = ensayo
            };

            var creada = await _planillaRepo.AddAsync(planilla);
            return (await _planillaRepo.GetByIdAsync(creada.Id)
                ?? throw new Exception("Error al recuperar la planilla creada.")).ToDto();
        }

        public async Task UpdateAsync(int id, PlanillaDiariaDto dto)
        {
            var planilla = await _planillaRepo.GetByIdAsync(id)
                ?? throw new NotFoundException($"Planilla con ID {id} no encontrada.");

            planilla.Operador = dto.Operador;
            planilla.Observaciones = dto.Observaciones;

            // Actualizar ensayo de jarras
            if (dto.EnsayoJarras != null && planilla.EnsayoJarras != null)
            {
                planilla.EnsayoJarras.Dosis1 = dto.EnsayoJarras.Dosis1;
                planilla.EnsayoJarras.Dosis2 = dto.EnsayoJarras.Dosis2;
                planilla.EnsayoJarras.Dosis3 = dto.EnsayoJarras.Dosis3;
                planilla.EnsayoJarras.Dosis4 = dto.EnsayoJarras.Dosis4;
                planilla.EnsayoJarras.Dosis5 = dto.EnsayoJarras.Dosis5;
                planilla.EnsayoJarras.DosisSeleccionada = dto.EnsayoJarras.DosisSeleccionada;
                planilla.EnsayoJarras.PreCal = dto.EnsayoJarras.PreCal;
                planilla.EnsayoJarras.PostCal = dto.EnsayoJarras.PostCal;
            }
            else if (dto.EnsayoJarras != null && planilla.EnsayoJarras == null)
            {
                planilla.EnsayoJarras = new EnsayoJarras
                {
                    Dosis1 = dto.EnsayoJarras.Dosis1,
                    Dosis2 = dto.EnsayoJarras.Dosis2,
                    Dosis3 = dto.EnsayoJarras.Dosis3,
                    Dosis4 = dto.EnsayoJarras.Dosis4,
                    Dosis5 = dto.EnsayoJarras.Dosis5,
                    DosisSeleccionada = dto.EnsayoJarras.DosisSeleccionada,
                    PreCal = dto.EnsayoJarras.PreCal,
                    PostCal = dto.EnsayoJarras.PostCal,
                    UnidadMedida = "mg/L"
                };
            }

            // Actualizar análisis físicoquímico por punto
            if (planilla.LibroEntrada?.Muestras != null)
            {
                foreach (var analisis in dto.AnalisisPorPunto)
                {
                    var puntoEntity = MapPuntoMuestreo(analisis.PuntoMuestreo);
                    var muestra = planilla.LibroEntrada.Muestras
                        .FirstOrDefault(m => m.PuntoMuestreo == puntoEntity);

                    if (muestra?.FisicoQuimico != null)
                    {
                        muestra.FisicoQuimico.Ph = analisis.Ph;
                        muestra.FisicoQuimico.Turbidez = analisis.Turbidez;
                        muestra.FisicoQuimico.Alcalinidad = analisis.Alcalinidad;
                        muestra.FisicoQuimico.Dureza = analisis.Dureza;
                        muestra.FisicoQuimico.Nitritos = analisis.Nitritos;
                        muestra.FisicoQuimico.Cloruros = analisis.Cloruros;
                        muestra.FisicoQuimico.Calcio = analisis.Calcio;
                        muestra.FisicoQuimico.Magnesio = analisis.Magnesio;
                        muestra.FisicoQuimico.Dbo5 = analisis.Dbo5;
                        muestra.FisicoQuimico.Cloro = analisis.Cloro;
                    }
                }
            }

            await _planillaRepo.UpdateAsync(planilla);
        }

        public async Task DeleteAsync(int id)
        {
            var planilla = await _planillaRepo.GetByIdAsync(id)
                ?? throw new NotFoundException($"Planilla con ID {id} no encontrada.");
            await _planillaRepo.DeleteAsync(id);
        }

        // ─── Helpers ────────────────────────────────────────────────────────────

        private static PuntoMuestreo MapPuntoMuestreo(PuntoMuestreoDto dto) => dto switch
        {
            PuntoMuestreoDto.AguaNatural => PuntoMuestreo.AguaNatural,
            PuntoMuestreoDto.Decantada   => PuntoMuestreo.Decantada,
            PuntoMuestreoDto.Filtrada    => PuntoMuestreo.Filtrada,
            PuntoMuestreoDto.Consumo     => PuntoMuestreo.Consumo,
            _ => throw new ArgumentException("PuntoMuestreo no válido.")
        };
    }
}
