using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Modelo;
using Ecommerce.DTO;
using Ecommerce.Repositorio.Contrato;
using Ecommerce.Servicio.Contrato;
using AutoMapper;

namespace Ecommerce.Servicio.Implementacion
{
    public class ProductoServicio : IProductoServicio
    {
        private readonly IGenericoRepositorio<Producto> _modeloProductoRepositorio;
        private readonly IGenericoRepositorio<DetalleVenta> _modeloDetalleVentaRepositorio;
        private readonly IGenericoRepositorio<ProductoImagen> _productoImagenRepositorio;
        private readonly IGenericoRepositorio<Categoria> _categoriaRepositorio;
        private readonly IGenericoRepositorio<ProductoFiltroValor> _productoFiltroValorRepositorio;
        private readonly IGenericoRepositorio<FiltroOpcion> _filtroOpcionRepositorio;

        private readonly IMapper _mapper;

        public ProductoServicio(
            IGenericoRepositorio<Producto> modeloRepositorio,
            IGenericoRepositorio<ProductoImagen> productoImagenRepositorio,
            IGenericoRepositorio<DetalleVenta> modeloDetalleVentaRepositorio,
            IGenericoRepositorio<Categoria> categoriaRepositorio,
            IGenericoRepositorio<ProductoFiltroValor> productoFiltroValorRepositorio,
            IGenericoRepositorio<FiltroOpcion> filtroOpcionRepositorio, 
            IMapper mapper)
        {
            _modeloProductoRepositorio = modeloRepositorio;
            _modeloDetalleVentaRepositorio = modeloDetalleVentaRepositorio;
            _productoImagenRepositorio = productoImagenRepositorio;
            _categoriaRepositorio = categoriaRepositorio;
            _productoFiltroValorRepositorio = productoFiltroValorRepositorio;
            _filtroOpcionRepositorio = filtroOpcionRepositorio; 
            _mapper = mapper;
        }


        public async Task<List<ProductoDTO>> Catalogo(string categoria, string buscar)
        {
            try
            {
                var consulta = _modeloProductoRepositorio.Consultar(p =>
                    p.Nombre.ToLower().Contains(buscar.ToLower()) &&
                    p.IdCategoriaNavigation.Nombre.ToLower().Contains(categoria.ToLower()) &&
                    p.Baja != true
                );

                consulta = consulta
                    .Include(i => i.ProductoImagenes)
                    .Include(p => p.ProductoFiltroValores);

                List<ProductoDTO> lista = _mapper.Map<List<ProductoDTO>>(await consulta.ToListAsync());
                return lista;
            }
            catch (Exception) { throw; }
        }

        public async Task<ProductoDTO> Crear(ProductoDTO modelo)
        {
            try
            {
                var dbModelo = _mapper.Map<Producto>(modelo);
                var rspModelo = await _modeloProductoRepositorio.Crear(dbModelo);

                if (rspModelo.IdProducto == 0)
                    throw new TaskCanceledException("No se pudo crear el producto en la base de datos.");

                if (modelo.Filtros != null && modelo.Filtros.Count > 0)
                {
                    foreach (var filtroDto in modelo.Filtros)
                    {
                        var filtroEntidad = new ProductoFiltroValor
                        {
                            IdProducto = rspModelo.IdProducto,
                            IdFiltro = filtroDto.IdFiltro,
                            Valor = filtroDto.Valor
                        };
                        await _productoFiltroValorRepositorio.Crear(filtroEntidad);
                    }
                }

                return _mapper.Map<ProductoDTO>(rspModelo);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear el producto y sus filtros", ex);
            }
        }

        public async Task<bool> Editar(ProductoDTO modelo)
        {
            try
            {
                var consulta = _modeloProductoRepositorio.Consultar(p => p.IdProducto == modelo.IdProducto)
                                                 .Include(p => p.ProductoImagenes);

                var fromDbModelo = await consulta.FirstOrDefaultAsync();

                if (fromDbModelo == null)
                    throw new TaskCanceledException("No se encontraron resultados");

                fromDbModelo.Nombre = modelo.Nombre;
                fromDbModelo.Descripcion = modelo.Descripcion;
                fromDbModelo.IdCategoria = modelo.IdCategoria;
                fromDbModelo.Precio = modelo.Precio;
                fromDbModelo.PrecioOferta = modelo.PrecioOferta;
                fromDbModelo.Cantidad = modelo.Cantidad;
                fromDbModelo.Baja = modelo.Baja;

                var imagenesActuales = fromDbModelo.ProductoImagenes?.ToList() ?? new List<ProductoImagen>();

                foreach (var (imgDto, imagenExistente) in
                    from imgDto in modelo.Imagenes
                    let imagenExistente = imagenesActuales.FirstOrDefault(i =>
                        i.IdProductoImagen == imgDto.IdImagen && i.NumeroImagen == imgDto.NumeroImagen)
                    select (imgDto, imagenExistente))
                {
                    if (imagenExistente != null)
                    {
                        imagenExistente.RutaImagen = imgDto.RutaImagen;
                    }
                    else
                    {
                        fromDbModelo.ProductoImagenes.Add(new ProductoImagen
                        {
                            IdProducto = modelo.IdProducto,
                            NumeroImagen = imgDto.NumeroImagen,
                            RutaImagen = imgDto.RutaImagen
                        });
                    }
                }

                var imagenesAEliminar = imagenesActuales
                    .Where(i => modelo.Imagenes.All(imgDto => imgDto.IdImagen != i.IdProductoImagen))
                    .ToList();

                foreach (var imagenAEliminar in imagenesAEliminar)
                {
                    fromDbModelo.ProductoImagenes.Remove(imagenAEliminar);
                }

                // Filtros: primero eliminar actuales, luego agregar nuevos
                var filtrosActuales = _productoFiltroValorRepositorio.Consultar(f => f.IdProducto == modelo.IdProducto);
                foreach (var filtro in await filtrosActuales.ToListAsync())
                {
                    await _productoFiltroValorRepositorio.Eliminar(filtro);
                }

                foreach (var filtroDto in modelo.Filtros)
                {
                    var nuevo = new ProductoFiltroValor
                    {
                        IdProducto = modelo.IdProducto,
                        IdFiltro = filtroDto.IdFiltro,
                        Valor = filtroDto.Valor
                    };
                    await _productoFiltroValorRepositorio.Crear(nuevo);
                }

                var respuesta = await _modeloProductoRepositorio.Editar(fromDbModelo);

                if (!respuesta)
                    throw new TaskCanceledException("No se pudo editar");

                return respuesta;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al editar el producto", ex);
            }
        }

        public async Task<bool> Eliminar(int id)
        {
            try
            {
                var consultaProducto = _modeloProductoRepositorio.Consultar(p => p.IdProducto == id);
                var fromDbProducto = await consultaProducto.FirstOrDefaultAsync();

                if (fromDbProducto == null)
                    throw new TaskCanceledException("No se encontraron resultados");

                var consultaDetalleVenta = _modeloDetalleVentaRepositorio.Consultar(p => p.IdProducto == id);

                if (!await consultaDetalleVenta.AnyAsync())
                {
                    await EliminarImagenesRelacionadas(id);

                    var filtrosActuales = _productoFiltroValorRepositorio.Consultar(f => f.IdProducto == id);
                    foreach (var filtro in await filtrosActuales.ToListAsync())
                    {
                        await _productoFiltroValorRepositorio.Eliminar(filtro);
                    }

                    var respuesta = await _modeloProductoRepositorio.Eliminar(fromDbProducto);
                    if (!respuesta)
                        throw new TaskCanceledException("No se pudo eliminar el producto");

                    return respuesta;
                }
                else
                {
                    fromDbProducto.Baja = true;
                    var respuesta = await _modeloProductoRepositorio.Editar(fromDbProducto);
                    if (!respuesta)
                        throw new TaskCanceledException("No se pudo actualizar el estado de baja del producto");

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task EliminarImagenesRelacionadas(int productoId)
        {
            var consultaImagenes = _productoImagenRepositorio.Consultar(img => img.IdProducto == productoId);
            var imagenesRelacionadas = await consultaImagenes.ToListAsync();

            foreach (var imagen in imagenesRelacionadas)
            {
                await _productoImagenRepositorio.Eliminar(imagen);
            }
        }

        public async Task<List<ProductoDTO>> Lista(string buscar)
        {
            try
            {
                var consulta = _modeloProductoRepositorio.Consultar(p =>
                    p.Nombre.ToLower().Contains(buscar.ToLower())
                );

                consulta = consulta
                    .Include(c => c.IdCategoriaNavigation)
                    .Include(i => i.ProductoImagenes)
                    .Include(p => p.ProductoFiltroValores);

                List<ProductoDTO> lista = _mapper.Map<List<ProductoDTO>>(await consulta.ToListAsync());
                return lista;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ProductoDTO> Obtener(int id)
        {
            try
            {
                var producto = await _modeloProductoRepositorio.Consultar(p => p.IdProducto == id)
                                        .Include(p => p.ProductoImagenes)
                                        .Include(p => p.ProductoFiltroValores)
                                        .FirstOrDefaultAsync();

                return _mapper.Map<ProductoDTO>(producto);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<FiltroDTO>> ObtenerFiltrosPorCategoria(string nombreCategoria)
        {
            var consulta = _categoriaRepositorio
                .Consultar(c => c.Nombre == nombreCategoria)
                .Include(c => c.CategoriasFiltro)
                    .ThenInclude(cf => cf.Filtro);

            var categoria = await consulta.AsNoTracking().FirstOrDefaultAsync();

            if (categoria == null)
                return new List<FiltroDTO>();

            var filtros = categoria.CategoriasFiltro
                                   .Select(cf => cf.Filtro)
                                   .Where(f => f != null)
                                   .Distinct()
                                   .ToList();

            var idCategoria = categoria.IdCategoria;

            // 🔹 Acá traemos todas las opciones disponibles de esos filtros para esa categoría
            var listaOpciones = await _filtroOpcionRepositorio
                .Consultar(fo => fo.IdCategoria == idCategoria)
                .ToListAsync();

            // 🔹 Agrupar opciones por filtro
            var opcionesPorFiltro = listaOpciones
                .GroupBy(fo => fo.IdFiltro)
                .ToDictionary(g => g.Key, g => g.Select(fo => fo.Valor).Distinct().ToList());

            // 🔹 Armar el DTO final
            var resultado = filtros.Select(f => new FiltroDTO
            {
                IdFiltro = f.IdFiltro,
                Nombre = f.Nombre,
                TipoFiltro = f.TipoFiltro,
                OpcionesDisponibles = opcionesPorFiltro.ContainsKey(f.IdFiltro)
                    ? opcionesPorFiltro[f.IdFiltro]
                    : new List<string>()
            }).ToList();

            return resultado;
        }


        public async Task<ResponseDTO<List<ProductoDTO>>> CatalogoConFiltros(string categoria, string buscar, List<ProductoFiltroValorDTO> filtros)
        {
            var response = new ResponseDTO<List<ProductoDTO>>();

            try
            {
                // Empezamos la consulta base (productos activos)
                var consulta = _modeloProductoRepositorio.Consultar(p => !p.Baja);

                // Filtrar por categoria si no es vacía o "todos"
                if (!string.IsNullOrEmpty(categoria))
                {
                    consulta = consulta.Where(p => p.IdCategoriaNavigation.Nombre.ToLower() == categoria.ToLower());
                }

                // Filtrar por texto buscar en nombre
                if (!string.IsNullOrEmpty(buscar))
                {
                    consulta = consulta.Where(p => p.Nombre.ToLower().Contains(buscar.ToLower()));
                }

                // Incluir imágenes y filtros para que estén cargados
                consulta = consulta.Include(p => p.ProductoImagenes)
                                   .Include(p => p.ProductoFiltroValores);

                // Aplicar filtros adicionales si existen
                if (filtros != null && filtros.Any())
                {
                    foreach (var filtro in filtros)
                    {
                        var idFiltro = filtro.IdFiltro;
                        var valorFiltro = filtro.Valor.ToLower();

                        // Filtrar los productos que tengan algún ProductoFiltroValor que coincida con filtro.IdFiltro y valor
                        consulta = consulta.Where(p =>
                            p.ProductoFiltroValores.Any(pf =>
                                pf.IdFiltro == idFiltro &&
                                pf.Valor.ToLower().Contains(valorFiltro)
                            )
                        );
                    }
                }

                // Ejecutar la consulta y mapear a DTO
                var listaProductos = await consulta.ToListAsync();
                var listaDTO = _mapper.Map<List<ProductoDTO>>(listaProductos);

                response.EsCorrecto = true;
                response.Resultado = listaDTO;
            }
            catch (Exception ex)
            {
                response.EsCorrecto = false;
                response.Mensaje = ex.Message;
                response.Resultado = new List<ProductoDTO>();
            }

            return response;
        }

    }
}
