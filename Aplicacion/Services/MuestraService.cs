using Infrastructure.Dtos;
using Dominio.Exceptions;
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

        public async Task RegistrarMuestraAsync(MuestraDto muestraDto)
        {
            var cliente = await _clienteRepository.GetByIdAsync(muestraDto.ClienteId);
            if (cliente == null)
                throw new NotFoundException($"Cliente con ID {muestraDto.ClienteId} no encontrado.");

            // Mapear TipoMuestraDto a TipoMuestra
            TipoMuestra tipoMuestra = muestraDto.TipoMuestra switch
            {
                TipoDeMuestraDto.Bacteriologica => TipoMuestra.Bacteriologica,
                TipoDeMuestraDto.FisicoQuimica => TipoMuestra.FisicoQuimica,
                _ => throw new ArgumentException("Tipo de muestra no válido.")
            };

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
        }

        public async Task<List<MuestraResponseDto>> GetMuestrasPorClienteAsync(int clienteId)
        {
            var cliente = await _clienteRepository.GetByIdAsync(clienteId);
            if (cliente == null)
            {
                throw new NotFoundException($"Cliente con ID {clienteId} no encontrado.");
            }

            var muestras = await _muestraRepository.GetByClienteIdAsync(clienteId);
            return muestras.Select(m => new MuestraResponseDto
            {
                Id = m.Id,
                Procedencia = m.Procedencia,
                NombreMuestreador = m.NombreMuestreador,
                Latitud = m.Latitud,
                Longitud = m.Longitud,
                FechaExtraccion = m.FechaExtraccion,
                HoraExtraccion = m.HoraExtraccion,
                TipoMuestra = m.TipoMuestra switch
                {
                    TipoMuestra.Bacteriologica => TipoDeMuestraDto.Bacteriologica,
                    TipoMuestra.FisicoQuimica => TipoDeMuestraDto.FisicoQuimica,
                    _ => throw new ArgumentException("Tipo de muestra no válido.")
                },
                ClienteId = m.ClienteId,
                ClienteNombre = m.Cliente?.Nombre,
                LibroEntradaId = m.LibroEntradaId
            }).ToList();
        }
    }
}

