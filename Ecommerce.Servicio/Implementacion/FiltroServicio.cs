using Microsoft.EntityFrameworkCore;
using Ecommerce.Modelo;
using Ecommerce.DTO;
using Ecommerce.Repositorio.Contrato;
using Ecommerce.Servicio.Contrato;
using AutoMapper;
using Ecommerce.Repositorio.DBContext;

namespace Ecommerce.Servicio.Implementacion
{
    public class FiltroServicio : IFiltroServicio
    {
        private readonly IGenericoRepositorio<Filtro> _modeloRepositorio;
        private readonly DbecommerceContext _context;
        private readonly IMapper _mapper;

        public FiltroServicio(
            IGenericoRepositorio<Filtro> modeloRepositorio,
            DbecommerceContext context,
            IMapper mapper)
        {
            _modeloRepositorio = modeloRepositorio;
            _context = context;
            _mapper = mapper;
        }

        public async Task<FiltroDTO> Crear(FiltroDTO modelo)
        {
            try
            {
                var dbModelo = _mapper.Map<Filtro>(modelo);

                // Guardar Filtro
                var rspModelo = await _modeloRepositorio.Crear(dbModelo);

                // Insertar relación con categorías
                if (modelo.CategoriaIds != null && modelo.CategoriaIds.Any())
                {
                    var relaciones = modelo.CategoriaIds.Select(catId => new CategoriaFiltro
                    {
                        IdFiltro = rspModelo.IdFiltro,
                        IdCategoria = catId
                    }).ToList();

                    _context.AddRange(relaciones);
                    await _context.SaveChangesAsync();
                }

                modelo.IdFiltro = rspModelo.IdFiltro;
                return modelo;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> Editar(FiltroDTO modelo)
        {
            try
            {
                var consulta = _modeloRepositorio.Consultar(p => p.IdFiltro == modelo.IdFiltro);
                var fromDbModelo = await consulta.FirstOrDefaultAsync();

                if (fromDbModelo != null)
                {
                    fromDbModelo.Nombre = modelo.Nombre;
                    fromDbModelo.TipoFiltro = modelo.TipoFiltro;

                    var respuesta = await _modeloRepositorio.Editar(fromDbModelo);

                    if (!respuesta)
                        throw new TaskCanceledException("No se pudo editar");

                    // Eliminar relaciones anteriores
                    var relacionesAnteriores = _context.Set<CategoriaFiltro>()
                        .Where(cf => cf.IdFiltro == modelo.IdFiltro);
                    _context.RemoveRange(relacionesAnteriores);

                    // Agregar nuevas relaciones
                    if (modelo.CategoriaIds != null && modelo.CategoriaIds.Any())
                    {
                        var nuevasRelaciones = modelo.CategoriaIds.Select(catId => new CategoriaFiltro
                        {
                            IdFiltro = modelo.IdFiltro,
                            IdCategoria = catId
                        }).ToList();

                        await _context.AddRangeAsync(nuevasRelaciones);
                    }

                    await _context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    throw new TaskCanceledException("No se encontraron resultados");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> Eliminar(int id)
        {
            try
            {
                var consulta = _modeloRepositorio.Consultar(p => p.IdFiltro == id);
                var fromDbModelo = await consulta.FirstOrDefaultAsync();

                if (fromDbModelo != null)
                {
                    // Eliminar relaciones
                    var relaciones = _context.Set<CategoriaFiltro>().Where(cf => cf.IdFiltro == id);
                    _context.RemoveRange(relaciones);
                    await _context.SaveChangesAsync();

                    // Eliminar filtro
                    var respuesta = await _modeloRepositorio.Eliminar(fromDbModelo);
                    if (!respuesta)
                        throw new TaskCanceledException("No se pudo eliminar");
                    return respuesta;
                }
                else
                {
                    throw new TaskCanceledException("No se encontraron resultados");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<FiltroDTO>> Lista(string buscar)
        {
            try
            {
                var consulta = _modeloRepositorio.Consultar(p =>
                    p.Nombre!.ToLower().Contains(buscar.ToLower()));

                var filtros = await consulta.ToListAsync();

                var listaDto = new List<FiltroDTO>();

                foreach (var filtro in filtros)
                {
                    var categoriaIds = await _context.Set<CategoriaFiltro>()
                        .Where(cf => cf.IdFiltro == filtro.IdFiltro)
                        .Select(cf => cf.IdCategoria)
                        .ToListAsync();

                    var dto = _mapper.Map<FiltroDTO>(filtro);
                    dto.CategoriaIds = categoriaIds;
                    listaDto.Add(dto);
                }

                return listaDto;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<FiltroDTO> Obtener(int id)
        {
            try
            {
                var consulta = _modeloRepositorio.Consultar(p => p.IdFiltro == id);
                var fromDbModelo = await consulta.FirstOrDefaultAsync();

                if (fromDbModelo != null)
                {
                    var dto = _mapper.Map<FiltroDTO>(fromDbModelo);

                    dto.CategoriaIds = await _context.Set<CategoriaFiltro>()
                        .Where(cf => cf.IdFiltro == fromDbModelo.IdFiltro)
                        .Select(cf => cf.IdCategoria)
                        .ToListAsync();

                    return dto;
                }
                else
                {
                    throw new TaskCanceledException("No se encontraron coincidencias");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
