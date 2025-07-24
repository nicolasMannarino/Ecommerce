using Microsoft.AspNetCore.Mvc;
using Ecommerce.Servicio.Contrato;
using Ecommerce.DTO;

namespace Ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioServicio _usuarioServicio;

        public UsuarioController(IUsuarioServicio usuarioServicio)
        {
            _usuarioServicio = usuarioServicio;
        }

        // GET api/usuario?rol=admin&buscar=juan
        [HttpGet]
        public async Task<IActionResult> Lista([FromQuery] string rol = null, [FromQuery] string buscar = "")
        {
            var response = new ResponseDTO<List<UsuarioDTO>>();
            try
            {
                response.EsCorrecto = true;
                response.Resultado = await _usuarioServicio.Lista(rol, buscar);
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.EsCorrecto = false;
                response.Mensaje = ex.Message;
                return StatusCode(500, response);
            }
        }

        // GET api/usuario/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Obtener(int id)
        {
            var response = new ResponseDTO<UsuarioDTO>();
            try
            {
                var usuario = await _usuarioServicio.Obtener(id);
                if (usuario == null)
                {
                    response.EsCorrecto = false;
                    response.Mensaje = "Usuario no encontrado";
                    return NotFound(response);
                }
                response.EsCorrecto = true;
                response.Resultado = usuario;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.EsCorrecto = false;
                response.Mensaje = ex.Message;
                return StatusCode(500, response);
            }
        }

        // POST api/usuario
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] UsuarioDTO modelo)
        {
            var response = new ResponseDTO<UsuarioDTO>();
            try
            {
                var usuarioCreado = await _usuarioServicio.Crear(modelo);
                response.EsCorrecto = true;
                response.Resultado = usuarioCreado;
                return CreatedAtAction(nameof(Obtener), new { id = usuarioCreado.IdUsuario }, response);
            }
            catch (Exception ex)
            {
                response.EsCorrecto = false;
                response.Mensaje = ex.Message;
                return BadRequest(response);
            }
        }

        // PUT api/usuario/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Editar(int id, [FromBody] UsuarioDTO modelo)
        {
            var response = new ResponseDTO<bool>();
            try
            {
                if (id != modelo.IdUsuario)
                {
                    response.EsCorrecto = false;
                    response.Mensaje = "El ID de la URL no coincide con el ID del cuerpo";
                    return BadRequest(response);
                }

                var resultado = await _usuarioServicio.Editar(modelo);
                response.EsCorrecto = resultado;
                response.Resultado = resultado;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.EsCorrecto = false;
                response.Mensaje = ex.Message;
                return StatusCode(500, response);
            }
        }

        // DELETE api/usuario/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var response = new ResponseDTO<bool>();
            try
            {
                var resultado = await _usuarioServicio.Eliminar(id);
                if (!resultado)
                {
                    response.EsCorrecto = false;
                    response.Mensaje = "No se pudo eliminar el usuario";
                    return BadRequest(response);
                }

                response.EsCorrecto = true;
                response.Resultado = true;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.EsCorrecto = false;
                response.Mensaje = ex.Message;
                return StatusCode(500, response);
            }
        }
    }
}
