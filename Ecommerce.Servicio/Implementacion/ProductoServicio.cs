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
        private readonly IMapper _mapper;

        public ProductoServicio(
            IGenericoRepositorio<Producto> modeloRepositorio,
            IGenericoRepositorio<ProductoImagen> productoImagenRepositorio,
            IGenericoRepositorio<DetalleVenta> modeloDetalleVentaRepositorio,
            IGenericoRepositorio<Categoria> categoriaRepositorio,
            IMapper mapper)
        {
            _modeloProductoRepositorio = modeloRepositorio;
            _modeloDetalleVentaRepositorio = modeloDetalleVentaRepositorio;
            _productoImagenRepositorio = productoImagenRepositorio;
            _categoriaRepositorio = categoriaRepositorio;
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

                consulta = consulta.Include(i => i.ProductoImagenes);

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

                Console.WriteLine($"Producto creado con Id: {rspModelo.IdProducto}");

                return _mapper.Map<ProductoDTO>(rspModelo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear el producto: {ex.Message}");
                throw new Exception("Error al crear el producto y sus imágenes", ex);
            }
        }

        public async Task<bool> Editar(ProductoDTO modelo)
        {
            try
            {
                var consulta = _modeloProductoRepositorio.Consultar(p => p.IdProducto == modelo.IdProducto)
                                                 .Include(p => p.ProductoImagenes);

                var fromDbModelo = await consulta.FirstOrDefaultAsync();

                if (fromDbModelo != null)
                {
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

                    var respuesta = await _modeloProductoRepositorio.Editar(fromDbModelo);

                    if (!respuesta)
                        throw new TaskCanceledException("No se pudo editar");
                    return respuesta;
                }
                else
                {
                    throw new TaskCanceledException("No se encontraron resultados");
                }
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
                Console.WriteLine($"Error al eliminar el producto: {ex.Message}");
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

                consulta = consulta.Include(c => c.IdCategoriaNavigation)
                                   .Include(i => i.ProductoImagenes);

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
            try
            {
                var consulta = _categoriaRepositorio.Consultar(c => c.Nombre.ToLower() == nombreCategoria.ToLower())
                    .Include(c => c.CategoriasFiltro)
                        .ThenInclude(cf => cf.Filtro);

                var categoria = await consulta.FirstOrDefaultAsync();

                if (categoria == null)
                    return new List<FiltroDTO>();

                var filtros = categoria.CategoriasFiltro
                                       .Select(cf => cf.Filtro)
                                       .Distinct()
                                       .ToList();

                return _mapper.Map<List<FiltroDTO>>(filtros);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
