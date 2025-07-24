using Microsoft.AspNetCore.Mvc;
using Ecommerce.Servicio.Contrato;
using Ecommerce.DTO;

namespace Ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUsuarioServicio _usuarioServicio;

        public AuthController(IUsuarioServicio usuarioServicio)
        {
            _usuarioServicio = usuarioServicio;
        }

        // POST api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO modelo)
        {
            var response = new ResponseDTO<SesionDTO>();
            try
            {
                var sesion = await _usuarioServicio.Autorizacion(modelo);
                if (sesion == null)
                {
                    response.EsCorrecto = false;
                    response.Mensaje = "Credenciales inválidas";
                    return Unauthorized(response);
                }

                response.EsCorrecto = true;
                response.Resultado = sesion;
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
