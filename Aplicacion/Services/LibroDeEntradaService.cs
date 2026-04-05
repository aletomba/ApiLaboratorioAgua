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
            
            var items = libros.Select(le => new LibroDeEntradaResponseDto
            {
                Id = le.Id,
                FechaRegistro = le.Fecha,
                FechaLlegada = le.FechaLLegada,
                FechaAnalisis = le.FechaAnalisis,
                Procedencia = le.Procedencia,
                SitioExtraccion = le.SitioExtraccion,
                Observaciones = le.Observaciones,
                Muestras = le.Muestras?.Select(m => new MuestraResponseDto
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
                }).ToList() ?? new List<MuestraResponseDto>()
            }).ToList();

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

                TipoMuestra tipoMuestra = muestraDto.TipoMuestra switch
                {
                    TipoDeMuestraDto.Bacteriologica => TipoMuestra.Bacteriologica,
                    TipoDeMuestraDto.FisicoQuimica => TipoMuestra.FisicoQuimica,
                    _ => throw new ArgumentException("Tipo de muestra no válido.")
                };

                var muestra = new Muestra
                {
                    Procedencia = muestraDto.SitioExtraccion,                   
                    NombreMuestreador = muestraDto.NombreMuestreador,
                    Latitud = muestraDto.Latitud,
                    Longitud = muestraDto.Longitud,
                    FechaExtraccion = muestraDto.FechaExtraccion,
                    HoraExtraccion = muestraDto.HoraExtraccion,
                    TipoMuestra = tipoMuestra,
                    ClienteId = muestraDto.ClienteId
                };


                // Asociar libro de análisis según tipo
                if (tipoMuestra == TipoMuestra.Bacteriologica)
                {
                    muestra.Bacteriologia = new Bacteriologico
                    {
                        Fecha = libroEntradaDto.Fecha,
                        FechaAnalisis = libroEntradaDto.FechaAnalisis,
                        FechaLLegada = libroEntradaDto.FechaLLegada,
                        Procedencia = libroEntradaDto.Procedencia,
                        SitioExtraccion = muestraDto.SitioExtraccion,
                        ColiformesNmp = string.Empty,
                        ColiformesFecalesNmp = string.Empty,
                        ColoniasAgar = string.Empty,
                        ColiFecalesUfc = string.Empty,
                        Observaciones = string.Empty,
                        Muestra = muestra
                    };
                }
                else if (tipoMuestra == TipoMuestra.FisicoQuimica)
                {
                    muestra.FisicoQuimico = new FisicoQuimico
                    {
                        Fecha = libroEntradaDto.Fecha,
                        FechaAnalisis = libroEntradaDto.FechaAnalisis,
                        FechaLLegada = libroEntradaDto.FechaLLegada,
                        Procedencia = libroEntradaDto.Procedencia,
                        SitioExtraccion = muestraDto.SitioExtraccion,
                        Ph = string.Empty,
                        Turbidez = string.Empty,
                        Alcalinidad = string.Empty,
                        Dureza = string.Empty,
                        Nitritos = string.Empty,
                        Cloruros = string.Empty,
                        Calcio = string.Empty,
                        Magnesio = string.Empty,
                        Dbo5 = string.Empty,
                        Muestra = muestra
                    };
                }

                muestras.Add(muestra);
            }

            var libroEntrada = new LibroDeEntrada
            {
                Fecha = libroEntradaDto.Fecha,
                FechaLLegada = libroEntradaDto.FechaLLegada,
                FechaAnalisis = libroEntradaDto.FechaAnalisis,
                Procedencia = libroEntradaDto.Procedencia,
                SitioExtraccion = libroEntradaDto.SitioExtraccion ?? string.Empty,
                Observaciones = libroEntradaDto.Observaciones,
                Muestras = muestras
            };

            foreach (var muestra in muestras)
            {
                muestra.LibroEntrada = libroEntrada;
            }

            await _libroEntradaRepository.AddAsync(libroEntrada);
        }

        public async Task<LibroDeEntradaResponseDto> GetLibroEntradaByIdAsync(int id)
        {
            var libroEntrada = await _libroEntradaRepository.GetByIdAsync(id);
            if (libroEntrada == null)
            {
                throw new NotFoundException($"Libro de entrada con ID {id} no encontrado.");
            }

            return new LibroDeEntradaResponseDto
            {
                Id = libroEntrada.Id,
                FechaRegistro = libroEntrada.Fecha,
                FechaLlegada = libroEntrada.FechaLLegada,
                FechaAnalisis = libroEntrada.FechaAnalisis,
                Procedencia = libroEntrada.Procedencia,               
                Observaciones = libroEntrada.Observaciones,
                Muestras = libroEntrada.Muestras?.Select(m => new MuestraResponseDto
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
                }).ToList() ?? new List<MuestraResponseDto>()
            };
        }

        public async Task<List<LibroDeEntradaResponseDto>> GetLibroEntradasByMuestraIdAsync(int muestraId)
        {
            var muestra = await _muestraRepository.GetByIdAsync(muestraId);
            if (muestra == null)
            {
                throw new NotFoundException($"Muestra con ID {muestraId} no encontrada.");
            }

            var libroEntradas = await _libroEntradaRepository.GetByMuestraIdAsync(muestraId);
            return libroEntradas.Select(le => new LibroDeEntradaResponseDto
            {
                Id = le.Id,
                FechaRegistro = le.Fecha,
                FechaLlegada = le.FechaLLegada,
                FechaAnalisis = le.FechaAnalisis,
                Procedencia = le.Procedencia,           
                Observaciones = le.Observaciones,
                Muestras = le.Muestras?.Select(m => new MuestraResponseDto
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
                }).ToList() ?? new List<MuestraResponseDto>()
            }).ToList();
        }

        public async Task<List<LibroDeEntradaResponseDto>> GetLibroEntradasByProcedenciaAsync(string procedencia)
        {
            var libros = await _libroEntradaRepository.GetByProcedenciaAsync(procedencia);
            return libros.Select(le => new LibroDeEntradaResponseDto
            {
                Id = le.Id,
                FechaRegistro = le.Fecha,
                FechaLlegada = le.FechaLLegada,
                FechaAnalisis = le.FechaAnalisis,
                Procedencia = le.Procedencia,
                SitioExtraccion = le.SitioExtraccion,
                Observaciones = le.Observaciones,
                Muestras = le.Muestras?.Select(m => new MuestraResponseDto
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
                }).ToList() ?? new List<MuestraResponseDto>()
            }).ToList();
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

                TipoMuestra tipoMuestra = muestraDto.TipoMuestra switch
                {
                    TipoDeMuestraDto.Bacteriologica => TipoMuestra.Bacteriologica,
                    TipoDeMuestraDto.FisicoQuimica => TipoMuestra.FisicoQuimica,
                    _ => throw new ArgumentException("Tipo de muestra no válido.")
                };

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

                    // Actualizar o crear entidad de análisis
                    if (tipoMuestra == TipoMuestra.Bacteriologica)
                    {
                        if (muestraExistente.Bacteriologia == null)
                        {
                            muestraExistente.Bacteriologia = new Bacteriologico
                            {
                                Fecha = libroEntradaDto.Fecha,
                                FechaAnalisis = libroEntradaDto.FechaAnalisis,
                                FechaLLegada = libroEntradaDto.FechaLLegada,
                                Procedencia = libroEntradaDto.Procedencia,
                                SitioExtraccion = muestraDto.SitioExtraccion ?? string.Empty,
                                ColiformesNmp = string.Empty,
                                ColiformesFecalesNmp = string.Empty,
                                ColoniasAgar = string.Empty,
                                ColiFecalesUfc = string.Empty,
                                Observaciones = string.Empty,
                                Muestra = muestraExistente
                            };
                        }
                    }
                    else if (tipoMuestra == TipoMuestra.FisicoQuimica)
                    {
                        if (muestraExistente.FisicoQuimico == null)
                        {
                            muestraExistente.FisicoQuimico = new FisicoQuimico
                            {
                                Fecha = libroEntradaDto.Fecha,
                                FechaAnalisis = libroEntradaDto.FechaAnalisis,
                                FechaLLegada = libroEntradaDto.FechaLLegada,
                                Procedencia = libroEntradaDto.Procedencia,
                                SitioExtraccion = muestraDto.SitioExtraccion ?? string.Empty,
                                Ph = string.Empty,
                                Turbidez = string.Empty,
                                Alcalinidad = string.Empty,
                                Dureza = string.Empty,
                                Nitritos = string.Empty,
                                Cloruros = string.Empty,
                                Calcio = string.Empty,
                                Magnesio = string.Empty,
                                Dbo5 = string.Empty,
                                Muestra = muestraExistente
                            };
                        }
                    }
                }
                else
                {
                    // Agregar nueva muestra
                    var nuevaMuestra = new Muestra
                    {
                        Procedencia = libroEntradaDto.Procedencia,
                        NombreMuestreador = muestraDto.NombreMuestreador,
                        Latitud = muestraDto.Latitud,
                        Longitud = muestraDto.Longitud,
                        FechaExtraccion = muestraDto.FechaExtraccion,
                        HoraExtraccion = muestraDto.HoraExtraccion,
                        TipoMuestra = tipoMuestra,
                        ClienteId = muestraDto.ClienteId
                    };

                    if (tipoMuestra == TipoMuestra.Bacteriologica)
                    {
                        nuevaMuestra.Bacteriologia = new Bacteriologico
                        {
                            Fecha = libroEntradaDto.Fecha,
                            FechaAnalisis = libroEntradaDto.FechaAnalisis,
                            FechaLLegada = libroEntradaDto.FechaLLegada,
                            Procedencia = libroEntradaDto.Procedencia,
                            SitioExtraccion = muestraDto.SitioExtraccion ?? string.Empty,
                            ColiformesNmp = string.Empty,
                            ColiformesFecalesNmp = string.Empty,
                            ColoniasAgar = string.Empty,
                            ColiFecalesUfc = string.Empty,
                            Observaciones = string.Empty,
                            Muestra = nuevaMuestra
                        };
                    }
                    else if (tipoMuestra == TipoMuestra.FisicoQuimica)
                    {
                        nuevaMuestra.FisicoQuimico = new FisicoQuimico
                        {
                            Fecha = libroEntradaDto.Fecha,
                            FechaAnalisis = libroEntradaDto.FechaAnalisis,
                            FechaLLegada = libroEntradaDto.FechaLLegada,
                            Procedencia = libroEntradaDto.Procedencia,
                            SitioExtraccion = muestraDto.SitioExtraccion ?? string.Empty,
                            Ph = string.Empty,
                            Turbidez = string.Empty,
                            Alcalinidad = string.Empty,
                            Dureza = string.Empty,
                            Nitritos = string.Empty,
                            Cloruros = string.Empty,
                            Calcio = string.Empty,
                            Magnesio = string.Empty,
                            Dbo5 = string.Empty,
                            Muestra = nuevaMuestra
                        };
                    }

                    muestrasActuales.Add(nuevaMuestra);
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
    
    var items = libros.Select(le => new LibroDeEntradaResponseDto
    {
        Id = le.Id,
        FechaRegistro = le.Fecha,
        FechaLlegada = le.FechaLLegada,
        FechaAnalisis = le.FechaAnalisis,
        Procedencia = le.Procedencia,
        SitioExtraccion = le.SitioExtraccion,
        Observaciones = le.Observaciones,
        Muestras = le.Muestras?.Select(m => new MuestraResponseDto
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
        }).ToList() ?? new List<MuestraResponseDto>()
    }).ToList();

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

            var items = libros.Select(le => new LibroDeEntradaResponseDto
            {
                Id = le.Id,
                FechaRegistro = le.Fecha,
                FechaLlegada = le.FechaLLegada,
                FechaAnalisis = le.FechaAnalisis,
                Procedencia = le.Procedencia,
                SitioExtraccion = le.SitioExtraccion,
                Observaciones = le.Observaciones,
                Muestras = le.Muestras?.Select(m => new MuestraResponseDto
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
                }).ToList() ?? new List<MuestraResponseDto>()
            }).ToList();

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
