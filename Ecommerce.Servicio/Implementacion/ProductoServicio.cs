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
        private readonly IGenericoRepositorio<Producto> _modeloRepositorio;
        private readonly IGenericoRepositorio<ProductoImagen> _productoImagenRepositorio;
        private readonly IMapper _mapper;

        public ProductoServicio(IGenericoRepositorio<Producto> modeloRepositorio, IGenericoRepositorio<ProductoImagen> productoImagenRepositorio, IMapper mapper)
        {
            _modeloRepositorio = modeloRepositorio;
            _productoImagenRepositorio = productoImagenRepositorio;
            _mapper = mapper;
        }

        public async Task<List<ProductoDTO>> Catalogo(string categoria, string buscar)
        {
            try
            {
                var consulta = _modeloRepositorio.Consultar(p =>
                p.Nombre.ToLower().Contains(buscar.ToLower()) &&
                p.IdCategoriaNavigation.Nombre.ToLower().Contains(categoria.ToLower()));

                consulta = consulta.Include(i => i.ProductoImagenes);

                List<ProductoDTO> lista = _mapper.Map<List<ProductoDTO>>(await consulta.ToListAsync());
                return lista;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /*public async Task<ProductoDTO> Crear(ProductoDTO modelo)
        {
            try
            {
                var dbModelo = _mapper.Map<Producto>(modelo);
                var rspModelo = await _modeloRepositorio.Crear(dbModelo);

                if (rspModelo.IdProducto != 0)
                    return _mapper.Map<ProductoDTO>(rspModelo);
                else
                    throw new TaskCanceledException("No se puede crear");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }*/

        public async Task<ProductoDTO> Crear(ProductoDTO modelo)
        {
            try
            {
                // Mapea el DTO al modelo de la base de datos
                var dbModelo = _mapper.Map<Producto>(modelo);

                // Crea el producto en la base de datos utilizando el repositorio
                var rspModelo = await _modeloRepositorio.Crear(dbModelo);

                // Verifica si el producto se creó correctamente y tiene un ID válido
                if (rspModelo.IdProducto == 0)
                {
                    throw new TaskCanceledException("No se pudo crear el producto en la base de datos.");
                }

                // Log para verificar que el producto fue creado correctamente y tiene un ID asignado
                Console.WriteLine($"Producto creado con Id: {rspModelo.IdProducto}");

                // Inserta cada imagen en la tabla ProductoImagen
                /*foreach (var imagen in modelo.Imagenes)
                {
                    var imagenModelo = new ProductoImagen
                    {
                        IdProducto = rspModelo.IdProducto,  // Asegúrate de usar el ID correcto aquí
                        NumeroImagen = imagen.NumeroImagen,
                        RutaImagen = imagen.RutaImagen
                    };

                    // Log para verificar los detalles de la imagen antes de crearla
                    Console.WriteLine($"Agregando imagen: {imagenModelo.RutaImagen} con NumeroImagen: {imagenModelo.NumeroImagen} al ProductoId: {imagenModelo.IdProducto}");

                    await _productoImagenRepositorio.Crear(imagenModelo);
                }*/

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
                var consulta = _modeloRepositorio.Consultar(p => p.IdProducto == modelo.IdProducto)
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

                    var respuesta = await _modeloRepositorio.Editar(fromDbModelo);

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
                var consulta = _modeloRepositorio.Consultar(p => p.IdProducto == id);
                var fromDbModelo = await consulta.FirstOrDefaultAsync();

                if (fromDbModelo != null)
                {
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
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<List<ProductoDTO>> Lista(string buscar)
        {
            try
            {
                var consulta = _modeloRepositorio.Consultar(p =>
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

        public async Task<ProductoDTO> Obtener(int id)
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

    }
}
