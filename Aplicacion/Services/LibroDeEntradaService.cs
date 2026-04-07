using Aplicacion.Factories;
using Aplicacion.Mappers;
using Infrastructure.Dtos;
using Dominio.Exceptions;
using Dominio.Entities;
using Dominio.IRepository;

namespace Aplicacion.Services
{
    public class LibroDeEntradaService
    {
        private readonly ILibroEntradaRepository _libroEntradaRepository;
        private readonly IMuestraRepository _muestraRepository;
        private readonly IClienteRepository _clienteRepository;

        public LibroDeEntradaService(
            ILibroEntradaRepository libroEntradaRepository,
            IMuestraRepository muestraRepository,
            IClienteRepository clienteRepository)
        {
            _libroEntradaRepository = libroEntradaRepository;
            _muestraRepository = muestraRepository;
            _clienteRepository = clienteRepository;
        }

        public async Task<PagedResultDto<LibroDeEntradaResponseDto>> GetAllLibroEntradasAsync(int page = 1, int pageSize = 50)
        {
            var (libros, totalCount) = await _libroEntradaRepository.GetAllPagedAsync(page, pageSize);
            
            var items = libros.Select(le => le.ToDto()).ToList();

            return new PagedResultDto<LibroDeEntradaResponseDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task RegistrarLibroEntradaAsync(LibroDeEntradaDto libroEntradaDto)
        {
            var muestras = new List<Muestra>();
            foreach (var muestraDto in libroEntradaDto.Muestras)
            {
                var cliente = await _clienteRepository.GetByIdAsync(muestraDto.ClienteId);
                if (cliente == null)
                    throw new NotFoundException($"Cliente con ID {muestraDto.ClienteId} no encontrado.");

                muestras.Add(LibroDeEntradaFactory.CreateMuestra(muestraDto, libroEntradaDto, muestraDto.SitioExtraccion));
            }

            var libroEntrada = LibroDeEntradaFactory.CreateLibroEntrada(libroEntradaDto, muestras);
            await _libroEntradaRepository.AddAsync(libroEntrada);
        }

        public async Task<LibroDeEntradaResponseDto> GetLibroEntradaByIdAsync(int id)
        {
            var libroEntrada = await _libroEntradaRepository.GetByIdAsync(id);
            if (libroEntrada == null)
            {
                throw new NotFoundException($"Libro de entrada con ID {id} no encontrado.");
            }

            return libroEntrada.ToDto();
        }

        public async Task<List<LibroDeEntradaResponseDto>> GetLibroEntradasByMuestraIdAsync(int muestraId)
        {
            var muestra = await _muestraRepository.GetByIdAsync(muestraId);
            if (muestra == null)
            {
                throw new NotFoundException($"Muestra con ID {muestraId} no encontrada.");
            }

            var libroEntradas = await _libroEntradaRepository.GetByMuestraIdAsync(muestraId);
            return libroEntradas.Select(le => le.ToDto()).ToList();
        }

        public async Task<List<LibroDeEntradaResponseDto>> GetLibroEntradasByProcedenciaAsync(string procedencia)
        {
            var libros = await _libroEntradaRepository.GetByProcedenciaAsync(procedencia);
            return libros.Select(le => le.ToDto()).ToList();
        }

        public async Task UpdateLibroEntradaAsync(LibroDeEntradaDto libroEntradaDto)
        {
            var libro = await _libroEntradaRepository.GetByIdAsync(libroEntradaDto.Id);
            if (libro == null)
                throw new NotFoundException($"Libro de entrada con ID {libroEntradaDto.Id} no encontrado.");

            // Actualizar datos principales
            libro.Fecha = libroEntradaDto.Fecha;
            libro.FechaLLegada = libroEntradaDto.FechaLLegada;
            libro.FechaAnalisis = libroEntradaDto.FechaAnalisis;
            libro.Procedencia = libroEntradaDto.Procedencia;
            libro.SitioExtraccion = libroEntradaDto.SitioExtraccion ?? string.Empty;
            libro.Observaciones = libroEntradaDto.Observaciones;

            var muestrasActuales = libro.Muestras ?? new List<Muestra>();
            var muestrasDto = libroEntradaDto.Muestras ?? new List<MuestraDto>();

            // Actualizar y agregar muestras
            foreach (var muestraDto in muestrasDto)
            {
                // Solo buscar existente si el dto tiene un ID real (> 0).
                // Si Id es 0, es muestra nueva y siempre va al branch de creación,
                // evitando que varias muestras nuevas se sobreescriban entre sí.
                var muestraExistente = muestraDto.Id > 0
                    ? muestrasActuales.FirstOrDefault(m => m.Id == muestraDto.Id)
                    : null;

                var cliente = await _clienteRepository.GetByIdAsync(muestraDto.ClienteId);
                if (cliente == null)
                    throw new NotFoundException($"Cliente con ID {muestraDto.ClienteId} no encontrado.");

                var tipoMuestra = LibroDeEntradaFactory.ParseTipoMuestra(muestraDto.TipoMuestra);

                if (muestraExistente != null)
                {
                    // Actualizar muestra existente
                    muestraExistente.Procedencia = muestraDto.SitioExtraccion;
                    muestraExistente.NombreMuestreador = muestraDto.NombreMuestreador;
                    muestraExistente.Latitud = muestraDto.Latitud;
                    muestraExistente.Longitud = muestraDto.Longitud;
                    muestraExistente.FechaExtraccion = muestraDto.FechaExtraccion;
                    muestraExistente.HoraExtraccion = muestraDto.HoraExtraccion;
                    muestraExistente.TipoMuestra = tipoMuestra;
                    muestraExistente.ClienteId = muestraDto.ClienteId;

                    // Crear entidad de análisis si no existe todavía
                    if (tipoMuestra == TipoMuestra.Bacteriologica && muestraExistente.Bacteriologia == null)
                        muestraExistente.Bacteriologia = LibroDeEntradaFactory.CreateBacteriologico(libroEntradaDto, muestraDto, muestraExistente);
                    else if (tipoMuestra == TipoMuestra.FisicoQuimica && muestraExistente.FisicoQuimico == null)
                        muestraExistente.FisicoQuimico = LibroDeEntradaFactory.CreateFisicoQuimico(libroEntradaDto, muestraDto, muestraExistente);
                }
                else
                {
                    // Agregar nueva muestra
                    muestrasActuales.Add(LibroDeEntradaFactory.CreateMuestra(muestraDto, libroEntradaDto, libroEntradaDto.Procedencia));
                }
            }

            // Eliminar muestras que ya no están en el DTO (por Id)
            var muestrasAEliminar = muestrasActuales
                .Where(m => !muestrasDto.Any(dto => dto.Id == m.Id))
                .ToList();

            foreach (var muestra in muestrasAEliminar)
            {
            // Elimina de la base de datos
            await _muestraRepository.DeleteAsync (muestra.Id);
                muestrasActuales.Remove(muestra);
            }

            libro.Muestras = muestrasActuales;

            await _libroEntradaRepository.UpdateAsync(libro);
        }
        public async Task DeleteLibroEntradaAsync(int id)
        {
            var libro = await _libroEntradaRepository.GetByIdAsync(id);
            if (libro == null)
                throw new NotFoundException($"Libro de entrada con ID {id} no encontrado.");

            await _libroEntradaRepository.DeleteAsync(id);
        }

        public async Task<PagedResultDto<LibroDeEntradaResponseDto>> GetLibroEntradasByProcedenciaPagedAsync(
            string procedencia, int page = 1, int pageSize = 50)
        {
            var (libros, totalCount) = await _libroEntradaRepository.GetByProcedenciaPagedAsync(procedencia, page, pageSize);

            var items = libros.Select(le => le.ToDto()).ToList();

            return new PagedResultDto<LibroDeEntradaResponseDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<PagedResultDto<LibroDeEntradaResponseDto>> GetLibroEntradasByFechaRangoAsync(
            DateTime desde, DateTime hasta, int page = 1, int pageSize = 50)
        {
            var (libros, totalCount) = await _libroEntradaRepository.GetByFechaRangoPagedAsync(desde, hasta, page, pageSize);

            var items = libros.Select(le => le.ToDto()).ToList();

            return new PagedResultDto<LibroDeEntradaResponseDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }
    }
}
