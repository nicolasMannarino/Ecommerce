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
using System.Text.Json;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Servicio.Implementacion
{
    public class UsuarioServicio : IUsuarioServicio
    {
        private readonly IGenericoRepositorio<Usuario> _modeloRepositorio;
        private readonly IMensajeServicio _mensajeServicio;
        private readonly IMapper _mapper;
        public UsuarioServicio(IGenericoRepositorio<Usuario> modeloRepositorio, IMapper mapper, IMensajeServicio mensajeServicio)
        {
            _modeloRepositorio = modeloRepositorio;
            _mapper = mapper;
            _mensajeServicio = mensajeServicio;
        }

        public async Task<SesionDTO> Autorizacion(LoginDTO modelo)
        {
            try
            {
                var consulta = _modeloRepositorio.Consultar(p => p.Correo == modelo.Correo);
                var fromDbModelo = await consulta.FirstOrDefaultAsync();

                if (fromDbModelo == null)
                    throw new TaskCanceledException("No se encontró un usuario con ese correo.");

                var hasher = new PasswordHasher<Usuario>();
                var resultado = hasher.VerifyHashedPassword(fromDbModelo, fromDbModelo.Clave, modelo.Clave);

                if (resultado == PasswordVerificationResult.Success)
                {
                    return _mapper.Map<SesionDTO>(fromDbModelo);
                }
                else
                {
                    throw new TaskCanceledException("La contraseña ingresada es incorrecta.");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<UsuarioDTO> Crear(UsuarioDTO modelo)
        {
            try
            {
                var consulta = _modeloRepositorio.Consultar(p => p.Correo == modelo.Correo);
                var fromDbModelo = await consulta.FirstOrDefaultAsync();

                if (fromDbModelo == null)
                {
                    var dbModelo = _mapper.Map<Usuario>(modelo);

                    // Hashear la contraseña antes de guardar
                    var hasher = new PasswordHasher<Usuario>();
                    dbModelo.Clave = hasher.HashPassword(dbModelo, modelo.Clave);

                    var rspModelo = await _modeloRepositorio.Crear(dbModelo);

                    if (rspModelo.IdUsuario != 0)
                    {
                        var mensaje = JsonSerializer.Serialize(new { Email = modelo.Correo, Nombre = modelo.Nombre });
                        await _mensajeServicio.EnviarMensajeAsync("cola_registro_usuario", mensaje);
                        return _mapper.Map<UsuarioDTO>(rspModelo);
                    }
                    else
                    {
                        throw new TaskCanceledException("No se puede crear");
                    }
                }
                else
                {
                    throw new TaskCanceledException("Ya existe un usuario con el correo electrónico ingresado.");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> Editar(UsuarioDTO modelo)
        {
            var fromDbModelo = await _modeloRepositorio.Consultar(p => p.IdUsuario == modelo.IdUsuario)
                                                       .FirstOrDefaultAsync();

            if (fromDbModelo == null)
                throw new Exception("Usuario no encontrado");

            fromDbModelo.Nombre = modelo.Nombre;
            fromDbModelo.Apellido = modelo.Apellido;
            fromDbModelo.Correo = modelo.Correo;

            // Actualiza la contraseña solo si se envió una nueva
            if (!string.IsNullOrWhiteSpace(modelo.Clave))
            {
                if (modelo.Clave.Length < 8)
                    throw new Exception("La contraseña debe tener al menos 8 caracteres");

                var hasher = new PasswordHasher<Usuario>();
                fromDbModelo.Clave = hasher.HashPassword(fromDbModelo, modelo.Clave);
            }

            var respuesta = await _modeloRepositorio.Editar(fromDbModelo);

            if (!respuesta)
                throw new Exception("No se pudo editar");

            return respuesta;
        }



        public async Task<bool> Eliminar(int id)
        {
            try
            {
                var consulta = _modeloRepositorio.Consultar(p => p.IdUsuario == id);
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
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<UsuarioDTO>> Lista(string buscar)
        {
            try
            {
                var consulta = _modeloRepositorio.Consultar(p => string.Concat(p.Nombre.ToLower(), p.Apellido.ToLower(), p.Correo.ToLower()).Contains(buscar.ToLower()));

                List<UsuarioDTO> lista = _mapper.Map<List<UsuarioDTO>>(await consulta.ToListAsync());
                return lista;
            }
            catch (Exception)
            {
                throw; 
            }
        }

        public async Task<List<UsuarioDTO>> Lista(string rol, string buscar)
        {
            try
            {
                // Normalizar buscar para evitar nulls
                buscar = buscar ?? "";

                // Consulta base
                var consulta = _modeloRepositorio.Consultar(p =>
                    (string.IsNullOrEmpty(rol) || p.Rol == rol) &&  // filtro rol solo si no es null o vacío
                    string.Concat(p.Nombre.ToLower(), p.Apellido.ToLower(), p.Correo.ToLower()).Contains(buscar.ToLower()));

                List<UsuarioDTO> lista = _mapper.Map<List<UsuarioDTO>>(await consulta.ToListAsync());
                return lista;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<UsuarioDTO> Obtener(int id)
        {
            try
            {
                var consulta = _modeloRepositorio.Consultar(p => p.IdUsuario == id);
                var fromDbModelo = await consulta.FirstOrDefaultAsync();

                if (fromDbModelo != null)
                    return _mapper.Map<UsuarioDTO>(fromDbModelo);
                else
                    throw new TaskCanceledException("No se encontraron coincidencias");
            }
            catch (Exception)
            {
                throw; 
            }
        }
    }
}
