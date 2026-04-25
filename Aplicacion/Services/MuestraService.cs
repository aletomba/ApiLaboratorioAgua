using Aplicacion.Mappers;
using Infrastructure.Dtos;
using Dominio;
using Dominio.Entities;
using Dominio.IRepository;

namespace Aplicacion.Services
{
    public class MuestraService
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IMuestraRepository _muestraRepository;
        private readonly ILibroEntradaRepository _libroEntradaRepository;
        private readonly ILibroBacteriologiaRepository _libroBacteriologiaRepository;
        private readonly ILibroFisicoQuimicoRepository _libroFisicoQuimicoRepository;

        public MuestraService(
            IClienteRepository clienteRepository,
            IMuestraRepository muestraRepository,
            ILibroEntradaRepository libroEntradaRepository,
            ILibroBacteriologiaRepository libroBacteriologiaRepository,
            ILibroFisicoQuimicoRepository libroFisicoQuimicoRepository)
        {
            _clienteRepository = clienteRepository;
            _muestraRepository = muestraRepository;
            _libroEntradaRepository = libroEntradaRepository;
            _libroBacteriologiaRepository = libroBacteriologiaRepository;
            _libroFisicoQuimicoRepository = libroFisicoQuimicoRepository;
        }

        public async Task<Result<string>> RegistrarMuestraAsync(MuestraDto muestraDto)
        {
            var cliente = await _clienteRepository.GetByIdAsync(muestraDto.ClienteId);
            if (cliente == null)
                return Result<string>.Failure($"Cliente con ID {muestraDto.ClienteId} no encontrado.");

            var tipoMuestra = TipoMuestraMapper.ToDomain(muestraDto.TipoMuestra);

            var muestra = new Muestra
            {
                //Procedencia = muestraDto.Procedencia,
                NombreMuestreador = muestraDto.NombreMuestreador,
                Latitud = muestraDto.Latitud,
                Longitud = muestraDto.Longitud,
                FechaExtraccion = muestraDto.FechaExtraccion,
                HoraExtraccion = muestraDto.HoraExtraccion,
                TipoMuestra = tipoMuestra,
                ClienteId = muestraDto.ClienteId,
                Cliente = cliente
            };

            // Crear LibroEntrada y asociar la muestra
            var libroEntrada = new LibroDeEntrada
            {
                Fecha = muestraDto.FechaExtraccion,
                FechaLLegada = DateTime.Now,
                FechaAnalisis = DateTime.Now,
                Procedencia = muestra.Procedencia,
                Observaciones = "Muestra recibida",
                Muestras = new List<Muestra> { muestra }
            };
            muestra.LibroEntrada = libroEntrada;

            // Asociar libro de análisis según tipo
            if (muestra.TipoMuestra == TipoMuestra.Bacteriologica)
            {
                var libroBacteriologia = new Bacteriologico
                {
                    Fecha = muestraDto.FechaExtraccion,
                    FechaLLegada = DateTime.Now,
                    FechaAnalisis = DateTime.Now,
                    Procedencia = muestra.Procedencia,
                    ColiformesNmp = string.Empty,
                    ColiformesFecalesNmp = string.Empty,
                    Muestra = muestra
                };
                muestra.Bacteriologia = libroBacteriologia;
            }
            else if (muestra.TipoMuestra == TipoMuestra.FisicoQuimica)
            {
                var libroFisicoQuimico = new FisicoQuimico
                {
                    Fecha = muestraDto.FechaExtraccion,
                    FechaLLegada = DateTime.Now,
                    FechaAnalisis = DateTime.Now,
                    Procedencia = muestra.Procedencia,
                    Ph = string.Empty,
                    Cloruros = string.Empty,
                    Muestra = muestra
                };
                muestra.FisicoQuimico = libroFisicoQuimico;
            }

            // Solo agregas la entidad raíz
            await _libroEntradaRepository.AddAsync(libroEntrada);
            return Result<string>.Success("Muestra registrada con éxito.");
        }

        public async Task<Result<List<MuestraResponseDto>>> GetMuestrasPorClienteAsync(int clienteId)
        {
            var cliente = await _clienteRepository.GetByIdAsync(clienteId);
            if (cliente == null)
                return Result<List<MuestraResponseDto>>.Failure($"Cliente con ID {clienteId} no encontrado.");

            var muestras = await _muestraRepository.GetByClienteIdAsync(clienteId);
            return Result<List<MuestraResponseDto>>.Success(muestras.Select(m => m.ToDto()).ToList());
        }
    }
}

