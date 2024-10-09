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
        private readonly IMapper _mapper;

        public ProductoServicio(IGenericoRepositorio<Producto> modeloRepositorio, IGenericoRepositorio<ProductoImagen> productoImagenRepositorio, IGenericoRepositorio<DetalleVenta> modeloDetalleVentaRepositorio, IMapper mapper)
        {
            _modeloProductoRepositorio = modeloRepositorio;
            _modeloDetalleVentaRepositorio = modeloDetalleVentaRepositorio;
            _productoImagenRepositorio = productoImagenRepositorio;
            _mapper = mapper;
        }

        public async Task<List<ProductoDTO>> Catalogo(string categoria, string buscar)
        {
            try
            {
                var consulta = _modeloProductoRepositorio.Consultar(p =>
                p.Nombre.ToLower().Contains(buscar.ToLower()) &&
                p.IdCategoriaNavigation.Nombre.ToLower().Contains(categoria.ToLower())&&
                p.Baja != true);

                consulta = consulta.Include(i => i.ProductoImagenes);

                List<ProductoDTO> lista = _mapper.Map<List<ProductoDTO>>(await consulta.ToListAsync());
                return lista;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public async Task<ProductoDTO> Crear(ProductoDTO modelo)
        {
            try
            {
                // Mapea el DTO al modelo de la base de datos
                var dbModelo = _mapper.Map<Producto>(modelo);

                // Crea el producto en la base de datos utilizando el repositorio
                var rspModelo = await _modeloProductoRepositorio.Crear(dbModelo);

                // Verifica si el producto se creó correctamente y tiene un ID válido
                if (rspModelo.IdProducto == 0)
                {
                    throw new TaskCanceledException("No se pudo crear el producto en la base de datos.");
                }

                // Log para verificar que el producto fue creado correctamente y tiene un ID asignado
                Console.WriteLine($"Producto creado con Id: {rspModelo.IdProducto}");

                return _mapper.Map<ProductoDTO>(rspModelo);
            }
            catch (Exception ex)
            {
                // Log del error para diagnóstico
                Console.WriteLine($"Error al crear el producto: {ex.Message}");
                throw new Exception("Error al crear el producto y sus imágenes", ex);
            }
        }

        public async Task<bool> Editar(ProductoDTO modelo)
        {
            try
            {
                // Consultar el producto y sus imágenes actuales
                var consulta = _modeloProductoRepositorio.Consultar(p => p.IdProducto == modelo.IdProducto)
                                                 .Include(p => p.ProductoImagenes);

                var fromDbModelo = await consulta.FirstOrDefaultAsync();

                if (fromDbModelo != null)
                {
                    // Actualizar los campos del producto
                    fromDbModelo.Nombre = modelo.Nombre;
                    fromDbModelo.Descripcion = modelo.Descripcion;
                    fromDbModelo.IdCategoria = modelo.IdCategoria;
                    fromDbModelo.Precio = modelo.Precio;
                    fromDbModelo.PrecioOferta = modelo.PrecioOferta;
                    fromDbModelo.Cantidad = modelo.Cantidad;

                    var imagenesActuales = fromDbModelo.ProductoImagenes?.ToList() ?? new List<ProductoImagen>();

                    // Actualizar o agregar nuevas imágenes
                    foreach (var imgDto in modelo.Imagenes)
                    {
                        var imagenExistente = imagenesActuales.FirstOrDefault(i => i.IdProductoImagen == imgDto.IdImagen && i.NumeroImagen == imgDto.NumeroImagen);

                        if (imagenExistente != null)
                        {
                            // Actualizar la ruta de la imagen existente
                            imagenExistente.RutaImagen = imgDto.RutaImagen;
                        }
                        else
                        {
                            // Crear una nueva imagen si no existe
                            fromDbModelo.ProductoImagenes.Add(new ProductoImagen
                            {
                                IdProducto = modelo.IdProducto,
                                NumeroImagen = imgDto.NumeroImagen,
                                RutaImagen = imgDto.RutaImagen
                            });
                        }
                    }

                    // Eliminar imágenes que no están en la lista actual
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
                // Manejo de excepciones
                throw new Exception("Error al editar el producto", ex);
            }
        }

        public async Task<bool> Eliminar(int id)
        {
            try
            {
                // Consulta el producto a eliminar
                var consultaProducto = _modeloProductoRepositorio.Consultar(p => p.IdProducto == id);
                var fromDbProducto = await consultaProducto.FirstOrDefaultAsync();

                if (fromDbProducto == null)
                    throw new TaskCanceledException("No se encontraron resultados");

                var consultaDetalleVenta = _modeloDetalleVentaRepositorio.Consultar( p => p.IdProducto == id);
                
                if(consultaDetalleVenta == null) //No existe detalle venta para ese id de producto
                {
                    // Llama a la función para eliminar las imágenes relacionadas
                    await EliminarImagenesRelacionadas(id);

                    // Elimina el producto
                    var respuesta = await _modeloProductoRepositorio.Eliminar(fromDbProducto);

                    // Confirma que el producto se eliminó correctamente
                    if (!respuesta)
                        throw new TaskCanceledException("No se pudo eliminar el producto");
                    return respuesta;
                }
                else
                {

                    fromDbProducto.Baja = true;

                    // Guarda el cambio
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
            // Consulta las imágenes relacionadas al producto
            var consultaImagenes = _productoImagenRepositorio.Consultar(img => img.IdProducto == productoId);
            var imagenesRelacionadas = await consultaImagenes.ToListAsync();

            // Elimina cada imagen individualmente
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
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /*public async Task<ProductoDTO> Obtener(int id)
        {
            try
            {
                var consulta = _modeloRepositorio.Consultar(p => p.IdProducto == id);
                consulta = consulta.Include(c => c.IdCategoriaNavigation)
                                   .Include(i => i.ProductoImagenes);
                var fromDbModelo = await consulta.FirstOrDefaultAsync();

                if (fromDbModelo != null)
                    return _mapper.Map<ProductoDTO>(fromDbModelo);
                else
                    throw new TaskCanceledException("No se encontraron coincidencias");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        */
        public async Task<ProductoDTO> Obtener(int id)
        {
            try
            {
                var producto = await _modeloProductoRepositorio.Consultar(p => p.IdProducto == id)
                                        .Include(p => p.ProductoImagenes)
                                        .FirstOrDefaultAsync();
                return _mapper.Map<ProductoDTO>(producto);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
