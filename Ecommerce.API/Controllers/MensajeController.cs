using Ecommerce.DTO;
using Ecommerce.Servicio.Contrato;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MensajeController : ControllerBase
    {
        private readonly ICorreoServicio _correoServicio;

        public MensajeController(ICorreoServicio correoServicio)
        {
            _correoServicio = correoServicio;
        }

        [HttpPost("Enviar")]
        public async Task<IActionResult> Enviar([FromBody] MensajeDTO mensajeDto)
        {
            var response = new ResponseDTO<bool>();

            try
            {
                // Envía el mensaje a la cola para ser procesado y enviar el correo de bienvenida
                await _correoServicio.EnviarCorreoBienvenida(
                           mensajeDto.Destinatario,
                           "Bienvenido a MANIES SA",
                           $"Hola {mensajeDto.Nombre},\n\n" +
                           "¡Bienvenido a nuestra plataforma!\n\n" +
                           "Para iniciar sesión utiliza las siguientes credenciales:\n\n" +
                           $"Correo: {mensajeDto.Destinatario}\n" +
                           "Contraseña: La que ingresó cuando registró la cuenta.\n\n" +
                           "Lo esperamos para su próxima compra.\n\n" +
                           "Cualquier consulta, contactarse con el número que se encuentra en la página."
                       );

                response.EsCorrecto = true;
                response.Resultado = true;
            }
            catch (Exception ex)
            {
                response.EsCorrecto = false;
                response.Mensaje = ex.Message;
            }

            return Ok(response);
        }
    }
}
